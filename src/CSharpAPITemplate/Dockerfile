FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CSharpAPITemplate/CSharpAPITemplate.csproj", "CSharpAPITemplate/"]
RUN dotnet restore "CSharpAPITemplate/CSharpAPITemplate.csproj"
COPY . .
WORKDIR "/src/CSharpAPITemplate"
RUN dotnet build "CSharpAPITemplate.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CSharpAPITemplate.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CSharpAPITemplate.dll"]
