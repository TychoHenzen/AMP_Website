# Azure Deploy Setup — Next Steps

Context-survival doc for the multi-site monorepo migration + Nido Suave deploy.
Written 2026-06-29. Pick up here after a restart.

## Where things stand

Done (committed):
- Repo restructured to a **multi-site monorepo**:
  - `sites/portfolio/` — Tycho's portfolio (Blazor WASM .NET 6). Already live on existing SWA `icy-meadow-06fe80803`.
  - `sites/nido-suave/` — Denise's massage site (Blazor WASM .NET 8, Dutch). **Not yet deployed.**
  - `tools/ProjectEditor/` — local-only editor.
- Path-filtered workflows: `.github/workflows/deploy-portfolio.yml` and `deploy-nido.yml`.
- **Azure CLI 2.87.0** installed locally (`C:\Program Files\Microsoft SDKs\Azure\CLI2\wbin\az.cmd`).
- **Azure MCP server** registered project-scoped in `.mcp.json` (`@azure/mcp`, runs via `cmd /c npx`). Auth uses your `az login` — no secrets in the file.

Account facts (from portal):
- Subscription name: **Azure subscription 1**
- Subscription ID: **`7b0a4103-a709-4adf-9d70-ae2cfa43279d`**
- Tenant ID: **`8d570529-0ba9-46d6-8ed2-24a1b1683a75`** ("Default Directory", `siriusblack9999hotmail.onmicrosoft.com`)
- Role: Owner

## Blocker: az login needs MFA on the tenant

Plain `az login` failed: `AADSTS50076 ... must use multi-factor authentication`.
Fix — log in against the tenant explicitly (forces the MFA prompt):

```powershell
az login --tenant 8d570529-0ba9-46d6-8ed2-24a1b1683a75
az account set --subscription 7b0a4103-a709-4adf-9d70-ae2cfa43279d
az account show -o table   # confirm the right sub is active
```

(Run these yourself — interactive browser. In Claude prompt: prefix with `! `.)

## MCP server activation

New `.mcp.json` servers only load on Claude Code **restart**. On first load Claude will
ask to **approve the `azure` project MCP server** — approve it. Note: Azure MCP has **no
native Static Web Apps tool**; SWA is managed through its `extension` (az CLI passthrough).
So the steps below use `az` directly — they work whether or not the MCP server is loaded.

## Create the Nido Suave Static Web App

Pick/confirm a resource group and region (`westeurope` is closest for NL).

```bash
# 1. Resource group (reuse the portfolio's RG if you want them together; list them first)
az group list -o table
# az group create -n rg-websites -l westeurope    # only if you need a new one

# 2. Create the SWA (Free tier). No --source/--repo: we deploy via our own GH Action.
az staticwebapp create \
  --name nido-suave \
  --resource-group <resource-group> \
  --location westeurope \
  --sku Free

# 3. Get the deploy token
az staticwebapp secrets list \
  --name nido-suave \
  --resource-group <resource-group> \
  --query "properties.apiKey" -o tsv
```

## Wire the token into GitHub Actions

The `deploy-nido.yml` workflow expects secret `AZURE_STATIC_WEB_APPS_API_TOKEN_NIDO_SUAVE`.

```bash
# Requires gh authenticated (gh auth status)
az staticwebapp secrets list --name nido-suave --resource-group <rg> --query "properties.apiKey" -o tsv \
  | gh secret set AZURE_STATIC_WEB_APPS_API_TOKEN_NIDO_SUAVE
```

Then push to `master` — the path filter (`sites/nido-suave/**`) triggers `deploy-nido.yml`
and the site goes live at the SWA's default `*.azurestaticapps.net` URL.

## Custom domain / DNS (later)

Each site = its own SWA = its own custom domain. Once Nido is live:

```bash
az staticwebapp hostname set --name nido-suave --resource-group <rg> --hostname www.nidosuave.nl
```

Then add the validation/CNAME records at the domain registrar. Repeat per future site
(repair site, wife's CV site) — new SWA each, new path-filtered workflow each.

## Quick reference — what I (Claude) can do once you're logged in

- Drive all the `az staticwebapp` commands above from here.
- Set the GitHub secret via `gh secret set`.
- Trigger/observe the deploy.

I **cannot** run `az login` (interactive browser) — that stays with you.
