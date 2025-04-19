# Step 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app


# Copy the project files to the build context
COPY IpoAppSolution.sln ./
COPY IpoApp.API/IpoApp.API.csproj IpoApp.API/
COPY IpoApp.Core/IpoApp.Core.csproj IpoApp.Core/
COPY IpoApp.Data/IpoApp.Data.csproj IpoApp.Data/
COPY IpoApp.Models/IpoApp.Models.csproj IpoApp.Models/
COPY IpoApp.Repository/IpoApp.Repository.csproj IpoApp.Repository/

COPY . ./

RUN dotnet restore

RUN dotnet publish -c Release -o out
# Copy the necessary project files


# Restore dependencies for all projects
RUN dotnet restore "IpoApp.API/IpoApp.API.csproj"
# Step 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/out ./

EXPOSE 80
ENTRYPOINT ["dotnet", "IpoApp.API.dll"]
