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

#---------------------------------------------------------------------
#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY ../LibraryDeveloping LibraryDeveloping/
#COPY ../kzmp_energy_descktop kzmp_energy_descktop/ 
#COPY kzmpCloudAPI reactproject1/kzmpCloudAPI 
#COPY HangfireJobsToRabbit reactproject1/HangfireJobsToRabbit 
#COPY RabbitMQ_lib reactproject1/RabbitMQ_lib 
#WORKDIR reactproject1/kzmpCloudAPI
#RUN dotnet restore "kzmpCloudAPI.csproj"
#RUN dotnet build "kzmpCloudAPI.csproj" -c Release -o /app/build

#FROM build AS publish
#RUN dotnet publish "kzmpCloudAPI.csproj" -c Release -o /app/publish

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "kzmpCloudAPI.dll"]

#----------------------------------------------------------------------
FROM base AS final
WORKDIR /app
COPY ["kzmpCloudAPI_Publish/", "./"]
#COPY ["kzmpCloudAPI/bin/Release/net6.0/","./"]
#COPY --from=publish /app/publish .
COPY ["reactproject1/build/", "./ClientApp/build/"]
ENTRYPOINT ["dotnet", "kzmpCloudAPI.dll"]