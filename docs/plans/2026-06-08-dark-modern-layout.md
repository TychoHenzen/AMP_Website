# Dark Modern Portfolio Layout Rework - Requirements Spec

<claude_instructions>
**For the implementer:** Work through each task below.
1. Mark a task `[>]` when you begin working on it.
2. Call `dod_check` to verify proofs - do NOT mark proofs manually.
3. A task group is complete when ALL its concrete proofs pass via `dod_check`.
4. Use `dod_refine` to turn a draft leaf into a concrete proof or subdivide into child tasks.
5. If a proof cannot be met, use `dod_amend` to modify it with a reason.
6. Continue until `dod_check` returns PASS - then stop and report done.

**Behavioral predicates only.** Each proof is a concrete behavioral claim.
Read failure diagnoses carefully - they tell you WHAT went wrong and what to fix.
Proofs run on the HOST OS - write OS-correct commands (no bash on Windows).

**CWD:** `C:\Users\siriu\RiderProjects\AMP_Website\AutoARPG_WebAsm`

**Anti-cheat:** Proofs stored canonically in MCP storage.
`dod_check` executes commands from the canonical copy, not this markdown file.
</claude_instructions>

**Goal:** Rework the portfolio website from default Blazor template to a dark modern professional design with top navbar, terminal-style hero, and consistent dark theme across all 5 pages.

**Date:** 2026-06-08
**Target:** `C:\Users\siriu\RiderProjects\AMP_Website\AutoARPG_WebAsm`
**DoD ID:** `99815894-d107-4b7f-ba7a-7f2487ce83a0`
**Last check:** PASS (2026-06-08T15:55:56.120Z)

---

## Decisions (locked with user)

