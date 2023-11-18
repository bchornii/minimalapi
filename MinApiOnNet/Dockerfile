FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MinApiOnNet/MinApiOnNet.csproj", "MinApiOnNet/"]
RUN dotnet restore "MinApiOnNet/MinApiOnNet.csproj"
COPY . .
WORKDIR "/src/MinApiOnNet"
RUN dotnet build "MinApiOnNet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MinApiOnNet.csproj" -c Release -o /app/publish

# Install dotnet debug tools
RUN dotnet tool install --tool-path /tools dotnet-trace \
 && dotnet tool install --tool-path /tools dotnet-counters \
 && dotnet tool install --tool-path /tools dotnet-dump \
 && dotnet tool install --tool-path /tools dotnet-gcdump

FROM base AS final

# Copy dotnet-tools
WORKDIR /tools
COPY --from=publish /tools .

# Copy app
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "MinApiOnNet.dll"]
