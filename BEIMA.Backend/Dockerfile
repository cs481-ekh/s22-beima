FROM mcr.microsoft.com/dotnet/sdk:6.0 AS installer-env
EXPOSE 7071
# Copy in source
COPY ./BEIMA.Backend/ /src/dotnet-function-app
# set up for publish
RUN cd /src/dotnet-function-app && mkdir -p /home/site/wwwroot && dotnet publish *.csproj --output /home/site/wwwroot
# To enable ssh & remote debugging on app service change the base image to the one below
#FROM mcr.microsoft.com/azure-functions/dotnet:4-appservice
FROM mcr.microsoft.com/azure-functions/dotnet:4
ENV AzureWebJobsScriptRoot=/home/site/wwwroot AzureFunctionsJobHost__Logging__Console__IsEnabled=true
