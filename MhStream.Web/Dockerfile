FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MhStream.Web/MhStream.Web.csproj", "MhStream.Web/"]
RUN dotnet restore "MhStream.Web/MhStream.Web.csproj"
COPY . .
WORKDIR "/src/MhStream.Web"
RUN dotnet build "MhStream.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MhStream.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MhStream.Web.dll"]
