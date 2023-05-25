FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Api/Api.csproj", "Api/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "Api/Api.csproj"
COPY . .
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

# Executar as migrações
RUN dotnet tool install --global dotnet-ef --version 6.0.0-rc.2.21480.5
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet ef database update --project ../Infrastructure/Infrastructure.csproj --no-build --context YourDbContextName

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Definir as variáveis de ambiente para conexão com o PostgreSQL
ENV DB_HOST=containers-us-west-140.railway.app
ENV DB_PORT=6484
ENV DB_DATABASE=railway
ENV DB_USER=postgres
ENV DB_PASSWORD=fD4dzRadzbcwlgBXrLr7

ENTRYPOINT ["dotnet", "Api.dll"]
