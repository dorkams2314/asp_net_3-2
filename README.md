# Practical assignments 9, 10, and 11

The API combines layered CRUD and reservations with JWT authentication, role and resource authorization, CORS, and login rate limiting. The separate React client demonstrates requests from another origin.

## Run

Use SQL Server `.\SQLEXPRESS` and the `AutoFixDB` database.

- For a new installation, run `SQLQueryAutoFixDB.sql`, then `SQLQueryAutoFixDB.Update.sql`, then `SQLQueryAutoFixDB.Security.sql`.
- For an existing database, run the update and security scripts. They preserve existing rows and can be executed repeatedly. The security script adds a unique login index, creates Admin/User roles, and maps the original Russian roles to them.
- The creation script refuses to initialize a database that already contains application tables. Neither script drops an existing database or table.

Run the scripts in SQL Server Management Studio, or run these commands from the repository root for a new installation:

```powershell
sqlcmd -S .\SQLEXPRESS -E -C -b -f 65001 -i SQLQueryAutoFixDB.sql
sqlcmd -S .\SQLEXPRESS -E -C -b -f 65001 -i SQLQueryAutoFixDB.Update.sql
sqlcmd -S .\SQLEXPRESS -E -C -b -f 65001 -i SQLQueryAutoFixDB.Security.sql
```

In SSMS, choose **Database Engine**, server `.\SQLEXPRESS`, and **Windows Authentication**. For the local development instance, enable **Trust server certificate** if its generated certificate is not trusted.

Run `dotnet run --project asp_net_3-2 --launch-profile http`. The API and Swagger are available at `http://localhost:5152` and `http://localhost:5152/swagger`.

In another terminal, run `npm ci` followed by `npm run dev` from `frontend`. Open `http://localhost:5173`. Vite calls the API directly, without a proxy. Node.js 22.12+ and the .NET 10 SDK are required.

To demonstrate an actual browser CORS rejection, run `npm run dev:blocked` in a third terminal and open `http://localhost:5174`. Click **Without a token**. The browser refuses to expose the API response to this origin. JavaScript cannot distinguish CORS from other network failures; use the browser console to confirm the CORS reason.

### Configuration

- `ConnectionStrings:AutoFixDb`: SQL Server connection string. It can be overridden with the `ConnectionStrings__AutoFixDb` environment variable.
- `Cors:FrontendOrigin`: the single allowed origin, initially `http://localhost:5173`. Do not append a slash. Keep the client's address consistent with this setting.
- `Jwt:Issuer`, `Jwt:Audience`, `Jwt:LifetimeMinutes`: token validation and issuance settings.
- `Jwt:Key`: provide a random secret with at least 32 characters through `Jwt__Key` or .NET user secrets. Do not commit it. Development generates an ephemeral signing key if none is configured, so restarting the API invalidates existing tokens. Production requires an explicit key and HTTPS.
- `frontend/.env.local`: optional `VITE_API_URL`, based on `.env.example`. Restart Vite after changing it. The default is `http://localhost:5152`.

### Accounts and administrator setup

Register two accounts using **Create account**. Registration always assigns the User role, hashes the password with `PasswordHasher<User>`, and returns an access token. No request accepts a client-supplied password hash or registration role. Passwords must contain 8–128 characters. Logins use letters, digits, dots, underscores, or hyphens and are converted to lowercase.

The original seed accounts contain bcrypt hashes, which cannot be converted to PasswordHasher hashes without their plaintext passwords. They cannot sign in through the new authentication endpoint; use newly registered accounts for this assignment. Existing records are retained.

To grant administrator rights to a newly registered account, run this in SSMS against `AutoFixDB`, replacing `your.admin.login` with that account's login:

```sql
UPDATE dbo.Users
SET RoleId = (SELECT Id FROM dbo.Roles WHERE Name = N'Admin')
WHERE Login = N'your.admin.login';
```

Sign out and sign in again after changing a role. Tokens are checked against the current account's active flag and role on every protected request; deactivation or a changed role invalidates an existing token. To demonstrate inactivity, set `IsActive = 0` in SSMS and request the protected profile again.

### Access rules and demonstration

| Request | Anonymous | User | Admin |
| --- | --- | --- | --- |
| POST `/api/auth/register`, `/api/auth/login` | Allowed | Allowed | Allowed |
| GET `/api/users/me` | 401 | Own profile | Own profile |
| GET/PUT `/api/users/{id}` | 401 | Own profile only; another existing profile returns 403 | Any existing profile |
| GET `/api/users` | 401 | 403 | Paginated accounts |
| Existing catalog, roles, cart, order, and reservation CRUD | 401 | 403 | Allowed |

The resource policy uses `OwnerRequirement` and `ProfileAuthorizationHandler : AuthorizationHandler<OwnerRequirement, UserDto>`. Controllers load the resource and invoke `IAuthorizationService`; profile updates accept only a display name, so ownership cannot be used to change roles or activity. Missing profiles return 404, duplicate registrations return 409, invalid DTOs return 400, and incorrect credentials or inactive accounts return 401.

