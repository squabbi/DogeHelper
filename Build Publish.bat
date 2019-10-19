cd DogeHelper
dotnet clean -c Release
dotnet restore
dotnet build -c Release -f netcoreapp2.0
dotnet publish -c Release -f netcoreapp2.0
explorer "bin\Release\netcoreapp2.0\publish\"
pause