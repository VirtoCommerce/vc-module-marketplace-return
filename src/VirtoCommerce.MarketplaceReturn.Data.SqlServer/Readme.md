## Package manager
```
Add-Migration Initial -Context VirtoCommerce.MarketplaceReturn.Data.Repositories.MarketplaceReturnDbContext -Project VirtoCommerce.MarketplaceReturn.Data.SqlServer -StartupProject VirtoCommerce.MarketplaceReturn.Data.SqlServer -OutputDir Migrations -Verbose -Debug
```

### Entity Framework Core Commands
```
dotnet tool install --global dotnet-ef --version 10.0.1
```

**Generate Migrations**
```
dotnet ef migrations add Initial
dotnet ef migrations add Update1
dotnet ef migrations add Update2
```
etc..

**Apply Migrations**
```
dotnet ef database update -- "{connection string}"
```
