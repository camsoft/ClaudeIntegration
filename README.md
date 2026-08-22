# ClaudeIntegration

A sample ASP.NET Core + React (Vite) application that demonstrates integrating with the Anthropic Claude API.

## Projects

- **ClaudeIntegration.Server** — ASP.NET Core Web API (.NET 10) backend. Hosts the API controllers, Swagger UI, and the Claude integration service.
- **claudeintegration.client** — React + TypeScript frontend (Vite), served via the ASP.NET Core SPA proxy during development.

## Prerequisites

- Visual Studio 2026 (or later) with the ASP.NET and web development workload
- Node.js (for the client project's `npm` dependencies)
- An [Anthropic API key](https://console.anthropic.com/) (active, with available credits)

## First-time setup

### 1. Open the solution

Open `ClaudeIntegration.slnx` in Visual Studio. `ClaudeIntegration.Server` is configured as the default startup project, and the `https` launch profile is selected by default.

### 2. Configure your Anthropic API key

The Claude integration (`ClaudeService`) requires an API key to call the Anthropic API. **Do not put your key directly in source code** — use one of these options instead:

**Option A — .NET User Secrets (recommended)**

In Visual Studio, right-click the `ClaudeIntegration.Server` project → **Manage User Secrets**. This opens `secrets.json` for editing — add:

```json
{
  "Anthropic:ApiKey": "your-api-key-here"
}
```

Or from a terminal in the `ClaudeIntegration.Server` folder:

```powershell
dotnet user-secrets set "Anthropic:ApiKey" "your-api-key-here"
```

This stores the key outside the repository (per-user, per-machine), so it's never committed to source control.

**Option B — Environment variable**

Alternatively, set the `ANTHROPIC_API_KEY` environment variable (used as a fallback if the User Secret isn't set):

```powershell
[System.Environment]::SetEnvironmentVariable("ANTHROPIC_API_KEY", "your-api-key-here", "User")
```

Restart Visual Studio after setting this so it picks up the new environment variable.

### 3. Run the app

Press **F5** (or Ctrl+F5) in Visual Studio. This will:

- Start the ASP.NET Core backend (`https://localhost:7101`)
- Automatically launch the Vite dev server for the React frontend via the SPA proxy
- Open a browser tab to the running app
- Open a second browser tab to Swagger UI (`https://localhost:7101/swagger`) automatically, so you can explore/test the API directly

No manual URL entry is required — just press F5 and both tabs should open on their own.

## Claude integration notes

`ClaudeService` enables Claude's built-in web search tool (`WebSearchTool20250305`) with `MaxUses = 5`. This limits Claude to at most 5 web searches **per request**, not per conversation or per day.

> **Cost warning:** Each web search performed by Claude is billed by Anthropic. If you remove or increase `MaxUses`, a single request could trigger many more searches than expected, which can noticeably increase API costs. Leave this cap in place (or lower it) while experimenting, especially in a classroom setting where many students may be running requests simultaneously.

## Troubleshooting

- **404 / blank page on launch**: Make sure the active launch profile (toolbar dropdown next to the Run button) is set to **https**, not "IIS Express".
- **502 Bad Gateway / connection refused**: Usually means the backend isn't running, or another process is already bound to ports `7101`, `5246`, or `50666`. Close any stray `dotnet` or `node`/`vite` processes and re-run.
- **"Anthropic API key not configured" error**: Set the key using Option A or B above, then restart the app.
