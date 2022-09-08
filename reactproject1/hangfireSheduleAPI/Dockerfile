#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5188

# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /src
# COPY ["hangfireSheduleAPI/hangfireSheduleAPI.csproj", "hangfireSheduleAPI/"]
# RUN dotnet restore "hangfireSheduleAPI/hangfireSheduleAPI.csproj"
# COPY . .
# WORKDIR "/src/hangfireSheduleAPI"
# RUN dotnet build "hangfireSheduleAPI.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "hangfireSheduleAPI.csproj" -c Release -o /app/publish

#FROM base AS final
#WORKDIR /app
# COPY --from=publish /app/publish .
#COPY bin/Release/net6.0/publish .
COPY publish_api/ .
ENTRYPOINT ["dotnet", "hangfireSheduleAPI.dll"]