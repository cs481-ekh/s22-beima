#!/bin/bash

#Update the apt package manager
sudo apt update || {echo "Updating apt failed"; exit 1;}
# Install NodeJS
sudo apt install nodejs || {echo "Installing NodeJS failed"; exit 1;}
# Install npm
sudo apt install npm || {echo "Installing npm failed"; exit 1;}
# Change directory to BEIMA.Client
cd /BEIMA.Client || {echo "Changing directories failed"; exit 1;}
# Install node_modules
npm install react-scripts || {echo "Installing node_modules failed"; exit 1;}
# Install version 0.6.5 of jest-watch-typeahead
npm add --exact jest-watch-typeahead@0.6.5 || {echo "Installing version 0.6.5 of jest-watch-typeahead failed"; exit 1;}