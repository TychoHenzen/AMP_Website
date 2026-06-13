# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Is

Personal portfolio website for Tycho Henzen. Blazor WebAssembly (.NET 6.0), deployed to Azure Static Web Apps.

Live: https://icy-meadow-06fe80803.3.azurestaticapps.net

## Build & Run

```bash
# All commands from AutoARPG_WebAsm/ directory
dotnet restore
dotnet build
dotnet run              # serves at https://localhost:7294 or http://localhost:5111
dotnet publish -c Release
```

No test project exists. No linter configured.

## Architecture

Single-project Blazor WASM app. Solution file: `AutoARPG_WebAsm.sln`, project: `AutoARPG_WebAsm/AutoARPG_WebAsm.csproj`.

**Entry point**: `Program.cs` — registers `HttpClient`, `ProjectService` (scoped), and `Blazored.SessionStorage`. Root component is `App.razor` which runs the Blazor router.

**Data flow**: `ProjectService` fetches `wwwroot/Projects/projects.json` via HTTP GET → deserializes to `List<ProjectInfo>`. All models live in `Data/ProjectService.cs` (service + models in one file): `ProjectInfo`, `MediaItem`, `MediaType` enum, `ImageFitType` enum.

**Pages** (5 routes in `Pages/`): Index (`/`), Projects (`/projects`), Skills (`/skills`), Experience (`/experience`), Contact (`/contact`). Projects page is the most complex — has search, tag filtering, category grouping, and media display (images/videos/PDFs).

**Layout**: `Shared/MainLayout.razor` (top navbar + footer) + `Shared/NavMenu.razor` (horizontal nav with Font Awesome icons, hamburger on mobile). Dark modern theme — `#0a0a0a` body, `#1a1a2e` navbar, `#60a5fa` accent. Global styles in `wwwroot/css/app.css` with Bootstrap dark overrides. Inter font (Google Fonts CDN) for body, monospace for terminal elements.

**Design patterns**: Terminal-style sections (`.terminal-section`, `.terminal-header`, `.terminal-body`) used on Index hero, Skills page, and Projects page. Experience page uses vertical timeline (`.timeline`, `.timeline-item`). Index has JS-interop typing animation (`OnAfterRenderAsync`).

**State**: Component-local only. No global state store. `SessionStorage` available via Blazored but used sparingly.

## Key Dependencies

- `Blazored.SessionStorage` 2.3.0 — browser session storage
- `Newtonsoft.Json` 13.0.3 — JSON handling
- `SixLabors.ImageSharp` 3.0.1 — image processing

## Deployment

Push to `master` triggers GitHub Actions → Azure Static Web Apps deploy. Workflow: `.github/workflows/azure-static-web-apps-icy-meadow-06fe80803.yml`.

SPA routing configured in `staticwebapp.config.json` — all routes rewrite to `/index.html` except images and CSS.

## ProjectEditor (Local Portfolio Manager)

Second project in the solution. .NET 8 Blazor Server app for editing portfolio data locally. **Not deployed** — local-only, no auth, outside Azure SWA build path.

```bash
# From ProjectEditor/ directory
dotnet run    # serves at https://localhost:7xxx
```

**What it does**: reads/writes `AutoARPG_WebAsm/wwwroot/Projects/projects.json` directly via `ProjectDataService`. Features: project list with reorder (↑/↓) and delete, create/edit form with all fields, file upload (auto-routes by extension → `Projects/images/|videos/|pdfs/`).

**Path resolution**: `Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../AutoARPG_WebAsm/wwwroot/Projects/projects.json"))` with a `Directory.GetCurrentDirectory()` fallback for `dotnet run`.

**Models**: `ProjectEditor/Models.cs` duplicates `AutoARPG_WebAsm/Models.cs` as POCOs (no project reference). Keep in sync manually when model changes are needed.

**Pages**: `ProjectEditor/Pages/ProjectList.razor` (`/`), `ProjectEditor/Pages/ProjectEdit.razor` (`/edit/new`, `/edit/{Index:int}`). Note: pages live in `Pages/` (not `Components/Pages/`) — avoid creating Blazor-scaffolded pages with `@page "/"` in `Components/Pages/` or you'll get `AmbiguousMatchException`.

## Project Data

To add/edit portfolio projects, modify `wwwroot/Projects/projects.json`. Each entry has: `finished`, `title`, `description`, `fullDescription`, `mediaItems` (array of `{url, name, type}`), `sourceUrl`, `tags`, `category`, `imageFit` (Cover/Contain).

## Conventions

- Nullable reference types enabled, implicit usings enabled
- Models and service colocated in `Data/ProjectService.cs`
- Static assets organized by category under `wwwroot/` (Games, Shaders, Logos, Technical, etc.)
- DI via `@inject` in Razor components
