# Form Submissions App

A simple application for submitting forms and viewing submitted data.

## Running

Backend:
```bash
cd backend
dotnet run
```
Will run on http://localhost:5080

Frontend:
```bash
cd frontend
npm install
npm run dev
```

## API

- POST /api/forms/{formKey}/submissions - submit a form
- GET /api/submissions - list submissions (you can add ?search=... for searching)
- GET /api/submissions/{id} - get submission by ID

## Notes

- Uses in-memory database, data is not persisted after restart
- Swagger is available in dev mode at /swagger

