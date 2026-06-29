# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Is

A **monorepo** hosting multiple small websites, each deployed to its own Azure Static Web Apps instance. Shared repo for tooling/conventions; isolated hosting per site (one SWA = one custom domain = one deploy).

```
AMP_Website/
├─ sites/
│  ├─ portfolio/      Tycho's portfolio. Blazor WASM (.NET 6). → SWA "icy-meadow" (live)
│  └─ nido-suave/     Denise's massage business (Dutch). Blazor WASM (.NET 8). → SWA TBD
├─ tools/
│  └─ ProjectEditor/  Local-only editor for portfolio data (.NET 8 Blazor Server). Not deployed.
├─ .github/workflows/
│  ├─ deploy-portfolio.yml   path filter: sites/portfolio/**
│  └─ deploy-nido.yml        path filter: sites/nido-suave/**
└─ AutoARPG_WebAsm.sln       solution covering all projects
```

Path-filtered workflows mean editing one site only redeploys that site.

**Adding a new site**: scaffold under `sites/<name>/`, add a `staticwebapp.config.json` to its `wwwroot/`, add to the `.sln`, create a path-filtered `deploy-<name>.yml`, then create the Azure SWA instance + add its deploy token as a GitHub secret.

## sites/portfolio — Tycho's portfolio

Blazor WebAssembly (.NET 6.0). Live: https://icy-meadow-06fe80803.3.azurestaticapps.net

```bash
# From sites/portfolio/
dotnet restore && dotnet build
dotnet run              # https://localhost:7294 or http://localhost:5111
dotnet publish -c Release
```

No test project. No linter.

**Project**: `sites/portfolio/AutoARPG_WebAsm.csproj` (project/assembly name still `AutoARPG_WebAsm`; only the folder was renamed to `portfolio`).

**Entry point**: `Program.cs` — registers `HttpClient`, `ProjectService` (scoped), `Blazored.SessionStorage`. Root component `App.razor` runs the Blazor router.

**Data flow**: `ProjectService` fetches `wwwroot/Projects/projects.json` via HTTP GET → `List<ProjectInfo>`. Models in `Models.cs`: `ProjectInfo`, `MediaItem`, `MediaType` enum, `ImageFitType` enum.

**Pages** (5 routes in `Pages/`): Index (`/`), Projects (`/projects`), Skills (`/skills`), Experience (`/experience`), Contact (`/contact`). Projects page is the most complex — search, tag filtering, category grouping, media display (images/videos/PDFs).

**Layout**: `Shared/MainLayout.razor` + `Shared/NavMenu.razor`. Dark modern theme — `#0a0a0a` body, `#1a1a2e` navbar, `#60a5fa` accent. Global styles in `wwwroot/css/app.css`. Inter font, monospace for terminal elements. Terminal-style sections (`.terminal-*`); Experience uses a vertical timeline; Index has a JS-interop typing animation.

**Key deps**: `Blazored.SessionStorage` 2.3.0, `Newtonsoft.Json` 13.0.3, `SixLabors.ImageSharp` 3.0.1.

**Project data**: edit `sites/portfolio/wwwroot/Projects/projects.json`. Each entry: `finished`, `title`, `description`, `fullDescription`, `mediaItems` (`{url, name, type}`), `sourceUrl`, `tags`, `category`, `imageFit` (Cover/Contain).

## sites/nido-suave — Denise's massage business

Blazor WebAssembly (.NET 8). Dutch-language. Not yet deployed (Azure SWA + secret pending).

```bash
# From sites/nido-suave/
dotnet run
```

**Theme**: warm/soft — cream `#faf6f1`, terracotta `#c08552`, sage `#8a9a7b`. Cormorant Garamond (serif headings) + Nunito Sans (body), Google Fonts CDN. Global styles in `wwwroot/css/app.css`; scoped layout CSS deliberately emptied.

**Layout**: `Layout/MainLayout.razor` (sticky header + footer) + `Layout/NavMenu.razor` (responsive nav, hamburger on mobile). Note: .NET 8 template uses `Layout/`, not `Shared/`.

**Pages** (`Pages/`): Home (`/`), Over (`/over` — full "Over Nido Suave" about story), Behandelingen (`/behandelingen` — placeholder), Contact (`/contact` — placeholder contact details).

## tools/ProjectEditor — Local Portfolio Manager

.NET 8 Blazor Server app for editing portfolio data locally. **Not deployed** — local-only, no auth, outside any SWA build path.

```bash
# From tools/ProjectEditor/
dotnet run    # https://localhost:7xxx
```

**What it does**: reads/writes `sites/portfolio/wwwroot/Projects/projects.json` via `ProjectDataService`. Project list with reorder (↑/↓) and delete, create/edit form, file upload (auto-routes by extension → `Projects/images/|videos/|pdfs/`).

**Path resolution** (`Services/ProjectDataService.cs`): `AppContext.BaseDirectory` + `../../../../../sites/portfolio/wwwroot/Projects/projects.json` (5 levels up from `bin/Debug/net8.0/`), with a `Directory.GetCurrentDirectory()` + `../../sites/portfolio/...` fallback for `dotnet run`. **If the folder layout changes, update these relative paths.**

**Models**: `tools/ProjectEditor/Models.cs` duplicates `sites/portfolio/Models.cs` as POCOs (no project reference). Keep in sync manually.

**Pages**: live in `Pages/` (not `Components/Pages/`) — `ProjectList.razor` (`/`), `ProjectEdit.razor` (`/edit/new`, `/edit/{Index:int}`). Avoid scaffolding `@page "/"` pages in `Components/Pages/` or you'll get `AmbiguousMatchException`.

## Deployment

Each site has its own path-filtered GitHub Actions workflow → its own Azure SWA. Pushing to `master` deploys only the site(s) whose files changed. Each site's `wwwroot/staticwebapp.config.json` handles SPA routing (all routes → `/index.html` except static assets).

## Conventions

- Nullable reference types + implicit usings enabled
- Static assets organized by category under each site's `wwwroot/`
- DI via `@inject` in Razor components
- Keep each site self-contained; no shared Razor library yet (add `shared/` when real duplication appears)
