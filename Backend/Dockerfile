FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar archivos de proyecto y restaurar dependencias
COPY *.sln .
COPY src/NPSApplication.API/*.csproj ./src/NPSApplication.API/
COPY src/NPSApplication.Application/*.csproj ./src/NPSApplication.Application/
COPY src/NPSApplication.Domain/*.csproj ./src/NPSApplication.Domain/
COPY src/NPSApplication.Infrastructure/*.csproj ./src/NPSApplication.Infrastructure/
RUN dotnet restore

# Copiar el resto del código y compilar
COPY . .
RUN dotnet publish -c Release -o out

# Construir imagen final
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "NPSApplication.API.dll"]
