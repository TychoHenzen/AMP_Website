# ProjectEditor — Local Blazor Server Portfolio Manager — Requirements Spec

> **For Claude (/goal):** Work through each incomplete step below.
> 1. Mark a step `[>]` when you begin working on it.
> 2. Call `dod_check` to verify proofs — do NOT mark proofs manually.
> 3. A step is complete when ALL its proofs pass via `dod_check`.
> 4. If a proof cannot be met, use `dod_amend` to modify it with a reason.
> 5. Continue until `dod_check` returns PASS — then stop and report done.
>
> **Self-contained.** All commands run from `C:\Users\siriu\RiderProjects\AMP_Website` unless noted.
>
> **🔒 Anti-cheat:** Proofs are stored canonically in MCP storage (dod-guard).
> `dod_check` executes commands from the canonical copy, not this markdown file.
> Editing proof text here has no effect on verification.
> Store tampering is **logged and detectable** — each check prints a proof-set fingerprint.

**Goal:** Build a local Blazor Server web app that lets you create, edit, delete, reorder, and add media to portfolio projects by reading/writing projects.json directly on disk.

**Date:** 2026-06-12
**Target:** `C:\Users\siriu\RiderProjects\AMP_Website`
**DoD ID:** `4aaf5cee-a908-4394-bc4e-55a1516cc26d`
**Last check:** PASS (2026-06-13T10:37:07.743Z)

---

## Decisions (locked with user)

- Local-only (no Azure backend)
- Blazor Server (.NET 8) — can do direct file I/O unlike WASM
- New project added to existing `AutoARPG_WebAsm.sln`
- Models duplicated as POCOs (no cross-project reference from Server→WASM)
- Hardcoded relative path: `../AutoARPG_WebAsm/wwwroot/Projects/projects.json`
- Media auto-routed by extension: images→`Projects/images/`, videos→`Projects/videos/`, PDFs→`Projects/pdfs/`
- Deploy via manual `git push` — editor just saves files

## Current state

- Pure static Blazor WASM site, no backend
- `projects.json` has 38 entries, edited manually
- `Api/` folder in workflow config exists but is empty
- Models defined in `AutoARPG_WebAsm/Models.cs`: `ProjectInfo`, `MediaItem`, `MediaType` enum, `ImageFitType` enum

## Requirements

## Functional Requirements

1. **Project List** — home page shows all projects (title, category, finished status), with up/down reorder buttons, Edit button, Delete button (with confirmation), and a New Project button.
2. **Create/Edit Form** — form covering all `ProjectInfo` fields:
   - Title (required), Description, FullDescription
   - Tags (comma-separated chip input)
   - Category, SourceUrl
   - ImageFit (Cover/Contain toggle)
   - Finished (checkbox)
   - MediaItems sub-list: add/remove entries (Url, Name, Type)
3. **Media Upload** — file picker uploads image/video/PDF; auto-detects type by extension; copies file to `../AutoARPG_WebAsm/wwwroot/Projects/{images|videos|pdfs}/filename`; auto-populates the `Url` field with the relative path used in projects.json.
4. **Persistence** — every save writes the full `projects.json` atomically (write to temp file, then replace); a failed write shows an error toast and leaves the existing file intact.
5. **Path** — `ProjectDataService` resolves projects.json relative to the running process: `Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../AutoARPG_WebAsm/wwwroot/Projects/projects.json"))` with a `dotnet run`-aware fallback.

## Non-Functional
- .NET 8 Blazor Server
- Project added to `AutoARPG_WebAsm.sln`
- No authentication (local only)
- No Azure infrastructure changes
- Azure SWA workflow unaffected (`app_location: AutoARPG_WebAsm` — editor not in build path)

## Research Notes

- `AutoARPG_WebAsm/Models.cs` — ProjectInfo, MediaItem, MediaType (Image=0, Video=1, Pdf=2), ImageFitType (Cover=0, Contain=1)
- `Projects.razor` line 264 — loads via `Http.GetFromJsonAsync<List<ProjectInfo>>("Projects/projects.json")`; no service abstraction
- `wwwroot/Projects/` structure: `projects.json`, `images/{Logos,Games,Shaders,Technical,Work,Evolution,Manim,Analyses,StableDiffusion,WebsiteParts}/`, `videos/Manim/`, `pdfs/`
- `.github/workflows/azure-static-web-apps-icy-meadow-06fe80803.yml` — `app_location: AutoARPG_WebAsm`, `api_location: AutoARPG_WebAsm/Api` (empty) — editor project outside this path is safe
- `AutoARPG_WebAsm.sln` at repo root — add editor project there

## Open Questions

- Exact runtime path resolution strategy needs a smoke test on first run — may need a config override if the relative path doesn't resolve correctly
- Media subfolder within `images/` (e.g. `images/Games/`) not auto-categorized — files land in flat `images/` for now; user can move manually

---

## Definition of Done

### Step 1: Scaffold ProjectEditor Blazor Server project and add to solution [x]

- [x] Proof: `test -f ProjectEditor/ProjectEditor.csproj` → ProjectEditor.csproj exists
- [x] Proof: `grep -i "net8.0" ProjectEditor/ProjectEditor.csproj` → targets .NET 8
- [x] Proof: `grep "ProjectEditor" AutoARPG_WebAsm.sln` → solution file references ProjectEditor

