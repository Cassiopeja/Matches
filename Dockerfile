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
COPY ["Matches.Web/Matches.Web.csproj", "Matches.Web/"]
COPY ["Matches.Core/Matches.Core.csproj", "Matches.Core/"]
RUN dotnet restore "Matches.Web/Matches.Web.csproj"
COPY . .
WORKDIR "/src/Matches.Web"
RUN dotnet build "Matches.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Matches.Web.csproj" -c Release -o /app/publish
RUN mkdir /app/publish/ClientApp
RUN cp -R "ClientApp/dist" /app/publish/ClientApp/dist
#
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish publish/
COPY --from=publish  /app/publish/ClientApp/dist publish/ClientApp/dist
WORKDIR /app/publish
#ENTRYPOINT ["dotnet", "Matches.Web.dll"]
# Use the following instead for Heroku
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Matches.Web.dll
