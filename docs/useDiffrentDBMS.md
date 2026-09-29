## Addtional information
MovieQuotes currently uses MySQL. Other relational databases may be possible because the data layer uses EF Core, but additional provider/configuration changes may be required.

and get the connection strings add it update your connection string in AppSetting;

get the correct [enity framework database providers] nuget package i use mysql 
```
MySql.EntityFrameworkCore
```

to work with sqlserver you need to install this package instead
```
Microsoft.EntityFrameworkCore.SqlServer
```

and then remove the [migration folder in Infrastructure](./Src/MovieQuotes.Infrastructure/Migrations/) projet and reginerate it with the new provider
```bash
cd ./Src/MovieQuotes.Infrastructure/
dotnet ef Migrations add IntialCreate -s ../MovieQuotes.Api/MovieQuotes.Api.csproj
```

[enity framework database providers]: https://learn.microsoft.com/en-us/ef/core/what-is-new/nuget-packages#database-providers
