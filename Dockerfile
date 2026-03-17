FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and all project files first (for layer caching)
COPY src/Explorer.sln src/
COPY src/Explorer.API/Explorer.API.csproj src/Explorer.API/
COPY src/BuildingBlocks/Explorer.BuildingBlocks.Core/Explorer.BuildingBlocks.Core.csproj src/BuildingBlocks/Explorer.BuildingBlocks.Core/
COPY src/BuildingBlocks/Explorer.BuildingBlocks.Infrastructure/Explorer.BuildingBlocks.Infrastructure.csproj src/BuildingBlocks/Explorer.BuildingBlocks.Infrastructure/
COPY src/BuildingBlocks/Explorer.BuildingBlocks.Tests/Explorer.BuildingBlocks.Tests.csproj src/BuildingBlocks/Explorer.BuildingBlocks.Tests/
COPY src/Explorer.Architecture.Tests/Explorer.Architecture.Tests.csproj src/Explorer.Architecture.Tests/
COPY src/Modules/Blog/Explorer.Blog.API/Explorer.Blog.API.csproj src/Modules/Blog/Explorer.Blog.API/
COPY src/Modules/Blog/Explorer.Blog.Core/Explorer.Blog.Core.csproj src/Modules/Blog/Explorer.Blog.Core/
COPY src/Modules/Blog/Explorer.Blog.Infrastructure/Explorer.Blog.Infrastructure.csproj src/Modules/Blog/Explorer.Blog.Infrastructure/
COPY src/Modules/Blog/Explorer.Blog.Tests/Explorer.Blog.Tests.csproj src/Modules/Blog/Explorer.Blog.Tests/
COPY src/Modules/Encounters/Explorer.Encounters.API/Explorer.Encounters.API.csproj src/Modules/Encounters/Explorer.Encounters.API/
COPY src/Modules/Encounters/Explorer.Encounters.Core/Explorer.Encounters.Core.csproj src/Modules/Encounters/Explorer.Encounters.Core/
COPY src/Modules/Encounters/Explorer.Encounters.Infrastructure/Explorer.Encounters.Infrastructure.csproj src/Modules/Encounters/Explorer.Encounters.Infrastructure/
COPY src/Modules/Encounters/Explorer.Encounters.Tests/Explorer.Encounters.Tests.csproj src/Modules/Encounters/Explorer.Encounters.Tests/
COPY src/Modules/Payments/Explorer.Payments.API/Explorer.Payments.API.csproj src/Modules/Payments/Explorer.Payments.API/
COPY src/Modules/Payments/Explorer.Payments.Core/Explorer.Payments.Core.csproj src/Modules/Payments/Explorer.Payments.Core/
COPY src/Modules/Payments/Explorer.Payments.Infrastructure/Explorer.Payments.Infrastructure.csproj src/Modules/Payments/Explorer.Payments.Infrastructure/
COPY src/Modules/Payments/Explorer.Payments.Tests/Explorer.Payments.Tests.csproj src/Modules/Payments/Explorer.Payments.Tests/
COPY src/Modules/Stakeholders/Explorer.Stakeholders.API/Explorer.Stakeholders.API.csproj src/Modules/Stakeholders/Explorer.Stakeholders.API/
COPY src/Modules/Stakeholders/Explorer.Stakeholders.Core/Explorer.Stakeholders.Core.csproj src/Modules/Stakeholders/Explorer.Stakeholders.Core/
COPY src/Modules/Stakeholders/Explorer.Stakeholders.Infrastructure/Explorer.Stakeholders.Infrastructure.csproj src/Modules/Stakeholders/Explorer.Stakeholders.Infrastructure/
COPY src/Modules/Stakeholders/Explorer.Stakeholders.Tests/Explorer.Stakeholders.Tests.csproj src/Modules/Stakeholders/Explorer.Stakeholders.Tests/
COPY src/Modules/Tours/Explorer.Tours.API/Explorer.Tours.API.csproj src/Modules/Tours/Explorer.Tours.API/
COPY src/Modules/Tours/Explorer.Tours.Core/Explorer.Tours.Core.csproj src/Modules/Tours/Explorer.Tours.Core/
COPY src/Modules/Tours/Explorer.Tours.Infrastructure/Explorer.Tours.Infrastructure.csproj src/Modules/Tours/Explorer.Tours.Infrastructure/
COPY src/Modules/Tours/Explorer.Tours.Tests/Explorer.Tours.Tests.csproj src/Modules/Tours/Explorer.Tours.Tests/

RUN dotnet restore src/Explorer.sln

# Copy everything and publish
COPY src/ src/
RUN dotnet publish src/Explorer.API/Explorer.API.csproj -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Create wwwroot/uploads directory
RUN mkdir -p wwwroot/uploads

ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000
ENTRYPOINT ["dotnet", "Explorer.API.dll"]
