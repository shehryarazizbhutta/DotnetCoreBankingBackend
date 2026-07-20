FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY BankingApp.Api/BankingApp.Api.csproj BankingApp.Api/
COPY BankingApp.Application/BankingApp.Application.csproj BankingApp.Application/
COPY BankingApp.Domain/BankingApp.Domain.csproj BankingApp.Domain/
COPY BankingApp.Infrastructure/BankingApp.Infrastructure.csproj BankingApp.Infrastructure/

RUN dotnet restore BankingApp.Api/BankingApp.Api.csproj

COPY . .

RUN dotnet publish BankingApp.Api/BankingApp.Api.csproj \
    -c Release \
    -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BankingApp.Api.dll"]