<decisions>
- Visual direction: Dark modern (GitHub dark / Vercel style)
- Navigation: Top navbar replacing left sidebar
- Hero: Terminal-style with typing animation (JS interop)
- Home page: Focused landing with CTAs, not content summary
- Footer: Social icons + copyright
- Color: Dark backgrounds (#0a0a0a / #1a1a2e), blue accents (#60a5fa / #3b82f6)
- Typography: Inter (Google Fonts) + monospace for terminal elements
- Skills: Terminal-style grid visualization
- Experience: Vertical timeline with dots/line
- Scope: All 5 pages + layout + global CSS
- Bootstrap stays, restyled via overrides
- Font Awesome replaces Open Iconic for nav icons
</decisions>

## Current state

<current_state>
- Default Blazor WASM template with left sidebar (purple/blue gradient)
- Top bar with GitHub/LinkedIn links
- Standard Bootstrap card-based content
- Simple footer with copyright
- 5 pages: Index, Projects, Skills, Experience, Contact
- No scoped CSS files for content pages (all inline or app.css)
- Skills page uses Bootstrap progress bars
- Experience page uses plain Bootstrap cards
- Contact page has EmailJS integration (must preserve)
- Projects page has search/filter/modal (must preserve logic)
</current_state>

## Requirements

<requirements>
## Layout & Navigation
- Replace left sidebar with sticky top navbar (dark bg, blue accent active links)
- Brand/name left, nav links right, hamburger on mobile
- Full-width content area below navbar
- Dark footer with social icon links + copyright 2026
- Remove Open Iconic, use Font Awesome consistently

## Typography & Colors
- Inter font from Google Fonts CDN for body
- Monospace (system or JetBrains Mono) for terminal elements
- Dark backgrounds: #0a0a0a (body), #111827 (cards/surfaces), #1a1a2e (navbar)
- Blue accents: #60a5fa (links/highlights), #3b82f6 (buttons/active)
- Text: #e5e7eb (body), #9ca3af (muted), #f9fafb (headings)

## Home Page (Index.razor)
- Terminal-style hero with typing animation via JS interop
- Brief bio sentence below terminal
- CTA links/buttons to Projects, Skills, Experience, Contact
- Remove current 3-column Skills/Experience/Education grid

## Skills Page
- Terminal-style skill visualization (block chars + percentage)
- Soft skills and hobbies as dark-themed styled lists
- Grouped by category (Languages, Frameworks, Specialized)

## Experience Page
- Vertical timeline with left-side line and dot nodes
- Dark content cards for each job entry
- Preserve all existing content/bullet points

## Projects Page
- Dark-themed project cards with hover effects
- Search bar and tag filters restyled dark
- Modal restyled with dark theme
- Preserve ALL existing logic (search, filter, carousel, YouTube embed, PDF preview)

## Contact Page
- Dark-themed contact form
- Dark social links section
- Preserve EmailJS integration unchanged

## Constraints
- Blazor WASM .NET 6.0 compatible
- Responsive (mobile + desktop)
- Bootstrap stays (override, don't remove)
- No backend/data model changes
- No new pages or routes
</requirements>

## Research Notes

<research_notes>
### Key Files
- `Shared/MainLayout.razor` + `.razor.css` — main layout wrapper
- `Shared/NavMenu.razor` + `.razor.css` — navigation component
- `wwwroot/css/app.css` — global styles (~250 lines)
- `wwwroot/index.html` — HTML shell, loads Bootstrap + Font Awesome + EmailJS
- `Pages/Index.razor` — home page (3-col summary grid)
- `Pages/Skills.razor` — progress bars, inline styles
- `Pages/Experience.razor` — 3 job cards
- `Pages/Contact.razor` — form + social links + EmailJS JS interop
- `Pages/Projects.razor` — 368 lines, search/filter/modal/carousel

### External Dependencies
- Bootstrap 5 (local CSS in wwwroot/css/bootstrap/)
- Font Awesome 6.5.2 (CDN)
- Open Iconic (imported in app.css — to be removed)
- EmailJS browser SDK (CDN)
- Blazored.SessionStorage, Newtonsoft.Json, SixLabors.ImageSharp

### Terminal Styling Already Exists
- app.css has `.terminal-section`, `.terminal-header`, `.terminal-body`, etc.
- Used on Projects page for incomplete projects section
- Can reuse/extend this pattern for hero and skills page
</research_notes>

---

## Definition of Done

<definition_of_done>

### Step 1: Add Inter font and update index.html with dark theme base [x]

  - [x] Proof: `type wwwroot\index.html | findstr "fonts.googleapis.com"` -> Inter font loaded from Google Fonts CDN <!--p:{"type":"exit_code","value":0}-->
  - [x] Proof: `type wwwroot\css\app.css | findstr "font-family"` -> Inter font set as primary font-family in app.css <!--p:{"type":"output_contains","value":"Inter"}-->

### Step 2: Rework MainLayout — replace sidebar with top navbar + dark footer [x]

  - [x] Proof: `findstr "sidebar" Shared/MainLayout.razor` -> Sidebar div removed from MainLayout <!--p:{"type":"exit_code","value":1}-->
  - [x] Proof: `type Shared\MainLayout.razor | findstr "navbar"` -> Top navbar present in MainLayout <!--p:{"type":"exit_code","value":0}-->
  - [x] Proof: `type Shared\MainLayout.razor | findstr "footer"` -> Footer section present in MainLayout <!--p:{"type":"exit_code","value":0}-->
  - [x] Proof: `type Shared\MainLayout.razor | findstr "2026"` -> Copyright year updated to 2026 <!--p:{"type":"exit_code","value":0}-->

### Step 3: Rework NavMenu — horizontal nav links with Font Awesome icons [x]

  - [x] Proof: `findstr "oi oi-" Shared/NavMenu.razor` -> Open Iconic icon classes removed from NavMenu <!--p:{"type":"exit_code","value":1}-->
  - [x] Proof: `type Shared\NavMenu.razor | findstr "fa-"` -> Font Awesome icons used in NavMenu <!--p:{"type":"exit_code","value":0}-->
  - [x] Proof: `type Shared\NavMenu.razor | findstr "navbar-toggler"` -> Mobile hamburger toggle preserved <!--p:{"type":"exit_code","value":0}-->

### Step 4: Global CSS overhaul — dark theme, Bootstrap overrides, remove Open Iconic [x]

  - [x] Proof: `findstr "open-iconic" wwwroot/css/app.css` -> Open Iconic import removed from app.css <!--p:{"type":"exit_code","value":1}-->
  - [x] Proof: `type wwwroot\css\app.css | findstr "#0a0a0a"` -> Dark background color present in app.css <!--p:{"type":"exit_code","value":0}-->
  - [x] Proof: `type wwwroot\css\app.css | findstr "#60a5fa"` -> Blue accent color present in app.css <!--p:{"type":"exit_code","value":0}-->

### Step 5: Home page — terminal hero with typing animation + focused landing CTAs [x]

  - [x] Proof: `type Pages\Index.razor | findstr "terminal"` -> Terminal-styled hero section present on home page <!--p:{"type":"exit_code","value":0}-->
  - [x] Proof: `type Pages\Index.razor | findstr "typing"` -> Typing animation referenced in home page <!--p:{"type":"exit_code","value":0}-->
  - [x] Proof: `findstr "jumbotron" Pages/Index.razor` -> Old jumbotron layout removed <!--p:{"type":"exit_code","value":1}-->
  - [x] Proof: `findstr "col-md-4" Pages/Index.razor` -> Old 3-column summary grid removed <!--p:{"type":"exit_code","value":1}-->

### Step 6: Skills page — terminal-style grid visualization [x]

  - [x] Proof: `findstr "progress-bar" Pages/Skills.razor` -> Bootstrap progress bars removed from Skills page <!--p:{"type":"exit_code","value":1}-->
  - [x] Proof: `type Pages\Skills.razor | findstr "terminal"` -> Terminal-style elements present on Skills page <!--p:{"type":"exit_code","value":0}-->

### Step 7: Experience page — vertical timeline with dark cards [x]

  - [x] Proof: `type Pages\Experience.razor | findstr "timeline"` -> Timeline structure present on Experience page <!--p:{"type":"exit_code","value":0}-->
  - [x] Proof: `type Pages\Experience.razor | findstr "Ellips"` -> Existing job content preserved (Ellips B.V.) <!--p:{"type":"exit_code","value":0}-->
  - [x] Proof: `type Pages\Experience.razor | findstr "VRee"` -> Existing job content preserved (VRee B.V.) <!--p:{"type":"exit_code","value":0}-->
  - [x] Proof: `type Pages\Experience.razor | findstr "Graylake"` -> Existing job content preserved (Graylake studios) <!--p:{"type":"exit_code","value":0}-->

### Step 8: Projects page — dark-themed cards, search, filter, modal [x]

  - [x] Proof: `type Pages\Projects.razor | findstr "FilterProject"` -> Filter logic preserved in Projects page <!--p:{"type":"exit_code","value":0}-->
  - [x] Proof: `type Pages\Projects.razor | findstr "modal"` -> Modal detail view preserved in Projects page <!--p:{"type":"exit_code","value":0}-->
  - [x] Proof: `type Pages\Projects.razor | findstr "YouTube"` -> YouTube embed logic preserved <!--p:{"type":"exit_code","value":0}-->

### Step 9: Contact page — dark form + social links, EmailJS preserved [x]

  - [x] Proof: `type Pages\Contact.razor | findstr "sendEmailJS"` -> EmailJS integration preserved in Contact page <!--p:{"type":"exit_code","value":0}-->
  - [x] Proof: `type Pages\Contact.razor | findstr "HandleSubmit"` -> Form submission handler preserved <!--p:{"type":"exit_code","value":0}-->

### Step 10: Build succeeds with no errors [x]

  - [x] Proof: `dotnet build` -> Project builds successfully with no errors <!--p:{"type":"exit_code","value":0}-->

</definition_of_done>

## Open risks

<open_risks>
- Typing animation requires JS interop — must handle Blazor lifecycle (OnAfterRenderAsync)
- Projects page is 368 lines — CSS changes must not break filtering/modal/carousel logic
- Bootstrap dark overrides may conflict with existing component styles
- Font loading from Google Fonts CDN adds external dependency
</open_risks>

## Amendment log

- **2026-06-08T15:53:54.898Z** [undefined] modified: findstr cannot handle forward-slash paths on Windows — treats / as flag prefix, causing "Cannot open" error. Using type pipe pattern instead.
- **2026-06-08T15:53:58.563Z** [undefined] modified: findstr cannot handle forward-slash paths on Windows — treats / as flag prefix. Using type pipe pattern.
- **2026-06-08T15:54:06.584Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:54:10.170Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:54:13.539Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:54:22.132Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:54:25.826Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:54:29.268Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:54:32.636Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:54:39.525Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:54:43.265Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:54:46.569Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:54:53.800Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:54:57.313Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:55:00.705Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:55:03.898Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:55:12.433Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:55:16.087Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:55:19.559Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:55:22.780Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
- **2026-06-08T15:55:25.945Z** [undefined] modified: findstr forward-slash path bug on Windows. Using type pipe.
