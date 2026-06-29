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
| AutoARPG | Static Web App | rg-websites | West Europe | portfolio (icy-meadow-06fe80803; domains www.aerialmage.com + arpg.aerialmage.com) |
| Manim | Cognitive Services | BWG_AutoARPG | East US | separate (leave alone) |
| nido-suave | Static Web App | rg-websites | West Europe | nido (zealous-moss-0d5fb8903) |
| DuurzaamDigitaal | App Service | DuurzaamDigitaal_group | North Europe | repair site (server-side) |
| ASP-DuurzaamDigitaalgroup-a945 | App Service Plan | DuurzaamDigitaal_group | North Europe | repair plan |
| duurzaamdigitaal | Cosmos DB | DuurzaamDigitaal_group | (multi) | SHARED DB (serverless) |
| amp-comms | Communication Services | DuurzaamDigitaal_group | global | ACS for booking emails |
| amp-email | Email Comm. Service | DuurzaamDigitaal_group | global | Azure-managed email domain (DKIM/SPF/DMARC verified) |
| DuurzaamDigitaal-id-a078 | Managed Identity | DuurzaamDigitaal_group | North Europe | repair identity |

RG deletion is OFF — every RG holds live resources. SWAs now live in `rg-websites` (Phase 4 done).
`amp-api-730024` (+ storage `ampapi730024`), Cosmos `duurzaamdigitaal`, and the repair App Service
still sit in `DuurzaamDigitaal_group`.

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
- [x] **Provisioned** (2026-06-29): Function App `amp-api-730024` + storage `ampapi730024`
      (RG DuurzaamDigitaal_group, westeurope, consumption, dotnet-isolated 8). App settings set
      (Cosmos secondary conn + db/container IDs). CORS allows nido + portfolio origins.
      `deploy-api.yml` deploys via `AMP_API_PUBLISHPROFILE`. Live: `GET /api/health` →
      `{"status":"ok","cosmosConfigured":true}`.
      GOTCHA: SCM basic-auth was disabled by default → functions-action got 401. Fix:
      `az resource update -g <rg> --namespace Microsoft.Web --parent sites/<funcapp>
      --resource-type basicPublishingCredentialsPolicies --name scm --set properties.allow=true`.
      Also had to register the `Microsoft.Storage` provider first (one-time, several minutes).
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

### Phase 3 — Nido Suave appointment booking ✅ DONE (2026-06-29)
- [x] Cosmos: `nido` database + `appointments` container (pk `/partitionKey` = date). Serverless
      account → no throughput to provision.
- [x] Data layer `shared/Data/Nido` (`NidoAppointment` + `NidoAppointmentRepository`, own db).
- [x] API (`apps/api`): `GET /api/nido/availability?date=` (slots minus booked, NL tz, open
      Tue–Sat 09–17) + `POST /api/nido/appointments` (validation + 409 slot-conflict).
- [x] Nido site `/boeken` booking page (service → date → slot grid → contact → confirm),
      `ApiBaseUrl` from `wwwroot/appsettings.json`; nav link + CTAs on Behandelingen/Contact.
- Verified live end-to-end (201 / slot-taken / 409 / 400-Dutch-errors). All deploys green.
- NOTE: Cosmos has an IP firewall (ipRules) — the API (Azure, via the `0.0.0.0` "allow Azure
  services" rule) can reach it, but **local data-plane access from a home IP is blocked (403)**.
  To inspect/edit docs locally: use Portal Data Explorer, or temporarily add your IP to ipRules.
- Schedule rules live in `apps/api/Nido/NidoSchedule.cs` (days/hours/slot length) — edit there.

#### Nido extras ✅ DONE (2026-06-29) — admin view + email
- **Admin endpoint**: `GET /api/nido/appointments` (AuthorizationLevel.Function) lists upcoming
  bookings. Page: nido site `/admin` (not in nav) — paste the key once, stored in localStorage.
  Retrieve the key: `az functionapp keys list -n amp-api-730024 -g DuurzaamDigitaal_group
  --query functionKeys.default -o tsv` (the per-function key was empty; the host key works).
  Rotate it any time with `az functionapp keys set ...` if it leaks.
- **Email on booking** via ACS (`amp-comms` + `amp-email`, Azure-managed domain). On a successful
  booking the API sends a notification to the business inbox + a confirmation to the customer
  (best-effort — a mail failure never breaks the booking). App settings on the Function app:
  `Acs__ConnectionString`, `Acs__SenderAddress` (DoNotReply@<id>.azurecomm.net),
  `Acs__BusinessEmail` (= tychohenzen@gmail.com). Code: `apps/api/Nido/BookingEmailService.cs`.
- **Atomic slot reservation** (2026-06-29): each hour-slot maps to a deterministic Cosmos doc id
  `{date}_{time}` (partition key = date). `CreateItemAsync` is atomic, so concurrent bookings for
  the same hour can't both win — the loser gets a Cosmos 409 → `SlotUnavailableException` → HTTP 409.
  Verified with two parallel POSTs (one 201, one 409). The old read-then-create check stays as a
  friendly fast-path. Code: `NidoAppointmentRepository.CreateAsync`.
- **Admin delete** (2026-06-29): `DELETE /api/nido/appointments/{id}?date=yyyy-MM-dd` (Functions key)
  frees a slot; `/admin` has a "Verwijderen" button per booking. Idempotent. This is now the easy way
  to delete docs (works through the API, no Cosmos-firewall issue).
- Not yet: configurable services/prices, per-day custom hours.

### Phase 4 — RG tidy + hardening ✅ DONE (2026-06-29)
- [x] Created `rg-websites` (westeurope); MOVED AutoARPG + nido-suave SWAs into it via
      `az resource move`. Hostnames, deploy tokens, and custom domains all preserved; both serve 200.
      Source RGs kept (BWG_AutoARPG still has Manim; DuurzaamDigitaal_group still has the App Service,
      Cosmos, Function App, storage) — no RG deletion.
- [x] Custom domains: portfolio already has `www.aerialmage.com` + `arpg.aerialmage.com` (status
      Ready, survived the move). Apex `aerialmage.com` is NOT bound (only www/arpg).
- [ ] (later) Custom domains for nido (`nidosuave.nl`) + repair (`duurzaamdigitaal.nl`) when DNS ready.
      Flow per SWA: `az staticwebapp hostname set -n <swa> -g rg-websites --hostname <domain>` →
      Azure returns a CNAME/TXT validation record → add it at the registrar → it flips to Ready.

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
