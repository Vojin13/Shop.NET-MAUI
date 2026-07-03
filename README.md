# Linea

A .NET MAUI shop app built for an Android exam project. Customers can browse/search products and check out with a local cart; admins manage products, categories, and users.

## Tech stack

- **.NET MAUI**, targeting `net10.0-android` and `net10.0-windows10.0.19041.0`
- **CommunityToolkit.Mvvm** — MVVM (`ObservableObject`, `[ObservableProperty]`, `[RelayCommand]`)
- **RestSharp** — HTTP calls to the backend (used inline in ViewModels, no separate service/repository layer)
- **Newtonsoft.Json** — (de)serialization
- **System.IdentityModel.Tokens.Jwt** — decoding the JWT returned on login
- **SecureStorage** — persists the logged-in session and the local shopping cart

## Backend: Platzi Fake Store API

This app talks to the open-source **Platzi Fake Store API** (`fake-store-api` on npm) — a NestJS + TypeORM backend with an in-memory SQLite database that re-seeds on every restart. It runs as a separate project alongside this one.

### ⚠️ Requires Node.js v20.x — do not use a newer Node version

Confirmed in the backend project itself:
- `.nvmrc` → `v20.17.0`
- `package.json` → `"engines": { "node": "20.x" }`

Using a newer Node (e.g. v22/v24) will break `npm install`: the pinned `better-sqlite3` version has no prebuilt binary for newer Node ABI versions, so npm falls back to compiling it from source via `node-gyp`, which requires a working Python 3 + native build toolchain that most machines won't have set up. Installing Node 20.17.0 (e.g. via `nvm-windows`) avoids this entirely.

### Backend setup

1. Install Node.js **20.17.0** (use `nvm-windows` if you also have other Node versions installed; run `nvm use 20.17.0` before the steps below).
2. In the backend project's root, create a **`.env.local`** file (the exact filename matters — not `.env`) with:
   ```
   ACCESS_SECRET_KEY=<any string>
   REFRESH_SECRET_KEY=<any string>
   RECOVERY_SECRET_KEY=<any string>
   NODE_ENV=dev
   OPENAI_API_KEY=
   PORT=3001
   ```
   (`OPENAI_API_KEY` can be left blank — it only disables an optional AI content-moderation check.)
3. `npm install`
4. `npm run start:dev`
5. API base URL: `http://localhost:3001/api/v1`
6. Swagger docs: `http://localhost:3001/docs`

The database is in-memory SQLite and reseeds automatically on every server restart — no separate seed step needed, and no external DB server (SQL Server/Postgres) required.

### Test accounts (seeded on every restart)

| Email | Password | Role |
|---|---|---|
| john@mail.com | changeme | customer |
| maria@mail.com | 12345 | customer |
| admin@mail.com | admin123 | admin |

## Features

**Auth** — single page (`AuthPage`) with a Log In / Register pill toggle, inline validation, server-error banners.

**Customer**
- Shop: search, category filter, pagination (10/page)
- Product details: image gallery with dot navigation, quantity selector, add to cart
- Cart: SecureStorage-based (no cart endpoint on the API), quantity controls, checkout clears the cart
- My Profile: avatar, email, role, member-since date

**Admin**
- Products: full CRUD, search, category filter, pagination
- Users: full CRUD, local (client-side) search
- Categories: full CRUD, local (client-side) search
- My Profile

## Running the app

- **Windows Machine** (fastest for local iteration): select the "Windows Machine" debug target in Visual Studio and run.
- **Android emulator/device**: the API base URL in the ViewModels is currently set to `http://localhost:3001/api/v1`, which assumes the app and API run on the same machine (i.e. Windows Machine target). For an Android emulator, this needs to point to `http://10.0.2.2:3001/api/v1` instead (or use `adb reverse tcp:3001 tcp:3001` on a physical device to keep using `localhost`).

## Design

Dark-only "Linea"/Nimbus visual direction — warm charcoal surfaces, coral/terracotta accent (`#D97757`), cream text. Space Grotesk for headings, product names, prices, and primary buttons; Hanken Grotesk for everything else.
