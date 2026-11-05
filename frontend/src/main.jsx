import React, { useState } from "react";
import { createRoot } from "react-dom/client";
import "./styles.css";

const api = (import.meta.env.VITE_API_URL || "http://localhost:5152").replace(
  /\/$/,
  "",
);

function readSession() {
  try {
    const value = JSON.parse(sessionStorage.getItem("autofix-session"));
    return value && Date.parse(value.expiresAt) > Date.now() ? value : null;
  } catch {
    return null;
  }
}

function App() {
  const [session, setSession] = useState(readSession);
  const [mode, setMode] = useState("login");
  const [busy, setBusy] = useState(false);
  const [notice, setNotice] = useState(null);
  const [history, setHistory] = useState([]);
  const [profile, setProfile] = useState(null);
  const [target, setTarget] = useState("");
  const [displayName, setDisplayName] = useState("");

  function saveSession(value) {
    setSession(value);
    if (value) sessionStorage.setItem("autofix-session", JSON.stringify(value));
    else sessionStorage.removeItem("autofix-session");
  }

  async function request(
    path,
    { method = "GET", body, token = session?.accessToken } = {},
  ) {
    setBusy(true);
    setNotice(null);
    try {
      const response = await fetch(`${api}${path}`, {
        method,
        headers: {
          ...(body ? { "Content-Type": "application/json" } : {}),
          ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        ...(body ? { body: JSON.stringify(body) } : {}),
        signal: AbortSignal.timeout(10000),
      });
      const data =
        response.status === 204
          ? null
          : await response.json().catch(() => null);
      const retry = response.headers.get("Retry-After");
      const messages = {
        401: "Please sign in again. Your credentials or access token were not accepted.",
        403: "Access denied. This action needs an administrator or the owner of this profile.",
        429: `Too many sign-in attempts. ${retry ? `Try again in ${retry} seconds.` : "Wait a minute and try again."}`,
      };
      const detail = response.ok
        ? "Request completed successfully."
        : messages[response.status] ||
          data?.detail ||
          Object.values(data?.errors || {})
            .flat()
            .join(" ") ||
          "Request failed.";
      setNotice({ ok: response.ok, status: response.status, detail });
      setHistory((items) =>
        [
          {
            method,
            path,
            status: response.status,
            data,
            time: new Date().toLocaleTimeString(),
          },
          ...items,
        ].slice(0, 12),
      );
      if (response.status === 401 && token === session?.accessToken && token) {
        saveSession(null);
        setProfile(null);
      }
      return response.ok ? (data ?? true) : null;
    } catch (error) {
      const detail =
        error.name === "TimeoutError"
          ? "The API did not respond within 10 seconds."
          : "The browser could not read the response. CORS may have blocked it, or the API may be offline. Check the browser console and the API address.";
      setNotice({ ok: false, status: "Network / CORS", detail });
      setHistory((items) =>
        [
          {
            method,
            path,
            status: "Blocked / offline",
            data: { detail },
            time: new Date().toLocaleTimeString(),
          },
          ...items,
        ].slice(0, 12),
      );
      return null;
    } finally {
      setBusy(false);
    }
  }

  async function authenticate(event) {
    event.preventDefault();
    const fields = Object.fromEntries(new FormData(event.currentTarget));
    const data = await request(`/api/auth/${mode}`, {
      method: "POST",
      body: fields,
      token: null,
    });
    if (data) {
      saveSession(data);
      setTarget(String(data.user.id));
      setDisplayName(data.user.userName);
      setProfile(data.user);
      event.target.reset();

      const verifiedProfile = await request("/api/users/me", {
        token: data.accessToken,
      });
      if (verifiedProfile) setProfile(verifiedProfile);
    }
  }

  async function loadProfile() {
    const data = await request("/api/users/me");
    if (data) {
      setProfile(data);
      setTarget(String(data.id));
      setDisplayName(data.userName);
    }
  }

  async function updateProfile(event) {
    event.preventDefault();
    const result = await request(`/api/users/${target}`, {
      method: "PUT",
      body: { userName: displayName },
    });
    if (result && Number(target) === profile?.id)
      setProfile({ ...profile, userName: displayName });
  }

  const blockedOrigin = location.origin !== "http://localhost:5173";

  return (
    <div className="shell">
      <header>
        <a className="brand" href="/" aria-label="AutoFix home">
          <span className="brand-mark">
            a<span>↗</span>
          </span>
          autofix<span className="brand-dot">.</span>
        </a>
        <span className="header-caption">A little care. A longer journey.</span>
        <span className="workspace">
          CUSTOMER WORKSPACE <span className="dot" />
        </span>
      </header>
      <main>
        <section className="intro">
          <div>
            <p className="eyebrow">YOUR GARAGE, CONNECTED</p>
            <h1>
              Good to have
              <br />
              you <em>back.</em>
            </h1>
            <p className="intro-copy">
              One place for your account and the road ahead.
              <br />
              Sign in, make yourself at home.
            </p>
          </div>
          <div className="route-art" aria-hidden="true">
            <div className="route-line" />
            <span className="route-start">01</span>
            <span className="route-end">↗</span>
            <span className="route-label">BUILT ON TRUST</span>
          </div>
        </section>
        <div className="content-grid">
          <section className="card account-card">
            <div className="card-top">
              <span className="eyebrow">01 / YOUR ACCOUNT</span>
              <span className={`pill ${session ? "green" : ""}`}>
                {session ? "Signed in" : "Guest"}
              </span>
            </div>
            {session ? (
              <>
                <h2>Hello, {profile?.userName || session.user.userName}.</h2>
                <p className="muted">
                  Your session is ready. Take a look at your profile.
                </p>
                <div className="identity">
                  <div className="avatar">
                    {(profile?.userName || session.user.userName)
                      .slice(0, 1)
                      .toUpperCase()}
                  </div>
                  <div>
                    <strong>{profile?.login || session.user.login}</strong>
                    <p>
                      Account #{session.user.id} · {session.user.role.name}
                    </p>
                  </div>
                </div>
                <dl>
                  <div>
                    <dt>Access expires</dt>
                    <dd>{new Date(session.expiresAt).toLocaleTimeString()}</dd>
                  </div>
                  <div>
                    <dt>Token storage</dt>
                    <dd>This browser tab</dd>
                  </div>
                </dl>
                <button
                  disabled={busy}
                  className="primary"
                  onClick={loadProfile}
                >
                  Load my profile <span>↗</span>
                </button>
                <button
                  className="text-button"
                  onClick={() => {
                    saveSession(null);
                    setProfile(null);
                    setNotice(null);
                  }}
                >
                  Sign out
                </button>
              </>
            ) : (
              <>
                <div className="tabs" role="group" aria-label="Account action">
                  <button
                    className={mode === "login" ? "selected" : ""}
                    onClick={() => setMode("login")}
                  >
                    Sign in
                  </button>
                  <button
                    className={mode === "register" ? "selected" : ""}
                    onClick={() => setMode("register")}
                  >
                    Create account
                  </button>
                </div>
                <h2>{mode === "login" ? "Welcome back." : "Make it yours."}</h2>
                <p className="muted">
                  {mode === "login"
                    ? "Your next chapter starts here."
                    : "A new account starts with the User role."}
                </p>
                <form onSubmit={authenticate} key={mode}>
                  {mode === "register" && (
                    <label>
                      Your name
                      <input
                        name="userName"
                        required
                        maxLength={100}
                        autoComplete="name"
                        placeholder="Alex Morgan"
                      />
                    </label>
                  )}
                  <label>
                    Login
                    <input
                      name="login"
                      required
                      maxLength={100}
                      pattern="[a-zA-Z0-9._-]+"
                      autoComplete="username"
                      placeholder="alex.morgan"
                    />
                  </label>
                  <label>
                    Password
                    <input
                      name="password"
                      type="password"
                      required
                      minLength={8}
                      maxLength={128}
                      autoComplete={
                        mode === "login" ? "current-password" : "new-password"
                      }
                      placeholder="At least 8 characters"
                    />
                  </label>
                  <button className="primary" disabled={busy}>
                    {busy
                      ? "Connecting…"
                      : mode === "login"
                        ? "Sign in to AutoFix"
                        : "Create my account"}
                    <span>↗</span>
                  </button>
                </form>
                <p className="small-note">
                  Your password is hashed on the server.
                  <br />
                  Your access token stays in this browser tab.
                </p>
              </>
            )}
          </section>
          <section className="card access-card">
            <div className="card-top">
              <span className="eyebrow">02 / ACCESS CHECK</span>
              <span className="small-arrow">↗</span>
            </div>
            <h2>A place of your own.</h2>
            <p className="muted">
              Update your profile, or see how access changes when a resource
              belongs to someone else.
            </p>
            <form onSubmit={updateProfile}>
              <div className="field-row">
                <label>
                  Profile ID
                  <input
                    type="number"
                    min="1"
                    max="2147483647"
                    required
                    value={target}
                    onChange={(e) => setTarget(e.target.value)}
                    placeholder="e.g. 4"
                  />
                </label>
                <label>
                  Display name
                  <input
                    required
                    maxLength={100}
                    value={displayName}
                    onChange={(e) => setDisplayName(e.target.value)}
                    placeholder="Your name"
                  />
                </label>
              </div>
              <button className="secondary" disabled={busy || !session}>
                Save profile <span>↗</span>
              </button>
            </form>
            <div className="divider" />
            <p className="eyebrow">TRY THE BOUNDARIES</p>
            <div className="check-actions">
              <button
                disabled={busy}
                onClick={() => request("/api/users/me", { token: null })}
              >
                <span>Without a token</span>
                <small>Expect 401</small>
              </button>
              <button
                disabled={busy || !session}
                onClick={() => request("/api/users")}
              >
                <span>Administrator area</span>
                <small>User → 403 · Admin → 200</small>
              </button>
              <button
                disabled={busy}
                onClick={() =>
                  request("/api/auth/login", {
                    method: "POST",
                    token: null,
                    body: {
                      login: "rate.limit.probe",
                      password: "deliberately-wrong-password",
                    },
                  })
                }
              >
                <span>Try a failed sign-in</span>
                <small>5 attempts / minute, then 429</small>
              </button>
            </div>
            <p className="small-note">
              To check ownership, enter another existing account’s ID above.
              Users can edit their own profile; administrators can edit any
              profile.
            </p>
          </section>
        </div>
        <section className="connection">
          <div>
            <span className={`dot ${blockedOrigin ? "amber" : ""}`} />
            <strong>
              {blockedOrigin
                ? "Unapproved browser origin"
                : "A direct connection"}
            </strong>
            <p>
              {location.origin} <span>→</span> {api}
            </p>
          </div>
          <div className="connection-note">
            {blockedOrigin ? (
              "Requests should be blocked by CORS. The browser console gives the exact cause."
            ) : (
              <>
                No proxy in between.{" "}
                <a
                  href="http://localhost:5174"
                  target="_blank"
                  rel="noreferrer"
                >
                  Open the CORS-denied client ↗
                </a>
                <small>Start it with npm run dev:blocked.</small>
              </>
            )}
          </div>
        </section>
        {notice && (
          <div
            className={`notice ${notice.ok ? "success" : "failure"}`}
            role="status"
            aria-live="polite"
          >
            <strong>{notice.status}</strong>
            <span>{notice.detail}</span>
          </div>
        )}
        <section className="activity">
          <div className="activity-title">
            <h3>Request journal</h3>
            <span>REAL RESPONSES, AS THEY HAPPEN</span>
          </div>
          {history.length === 0 ? (
            <p className="empty">
              A quiet start. Your requests will appear here.
            </p>
          ) : (
            history.map((entry, index) => (
              <details key={`${entry.time}-${index}`}>
                <summary>
                  <time>{entry.time}</time>
                  <span className="method">{entry.method}</span>
                  <code>{entry.path}</code>
                  <span
                    className={`status ${Number(entry.status) < 300 ? "ok" : ""}`}
                  >
                    {entry.status}
                  </span>
                </summary>
                <pre>
                  {JSON.stringify(
                    entry.data &&
                      typeof entry.data === "object" &&
                      "accessToken" in entry.data
                      ? { ...entry.data, accessToken: "[hidden]" }
                      : entry.data,
                    null,
                    2,
                  )}
                </pre>
              </details>
            ))
          )}
        </section>
      </main>
      <footer>
        <span>AutoFix / Made for the miles ahead.</span>
        <span>ACCOUNT & ACCESS · PRACTICAL 11</span>
      </footer>
    </div>
  );
}

createRoot(document.getElementById("root")).render(<App />);
