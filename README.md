# OrderManagement
dotnet dev-certs https --clean
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p password1
//dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p password1
dotnet dev-certs https --trust


docker compose up


https://localhost:5021/swagger/index.html