#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Benefit.Consumer.Worker/Benefit.Consumer.Worker.csproj", "Benefit.Consumer.Worker/"]
COPY ["Benefit.Service/Benefit.Service.csproj", "Benefit.Service/"]
COPY ["Benefit.Domain/Benefit.Domain.csproj", "Benefit.Domain/"]
COPY ["Toolkit/Toolkit.csproj", "Toolkit/"]
RUN dotnet restore "Benefit.Consumer.Worker/Benefit.Consumer.Worker.csproj"
COPY . .
WORKDIR "/src/Benefit.Consumer.Worker"
RUN dotnet build "Benefit.Consumer.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Benefit.Consumer.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Benefit.Consumer.Worker.dll"]