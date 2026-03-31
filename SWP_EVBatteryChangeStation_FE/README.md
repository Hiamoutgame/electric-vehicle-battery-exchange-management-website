# SWP_EVBatteryChangeStation_FE

React + Vite frontend for the EV battery exchange management system.

## Run locally

```bash
npm install
npm run dev
```

## Required `.env`

```env
VITE_API_BASE_URL=https://localhost:7071/api
```

Optional keys used by some map features:

```env
VITE_GOOGLE_MAPS_API_KEY=YOUR_GOOGLE_MAPS_KEY
VITE_APP_VIETMAP_API_KEY=YOUR_VIETMAP_KEY
```

## Notes

- Backend should be running on `https://localhost:7071`
- Frontend has been aligned to the current backend `/api/v1` routes
- For full setup instructions, use the root [`../README.md`](../README.md)
