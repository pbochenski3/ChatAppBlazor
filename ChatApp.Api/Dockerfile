
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ChatApp.Api/ChatApp.Api.csproj", "ChatApp.Api/"]
COPY ["ChatApp.Web/ChatApp.Web.csproj", "ChatApp.Web/"]
COPY ["ChatApp.Application/ChatApp.Application.csproj", "ChatApp.Application/"]
COPY ["ChatApp.Domain/ChatApp.Domain.csproj", "ChatApp.Domain/"]
COPY ["ChatApp.Infrastructure/ChatApp.Infrastructure.csproj", "ChatApp.Infrastructure/"]

RUN dotnet restore "ChatApp.Api/ChatApp.Api.csproj"

COPY . .

WORKDIR "/src/ChatApp.Api"
RUN dotnet publish "ChatApp.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ChatApp.Api.dll"]