# Decrypt

Decrypt Demo

This demo project is currently configured to work with Microsoft SQL Server using Windows Authentication, so the following connection string should work as-is:
Server=localhost;Database=Decrypt;Trusted_Connection=True;TrustServerCertificate=True;
---

I added two .bat files to make setup easier:

Setup.bat

- Checks the main dependencies
- Runs the backend service for the first time so the database can be created and migrations can be applied
- Then runs the data migration tool to import data from the mock storage

Start.bat

- Starts the backend and frontend applications

---

The data migration tool, MockLoader, is a console app that parses data from JavaScript files. 
It is based on the file structure you provided, so the parsed file must contain top-level arrays for all entities (organizations, projects, and so on).
When running either Setup.bat or MockLoader directly from the console, paste the path to the mock data file to import the data into the database.
MockLoader should be run only after the backend has been started for the first time, to ensure that the database already exists.
The backend API also includes Swagger, which can be used to explore and test the endpoints.
