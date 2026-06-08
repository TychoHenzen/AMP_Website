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

**Layout**: `Shared/MainLayout.razor` + `Shared/NavMenu.razor`. Component-scoped CSS via `.razor.css` files.

**State**: Component-local only. No global state store. `SessionStorage` available via Blazored but used sparingly.

## Key Dependencies

- `Blazored.SessionStorage` 2.3.0 — browser session storage
- `Newtonsoft.Json` 13.0.3 — JSON handling
- `SixLabors.ImageSharp` 3.0.1 — image processing

## Deployment

Push to `master` triggers GitHub Actions → Azure Static Web Apps deploy. Workflow: `.github/workflows/azure-static-web-apps-icy-meadow-06fe80803.yml`.

SPA routing configured in `staticwebapp.config.json` — all routes rewrite to `/index.html` except images and CSS.

## Project Data

To add/edit portfolio projects, modify `wwwroot/Projects/projects.json`. Each entry has: `finished`, `title`, `description`, `fullDescription`, `mediaItems` (array of `{url, name, type}`), `sourceUrl`, `tags`, `category`, `imageFit` (Cover/Contain).

## Conventions

- Nullable reference types enabled, implicit usings enabled
- Models and service colocated in `Data/ProjectService.cs`
- Static assets organized by category under `wwwroot/` (Games, Shaders, Logos, Technical, etc.)
- DI via `@inject` in Razor components
