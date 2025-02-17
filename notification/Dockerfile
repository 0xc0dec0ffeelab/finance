FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
# copy csproj file and restore
COPY notification/notification.csproj ./

ARG GITHUB_USERNAME
ARG GITHUB_TOKEN

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet nuget add source "https://nuget.pkg.github.com/0xc0dec0ffeelab/index.json" --name GitHub --username ${GITHUB_USERNAME} --password ${GITHUB_TOKEN} --store-password-in-clear-text \
    && dotnet restore notification.csproj

# Copy everything else and build
COPY notification/ ./
# trim-self-contained and trimmed .NET build
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish notification.csproj -c Release -o out


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "notification.dll"]