1. Register or sign in at port 5173. The client immediately requests `/api/users/me` with the saved token as `Authorization: Bearer ...`; both requests appear in the journal. **Load my profile** repeats that protected request.
2. Save your own display name: expect 204. Enter the other account's existing ID and try again: expect 403. Sign in as Admin and repeat: expect 204.
3. Click **Administrator area**: expect 403 as User and 200 as Admin. Click **Without a token**: expect 401, with the current session retained for further checks.
4. Click **Try a failed sign-in** repeatedly: the login endpoint allows five requests per remote IP per one-minute fixed window; further attempts return 429 with `Retry-After`. Successful and invalid requests also consume permits. Wait for the window to reset before signing in again.
5. Open the client at port 5174 and make a request to confirm browser CORS rejection. Return to port 5173 and repeat successfully.
6. In Swagger, paste the JWT into **Authorize**. Only protected operations display the security requirement; registration and login remain public.

The client stores the access token in sessionStorage for the current tab and hides it in the on-screen request journal. It displays successful responses, HTTP 401/403/429, validation errors, and network/CORS failures. The journal contains actual requests and responses, not simulated results.

## Architecture

- Controllers call services and produce HTTP responses. The exception filter translates domain errors into 404 or 409 responses.
- Services own business rules and use AutoMapper. They do not use DbContext or HTTP result types.
- Repositories own Entity Framework queries and persistence.
- Services, repositories, and IMapper use Scoped lifetimes. AddDbContext also uses Scoped by default. Each HTTP request therefore shares one DbContext across its repositories, allowing changes to related entities to be saved together.
- Read DTOs contain nested related data; write DTOs use related IDs. PATCH mappings skip null values.
- Required numeric and boolean write fields must be present in JSON. Explicit zero prices, zero stock, and false activity flags remain valid.
- The original business entities retain their CRUD endpoints. User account operations now use registration, login, profile reading/editing, and an administrator-only account list instead of generic password-hash CRUD.
- Removing a cart or order explicitly removes its items within the same save operation. Other foreign-key conflicts return 409.

## Reservation operation

POST `/api/reservations` checks the order, its user, the product, and available stock. It decreases stock and creates the reservation using one SaveChangesAsync call. POST `/api/reservations/{id}/cancel` restores stock and sets CancelledAt using one SaveChangesAsync call. Missing active reservations return 404; business conflicts return 409. No separate UnitOfWork or explicit transaction is introduced. EF Core saves both changes atomically, and concurrency tokens detect competing updates.

## Verification on 2026-10-31

This section records the historical assignment 9/10 checks before assignment 11 introduced authentication; it does not verify the current security flow.

The combined main version built successfully. Automated checks sent 258 HTTP requests using an isolated SQL Server database, and all 272 checks passed. They verified all six CRUD endpoints for all nine resources, paging and X-Total-Count, nested DTOs, missing records, request validation, empty and null PATCH, related-record deletion conflicts, removal of a cart with its items, reservations, quantity changes, insufficient stock, inactive users, cancellation, and repeated cancellation.

Two simultaneous reservation requests could not oversell the product. Two simultaneous cancellation requests restored stock only once in the tested scenario. Temporary database constraints forced failures during creation and cancellation; both tests confirmed that stock and the reservation record rolled back together. Separate checks confirmed that missing numeric and boolean fields return 400 without modifying stored records, while explicitly supplied zero and false values work.

The schema update was also applied twice to the existing local AutoFixDB database. Existing table row counts were preserved, and the new column and reservation table were created.

The existing transitive `System.Security.Cryptography.Pkcs` 9.0.11 dependency still produces NuGet audit warnings NU1902 and NU1903. This verification summary does not include the Swagger or Postman screenshots required for the submission report.

## Assignment 11 verification on 2026-11-05

SQL Server 2025 Express was installed as `.\SQLEXPRESS` with automatic startup, Windows authentication, and database files under `I:\SQLServer`. `AutoFixDB` is online. The schema, reservation, and security scripts were applied; the security script was also applied repeatedly to confirm idempotency. Unicode seed literals and UTF-8 sqlcmd input preserve the original text.

The API and React production builds passed. Forty-eight HTTP/database checks passed against the real database: registration, server-assigned roles, PasswordHasher persistence, duplicate login conflicts, login, owner editing, non-owner denial, administrator editing/listing, inactive account rejection, role-change invalidation, missing/malformed/tampered/expired/future tokens, wrong issuer/audience, login throttling with Retry-After, Swagger security metadata, CORS preflights for allowed and denied origins, and both React development server responses. These are HTTP/database checks; they do not establish a completed interactive browser sign-in walkthrough or visual review.

Local demonstration accounts are `demo.reader`, `demo.other`, and `demo.admin`. Their generated passwords and IDs are in `.local/demo-accounts.txt`, which is ignored by Git. Detailed check results are in `.local/security-audit.json`. The persistent development signing key is stored in .NET user secrets. The vulnerable historical Pkcs dependency is pinned to patched 9.0.21 for the current build.
