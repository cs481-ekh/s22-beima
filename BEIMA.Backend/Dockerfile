# Support until 2025
FROM ubuntu:20.04
# Set non interactive to avoid having to make keyboard/timezone selection
ENV DEBIAN_FRONTEND noninteractive
# Allow connections on port 3000
EXPOSE 7071
# Create directory for front end
RUN mkdir /beima
# Set work directory
WORKDIR /beima
# Update the apt package manager
RUN apt update
# Set time zone env variable and install tzdata package using it
RUN TZ=Etc/UTC apt-get -y install tzdata
# Move all required folders into the container, must have all projects or it won't buildsince the others are referenced in the Backend csproj
COPY ./BEIMA.Backend/ /beima/BEIMA.Backend/
COPY ./BEIMA.Backend.Test/ /beima/BEIMA.Backend.Test
COPY ./BEIMA.Backend.FT/ /beima/BEIMA.Backend.FT
#i Install tools
RUN apt install -y wget
# Get MS installer package
RUN wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
# Install 
RUN dpkg -i packages-microsoft-prod.deb
# Delete downloaded file
RUN rm packages-microsoft-prod.deb
# Update sources
RUN apt update
# Install Azure Functions Tools
RUN apt install azure-functions-core-tools-4
# Install dotnet 6 SDK to allow building
RUN apt-get install -y dotnet-sdk-6.0
# Set work directory 
WORKDIR /beima/BEIMA.Backend
# Set Environment Variable to ignore globalization
RUN export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
# Start the API
ENTRYPOINT ["func", "start", "BEIMA.Backend.csproj"]