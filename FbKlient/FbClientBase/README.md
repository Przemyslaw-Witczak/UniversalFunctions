# FbClientBase

Interface-contract library used by the FbClientCore facade for Firebird SQL. Responsible for creating connections and transactions and for managing commit/dispose semantics. Packaged for reuse across .NET Standard/.NET Framework/.NET Core projects.

## Supported Target Frameworks
- `netstandard2.1`
- `net48`
- `netcoreapp3.1`

## Package / Versioning
- Version set in the project: `1.1.0`
- Package is produced on build: `GeneratePackageOnBuild=true`
- Project repository: https://github.com/Przemyslaw-Witczak/UniversalFunctions

## Dependencies
- `FirebirdSql.Data.FirebirdClient` (10.3.4)
- `System.Threading.Tasks.Extensions` (4.6.0)

## Build
From the repository root:
- Build: `dotnet build FbClientBase\FbClientBase.csproj -c Release`
- Pack (if not auto-packed): `dotnet pack FbClientBase\FbClientBase.csproj -c Release`

## Usage
Reference the produced NuGet package or add the project reference. Example: