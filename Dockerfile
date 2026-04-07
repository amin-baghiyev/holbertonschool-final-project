FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY ["src/PLDMS.PL/PLDMS.PL.csproj", "src/PLDMS.PL/"]
COPY ["src/PLDMS.BL/PLDMS.BL.csproj", "src/PLDMS.BL/"]
COPY ["src/PLDMS.Core/PLDMS.Core.csproj", "src/PLDMS.Core/"]
COPY ["src/PLDMS.DL/PLDMS.DL.csproj", "src/PLDMS.DL/"]

RUN dotnet restore "src/PLDMS.PL/PLDMS.PL.csproj"

COPY src/ src/
WORKDIR "/app/src/PLDMS.PL"
RUN dotnet publish "PLDMS.PL.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PLDMS.PL.dll"]
