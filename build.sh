#!/bin/bash

#Update the apt package manager
sudo apt update || { echo "Updating apt failed"; exit 1; }
# Install software-properties-common apt-transport-https for wget
sudo apt install software-properties-common apt-transport-https wget || { echo "Install software-properties-common apt-transport-https for wget failed"; exit 1; }
# Download chrome using wget
wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb || { echo "Download chrome using wget failed"; exit 1; }
# Installed chrome
sudo dpkg -i google-chrome-stable_current_amd64.deb || { echo "Installed chrome failed"; exit 1; }
# Download edge using wget
wget -q https://packages.microsoft.com/keys/microsoft.asc -O- | sudo apt-key add - || { echo "Download edge using wget failed"; exit 1; }
# Add microsoft key
sudo add-apt-repository "deb [arch=amd64] https://packages.microsoft.com/repos/edge stable main" || { echo "Add microsoft key failed"; exit 1; }
# Install edge
sudo apt install microsoft-edge-dev || { echo "Install edge failed"; exit 1; }
# Install NodeJS
sudo apt install nodejs || { echo "Installing NodeJS failed"; exit 1; }
# Install npm
sudo apt install npm || { echo "Installing npm failed"; exit 1; }
# Install Azure functions core tools
npm i -g azure-functions-core-tools@4
# Change directory to BEIMA.Client
cd ./BEIMA.Client || { echo "Changing directories failed"; exit 1; }
# Install node_modules
npm install react-scripts || { echo "Installing node_modules failed"; exit 1; }
# Install version 0.6.5 of jest-watch-typeahead
npm add --exact jest-watch-typeahead@0.6.5 || { echo "Installing version 0.6.5 of jest-watch-typeahead failed"; exit 1; }
# Install start-server-and-test node_module
npm install start-server-and-test || { echo "Install start-server-and-test node_module failed"; exit 1; }
# Install cypress
npm install cypress || { echo "Install cypress failed"; exit 1; }