# FbClientCore

Fasade class for Firebird SQL command execution. Creates and manages connections and transactions, handles commit/dispose semantics. Packaged for .NET Core and distributed as a NuGet package.

## Project
- Project file: `FbClientCore\FbClientCore.csproj`
- Target framework: `netcoreapp3.1`
- Package version: `1.1.0`
- Description: Fasade class for Firebird SQL Command. Creates connection, transaction, manages commiting and disposing.

## Dependencies
- `FirebirdSql.Data.FirebirdClient` (10.3.4)
- `NLog` (4.7.13)
- Project reference: `..\FbClientBase\FbClientBase.csproj`

## Build & Pack
From repository root:
- Build: `dotnet build FbClientCore\FbClientCore.csproj -c Release`
- Pack: `dotnet pack FbClientCore\FbClientCore.csproj -c Release` (project has `GeneratePackageOnBuild` enabled)

## Usage
- Reference the produced NuGet package or add a project reference to `FbClientCore`.
- Use the facade classes to obtain and manage Firebird connections/transactions consistent with the `FbClientBase` contracts.

## Notes
- If you encounter duplicate assembly attribute errors (CS0579), check for duplicated attributes across projects and generated files. Inspect `Properties\AssemblyVersion.cs`, `Properties\AssemblyInfo.cs`, and the project property `__GenerateAssemblyInfo__`. Ensure only one source produces assembly attributes or disable auto-generation via `__GenerateAssemblyInfo__` as needed.
- Repository: https://github.com/Przemyslaw-Witczak/UniversalFunctions

## Contributing & Tests
- Run tests in sibling test projects with `dotnet test` or your solution test runner.
- Follow the repository's contribution guidelines and code style.