### Step 2: Add ProjectInfo models as copied POCOs in ProjectEditor [x]

- [x] Proof: `test -f ProjectEditor/Models.cs` → Models.cs exists in ProjectEditor
- [x] Proof: `grep "class ProjectInfo" ProjectEditor/Models.cs` → ProjectInfo class defined
- [x] Proof: `grep "enum MediaType" ProjectEditor/Models.cs` → MediaType enum defined
- [x] Proof: `grep "enum ImageFitType" ProjectEditor/Models.cs` → ImageFitType enum defined
- [x] Proof: `grep "class MediaItem" ProjectEditor/Models.cs` → MediaItem class defined

### Step 3: Implement ProjectDataService — read, write, and reorder projects.json [x]

- [x] Proof: `test -f ProjectEditor/Services/ProjectDataService.cs` → ProjectDataService.cs exists
- [x] Proof: `grep "LoadProjects\|GetProjects" ProjectEditor/Services/ProjectDataService.cs` → load method exists
- [x] Proof: `grep "SaveProjects\|WriteProjects" ProjectEditor/Services/ProjectDataService.cs` → save method exists
- [x] Proof: `grep -i "MoveUp\|MoveDown\|Reorder\|swap" ProjectEditor/Services/ProjectDataService.cs` → reorder logic exists
- [x] Proof: `grep "AutoARPG_WebAsm" ProjectEditor/Services/ProjectDataService.cs` → hardcoded path references AutoARPG_WebAsm
- [x] Proof: `grep -i "temp\|tmp\|\.tmp\|replace\|move" ProjectEditor/Services/ProjectDataService.cs` → atomic write (temp file + replace) implemented

### Step 4: Project list page — list all projects with reorder and delete [x]

- [x] Proof: `test -f "ProjectEditor/Pages/ProjectList.razor"` → ProjectList.razor exists
- [x] Proof: `grep -i "MoveUp\|MoveDown\|reorder\|↑\|↓" "ProjectEditor/Pages/ProjectList.razor"` → reorder buttons present
- [x] Proof: `grep -i "delete\|remove" "ProjectEditor/Pages/ProjectList.razor"` → delete action present
- [x] Proof: `grep -i "new project\|add project\|create" "ProjectEditor/Pages/ProjectList.razor"` → new project button present
- [x] Proof: `grep -i "confirm\|are you sure\|onclick" "ProjectEditor/Pages/ProjectList.razor"` → delete confirmation present

### Step 5: Project create/edit form — all ProjectInfo fields + MediaItems sub-list [x]

- [x] Proof: `test -f "ProjectEditor/Pages/ProjectEdit.razor"` → ProjectEdit.razor exists
- [x] Proof: `grep -i "FullDescription\|fulldescription" "ProjectEditor/Pages/ProjectEdit.razor"` → FullDescription field present
- [x] Proof: `grep -i "tags" "ProjectEditor/Pages/ProjectEdit.razor"` → Tags field present
- [x] Proof: `grep -i "Category" "ProjectEditor/Pages/ProjectEdit.razor"` → Category field present
- [x] Proof: `grep -i "ImageFit\|imagefit" "ProjectEditor/Pages/ProjectEdit.razor"` → ImageFit toggle present
- [x] Proof: `grep -i "Finished\|finished" "ProjectEditor/Pages/ProjectEdit.razor"` → Finished checkbox present
- [x] Proof: `grep -i "MediaItem\|mediaitem" "ProjectEditor/Pages/ProjectEdit.razor"` → MediaItems sub-list present
- [x] Proof: `grep -i "SourceUrl\|sourceurl" "ProjectEditor/Pages/ProjectEdit.razor"` → SourceUrl field present

### Step 6: Media upload — auto-route by file type, copy to wwwroot, populate URL field [x]

- [x] Proof: `grep -ri "InputFile\|IBrowserFile" ProjectEditor/` → Blazor InputFile component used for file upload
- [x] Proof: `grep -ri "images/\|videos/\|pdfs/" ProjectEditor/` → target subfolder paths referenced in upload logic
- [x] Proof: `grep -ri "\.jpg\|\.png\|\.mp4\|\.pdf\|\.webm" ProjectEditor/` → file extension detection for type routing
- [x] Proof: `grep -ri "File.Copy\|CopyToAsync\|WriteAsync\|FileStream" ProjectEditor/` → file copy/write to disk implemented

### Step 7: Full build passes — dotnet build clean with no errors [x]

- [x] Proof: `dotnet build ProjectEditor/ProjectEditor.csproj 2>&1` → dotnet build exits with Build succeeded
- [x] Proof: `dotnet build ProjectEditor/ProjectEditor.csproj 2>&1` → no build errors

## Amendment log

- **2026-06-13T10:37:00.052Z** [step-7/proof-7-2] modified: The original predicate output_not_contains "error" fires a false positive: dotnet build always prints "0 Error(s)" in its summary line even on a clean build, causing the check to fail despite exit code 0 and "Build succeeded." message. Changing to output_not_contains "Build FAILED" which is the actual string dotnet emits on a real build failure.
