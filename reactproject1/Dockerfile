FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
#EXPOSE 5001
#EXPOSE 443

#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY ["kzmpCloudAPI/kzmpCloudAPI.csproj", "kzmpCloudAPI/"]
#COPY ["kzmpCloudAPI/../../","react1"]
#WORKDIR "./react1/reactproject1/kzmpCloudAPI"
#RUN dotnet restore "kzmpCloudAPI/kzmpCloudAPI.csproj"
#COPY . .
#WORKDIR "/src/react1/reactproject1/kzmpCloudAPI"
#RUN dotnet build "kzmpCloudAPI.csproj" -c Release -o /app/build

#FROM build AS publish
#RUN dotnet publish "kzmpCloudAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY ["kzmpCloudAPI_Publish/", "./"]
#COPY --from=publish /app/publish .
COPY ["reactproject1/build/", "./ClientApp/build/"]
ENTRYPOINT ["dotnet", "kzmpCloudAPI.dll"]