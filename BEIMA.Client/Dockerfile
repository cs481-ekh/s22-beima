# Support until 2025
from ubuntu:20.04
# Set non interactive to avoid having to make keyboard/timezone selection
ENV DEBIAN_FRONTEND noninteractive
# Allow connections on port 3000
EXPOSE 3000
# Create directory for front end
RUN mkdir /beima
# Set work directory
WORKDIR /beima
# Update the apt package manager
RUN apt update
# Set time zone env variable and install tzdata package using it
RUN TZ=Etc/UTC apt-get -y install tzdata
# Install build utilities
RUN apt install -y curl
# Get the latest (ver 16) node server PPA
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash
# Install node from PPA
RUN apt install -y nodejs
# Move work directory to client
WORKDIR /beima/BEIMA.Client
# Import package lists
COPY ./BEIMA.Client/package.json /beima/BEIMA.Client/
COPY ./BEIMA.Client/package-lock.json /beima/BEIMA.Client/
# Install Node dependencies
RUN npm i
# Install web server
RUN npm install -g serve
# Move all required folders into the container
COPY ./BEIMA.Client/ /beima/BEIMA.Client/
# Build client in production mode
RUN npm run build
# Run the front end
CMD ["serve", "-s", "build"]