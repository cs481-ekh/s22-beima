#!/bin/bash

#Update the apt package manager
sudo apt update || { echo "Updating apt failed"; exit 1; }
# Install software-properties-common apt-transport-https for wget
sudo apt install software-properties-common apt-transport-https wget || { echo "Install software-properties-common apt-transport-https for wget failed"; exit 1; }
# Install gnupg (for MongoDB key)
sudo apt install gnupg || { echo "Install gnupg failed"; exit 1; }
# Install MongoDB key
wget -qO - https://www.mongodb.org/static/pgp/server-5.0.asc | sudo apt-key add - || { echo "Download MongoDB using wget failed"; exit 1; }
# Create list file for MongoDB
echo "deb [ arch=amd64,arm64 ] https://repo.mongodb.org/apt/ubuntu focal/mongodb-org/5.0 multiverse" | sudo tee /etc/apt/sources.list.d/mongodb-org-5.0.list || { echo "Create list file for MongoDB failed"; exit 1; } 
# Install MongoDB
sudo apt install -y mongodb-org || { echo "Install MongoDB failed"; exit 1; }
# Install NodeJS
sudo apt install nodejs || { echo "Installing NodeJS failed"; exit 1; }
# Install npm
sudo apt install npm || { echo "Installing npm failed"; exit 1; }
# Install Azure functions core tools
npm i -g azure-functions-core-tools@4 || { echo "Installing Azure functions core tools failed"; exit 1; }
# install axios for http requests
npm install axios || { echo "Installing axios for http requests failed"; exit 1; }
# Allow executable for test.sh
chmod +x test.sh
# Make Minio directory
mkdir Minio || { echo "Creating Minio directory failed"; exit 1; }
# Change directory to Minio
cd ./Minio || { echo  "Changing directories failed", exit 1; }
# Download minio server client using wget
wget -q https://dl.min.io/server/minio/release/linux-amd64/minio || { echo "Download Minio using wget failed"; exit 1; }
# Set minio server file to allow executing
chmod +x minio || { echo "Setting minio execute permissions failed"; exit 1; }
# Make storage directory
mkdir storage || { echo "Creating storage directory failed"; exit 1; }
# Change directory to storage
cd ./storage || { echo  "Changing directories failed", exit 1; }
# Make files 'bucket' directory
mkdir files || { echo "Creating files directory failed"; exit 1; }
# Make test 'bucket' directory
mkdir test-files || { echo "Creating test-files directory failed"; exit 1; }
# Change back to root directory
cd ../../
# Change directory to BEIMA.Backend
cd ./BEIMA.Backend || { echo "Changing directories failed"; exit 1; }
# Build all backend projects 
dotnet build || { echo "Building backend projects failed"; exit 1; }
# Change back to root directory
cd ../
# Change directory to BEIMA.Client
cd ./BEIMA.Client || { echo "Changing directories failed"; exit 1; }
# Install node_modules
npm install react-scripts || { echo "Installing node_modules failed"; exit 1; }
# Install icons module
npm install react-icons --save || { echo "Installing icon module failed"; exit 1; }
# Install version 0.6.5 of jest-watch-typeahead
npm add --exact jest-watch-typeahead@0.6.5 || { echo "Installing version 0.6.5 of jest-watch-typeahead failed"; exit 1; }
# Install start-server-and-test node_module
npm install start-server-and-test || { echo "Install start-server-and-test node_module failed"; exit 1; }
# Install cypress
npm install cypress || { echo "Install cypress failed"; exit 1; }
# Build client
npm run build || { echo "Building client failed"; exit 1; }
