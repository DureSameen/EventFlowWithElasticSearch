# Build Stage
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine     AS build-env
WORKDIR  /EventFlowWithElasticSearch
Copy . . 
RUN dotnet restore   EventflowApi.Read/EventflowApi.Read.csproj
#COPY tests/tests.csproj ./tests/
#RUN dotnet restore  tests/tests.csproj

 
## test
#ENV TEAMCITY_PROJECT_NAME=fake
#RUN dotnet test tests/tests.csproj --verbosity=normal
WORKDIR  /EventFlowWithElasticSearch/EventflowApi.Read 
RUN dotnet publish EventflowApi.Read.csproj -o /publish
# Runtime Image Stage
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine   AS build
WORKDIR /publish
COPY --from=build-env /publish .
ENTRYPOINT ["dotnet", "EventflowApi.Read.dll"]