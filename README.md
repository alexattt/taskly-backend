# TasklyAPI

### About

API consists of two main endpoints, /auth and /tasks. /auth is for user creation and login. /tasks is for task related CRUD operations.<br/>
Swagger documentation has been added.

### Requirements 

.NET 8 SDK<br/>
SQL Server or SQL Server Express LocalDB<br/>
Visual Studio, VS Code, or your preferred IDE<br/>

Update the connection string in appsettings.json and appsettings.Development.json.<br/>
Everything is Entity Framework based, you can find database migrations in TasklyAPI Migrations folder.<br/>
To apply migrations, run command ```dotnet ef database update```.<br/>

### Tests

API unit tests have been added in TasklyTests project. xUnit is being used.

