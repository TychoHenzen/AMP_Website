# Azure Setup & Architecture — Status + Next Steps

Context-survival doc. Pick up here after a restart. Last updated 2026-06-29.

## Account / environment

- Subscription: **Azure subscription 1** — ID `7b0a4103-a709-4adf-9d70-ae2cfa43279d`
- Tenant: `8d570529-0ba9-46d6-8ed2-24a1b1683a75` (`siriusblack9999hotmail.onmicrosoft.com`), role Owner
- Login (MFA required on tenant): `az login --tenant 8d570529-0ba9-46d6-8ed2-24a1b1683a75`
- Azure CLI 2.87.0 at `C:\Program Files\Microsoft SDKs\Azure\CLI2\wbin\az.cmd`
- Azure MCP server registered in `.mcp.json` (`@azure/mcp`, `cmd /c npx`, auth via az login)

## Azure inventory (as of 2026-06-29)

| Resource | Type | RG | Region | Purpose |
| --- | --- | --- | --- | --- |
| AutoARPG | Static Web App | BWG_AutoARPG | West Europe | portfolio (icy-meadow-06fe80803) |
| Manim | Cognitive Services | BWG_AutoARPG | East US | separate (leave alone) |
| nido-suave | Static Web App | DuurzaamDigitaal_group | West Europe | nido (zealous-moss-0d5fb8903) |
| DuurzaamDigitaal | App Service | DuurzaamDigitaal_group | North Europe | repair site (server-side) |
| ASP-DuurzaamDigitaalgroup-a945 | App Service Plan | DuurzaamDigitaal_group | North Europe | repair plan |
| duurzaamdigitaal | Cosmos DB | DuurzaamDigitaal_group | (multi) | SHARED DB (free-tier) |
| DuurzaamDigitaal-id-a078 | Managed Identity | DuurzaamDigitaal_group | North Europe | repair identity |

RG deletion is OFF — every RG holds live resources. Tidy = create dedicated RG and *move* SWAs (Phase 4).

## Decisions locked

- Multi-site monorepo, one Azure host per site, path-filtered deploys.
- Repo `AMP_Website` stays **PUBLIC** → repair site merged via **squash (no history)** + secrets externalized.
- Shared **Cosmos account = existing `duurzaamdigitaal`** (free-tier; only one allowed).
- WASM sites can't hold DB secrets → **one shared Azure Functions app** (.NET isolated) fronts Cosmos.

## DONE

- Restructured to `sites/` + `tools/`; portfolio + nido (WASM) live/ready.
- Deploy secrets set in GitHub repo: `AZURE_STATIC_WEB_APPS_API_TOKEN_ICY_MEADOW_06FE80803`,
  `AZURE_STATIC_WEB_APPS_API_TOKEN_NIDO_SUAVE`, `AZUREWEBAPP_PUBLISHPROFILE`.
- **Cosmos key rotated** (2026-06-29): the leaked PRIMARY key (was committed in the repair repo's
  `appsettings*.json`) is DEAD. Live app moved to SECONDARY via App Service setting
  `CosmosDb__ConnectionString`, then primary regenerated. Zero downtime. The dead key remains only in
  the *private* DuurzaamDigitaal repo history (harmless — invalid).
- **Repair site merged** → `sites/duurzaam-digitaal/` (squash, no history). Sanitized appsettings
  (empty connection string + container IDs kept). Fixed triple-BOM in MainLayout.razor + _Imports.razor.
  Added to root sln. `deploy-duurzaam.yml` created (path-filtered, publish-profile deploy).
  Builds clean; 18/18 tests pass.

## TODO

### Phase 1 — finish the merge (almost done)
- [ ] Commit + push. Pushing redeploys portfolio (harmless) and triggers nido + duurzaam deploys.
      Verify each Action goes green.
- [ ] Confirm repair site still serves after a monorepo-sourced deploy (config now from App Service setting).
- [ ] (optional) Archive/private-note the old `TychoHenzen/DuurzaamDigitaal` repo to avoid drift.
- [ ] (optional) App Service `httpsOnly=true` on DuurzaamDigitaal (currently false).

### Phase 2 — shared data layer + Functions API
- [x] Extract `shared/Data` class library (`Amp.Data`, .NET 8) from the repair site's `Data/`
      (CosmosDbConfig, Entities, Repositories). Namespace `DuurzaamDigitaal.Data` → `Amp.Data`.
      Repair site references it; builds clean, 18/18 tests pass. (commit 4cd0ee4)
- [x] New `apps/api/` Azure Functions app (`Amp.Api`, .NET 8 isolated), references `Amp.Data`,
      registers CosmosDbConfig + CosmosClient from app settings. `GET /api/health` endpoint. Builds.
- [ ] **Provision** Functions app in Azure (consumption): needs a storage account + function app.
      Set its `CosmosDb__ConnectionString` app setting to the **secondary** Cosmos connection string.
      Configure CORS for site origins. Add `deploy-api.yml` (path filter `apps/api/**`) + publish secret.
- [ ] (later) Repair site can switch from direct Cosmos to the shared lib/API; keep direct for now.

      Provision sketch:
      ```bash
      az storage account create -n ampapistore<suffix> -g DuurzaamDigitaal_group -l westeurope --sku Standard_LRS
      az functionapp create -n amp-api -g DuurzaamDigitaal_group --consumption-plan-location westeurope \
        --runtime dotnet-isolated --runtime-version 8 --functions-version 4 --storage-account ampapistore<suffix>
      SEC=$(az cosmosdb keys list -n duurzaamdigitaal -g DuurzaamDigitaal_group --type connection-strings \
        --query "connectionStrings[?keyKind=='Secondary'].connectionString | [0]" -o tsv)
      az functionapp config appsettings set -n amp-api -g DuurzaamDigitaal_group \
        --settings "CosmosDb__ConnectionString=$SEC" "CosmosDb__DatabaseId=..." # + container ids
      az functionapp cors add -n amp-api -g DuurzaamDigitaal_group --allowed-origins https://<nido-host>
      ```

### Phase 3 — Nido Suave appointment booking
- [ ] Cosmos: `nido` database + `appointments`, `timeslots` containers.
- [ ] API endpoints: list timeslots, create appointment, etc.
- [ ] Nido WASM booking UI calling the API; replace Contact/Behandelingen placeholders as needed.

### Phase 4 — RG tidy + hardening
- [ ] Create `rg-websites` (westeurope); MOVE AutoARPG + nido-suave SWAs into it (preserves hostnames).
- [ ] Custom domains per site (`az staticwebapp hostname set ...` / App Service custom domain).

## Handy commands

```bash
# Cosmos connection strings (secondary is the one currently in use)
az cosmosdb keys list -n duurzaamdigitaal -g DuurzaamDigitaal_group --type connection-strings

# Repair App Service settings / restart
az webapp config appsettings list -n DuurzaamDigitaal -g DuurzaamDigitaal_group -o table
az webapp restart -n DuurzaamDigitaal -g DuurzaamDigitaal_group

# Static web app deploy tokens
az staticwebapp secrets list -n nido-suave -g DuurzaamDigitaal_group --query properties.apiKey -o tsv
```

I (Claude) can run all `az`/`gh` steps. Only `az login` (interactive browser) is yours.
