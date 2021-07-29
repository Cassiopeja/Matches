FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_14.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY *.sln .
COPY ["Pexeso.Web/Pexeso.Web.csproj", "Pexeso.Web/"]
COPY ["Pexeso.Core/Pexeso.Core.csproj", "Pexeso.Core/"]
RUN dotnet restore "Pexeso.Web/Pexeso.Web.csproj"
COPY . .
WORKDIR "/src/Pexeso.Web"
RUN dotnet build "Pexeso.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pexeso.Web.csproj" -c Release -o /app/publish
RUN mkdir /app/publish/ClientApp
RUN cp -R "ClientApp/dist" /app/publish/ClientApp/dist
#
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish publish/
COPY --from=publish  /app/publish/ClientApp/dist publish/ClientApp/dist
WORKDIR /app/publish
ENTRYPOINT ["dotnet", "Pexeso.Web.dll"]
