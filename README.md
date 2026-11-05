# Practical assignments 9 and 10

The main branch combines the CRUD and DTO fixes from assignment 9 with the layered architecture, AutoMapper, and reservation operation from assignment 10.

## Run

Use SQL Server `.\SQLEXPRESS` and the `AutoFixDB` database.

- For a new installation, run `SQLQueryAutoFixDB.sql` to create the original eight tables and their initial data, then run `SQLQueryAutoFixDB.Update.sql` to add stock quantities and reservations for assignment 10.
- If the original database already exists, run only `SQLQueryAutoFixDB.Update.sql`. It preserves existing rows and can be executed repeatedly.
- The creation script refuses to initialize a database that already contains application tables. Neither script drops an existing database or table.

Run the scripts in SQL Server Management Studio, or use `sqlcmd -S .\SQLEXPRESS -E -C -b -i SQLQueryAutoFixDB.sql` followed by `sqlcmd -S .\SQLEXPRESS -E -C -b -i SQLQueryAutoFixDB.Update.sql` for a new installation.

Run `dotnet run --project asp_net_3-2` and open `/swagger` at the development URL printed by the application.

## Architecture

- Controllers call services and produce HTTP responses. The exception filter translates domain errors into 404 or 409 responses.
- Services own business rules and use AutoMapper. They do not use DbContext or HTTP result types.
- Repositories own Entity Framework queries and persistence.
- Services, repositories, and IMapper use Scoped lifetimes. AddDbContext also uses Scoped by default. Each HTTP request therefore shares one DbContext across its repositories, allowing changes to related entities to be saved together.
- Read DTOs contain nested related data; write DTOs use related IDs. PATCH mappings skip null values.
- Required numeric and boolean write fields must be present in JSON. Explicit zero prices, zero stock, and false activity flags remain valid.
- All eight original entities have complete CRUD endpoints, and reservations add a ninth resource. Creating or fully updating a resource requires every writable field; PATCH fields remain optional.
- Removing a cart or order explicitly removes its items within the same save operation. Other foreign-key conflicts return 409.

## Reservation operation

POST `/api/reservations` checks the order, its user, the product, and available stock. It decreases stock and creates the reservation using one SaveChangesAsync call. POST `/api/reservations/{id}/cancel` restores stock and sets CancelledAt using one SaveChangesAsync call. Missing active reservations return 404; business conflicts return 409. No separate UnitOfWork or explicit transaction is introduced. EF Core saves both changes atomically, and concurrency tokens detect competing updates.

## Verification on 2026-10-31

The combined main version built successfully. Automated checks sent 258 HTTP requests using an isolated SQL Server database, and all 272 checks passed. They verified all six CRUD endpoints for all nine resources, paging and X-Total-Count, nested DTOs, missing records, request validation, empty and null PATCH, related-record deletion conflicts, removal of a cart with its items, reservations, quantity changes, insufficient stock, inactive users, cancellation, and repeated cancellation.

Two simultaneous reservation requests could not oversell the product. Two simultaneous cancellation requests restored stock only once in the tested scenario. Temporary database constraints forced failures during creation and cancellation; both tests confirmed that stock and the reservation record rolled back together. Separate checks confirmed that missing numeric and boolean fields return 400 without modifying stored records, while explicitly supplied zero and false values work.

The schema update was also applied twice to the existing local AutoFixDB database. Existing table row counts were preserved, and the new column and reservation table were created.

The existing transitive `System.Security.Cryptography.Pkcs` 9.0.11 dependency still produces NuGet audit warnings NU1902 and NU1903. This verification summary does not include the Swagger or Postman screenshots required for the submission report.
