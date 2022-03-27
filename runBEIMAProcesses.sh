#!/bin/bash

# Change directory to Minio
cd ./Minio
# Start up the minio server locally
./minio server ./storage &
# Change directory to root
cd ../
# Change directory to BEIMA.Backend
cd ./BEIMA.Backend
# Start up the backend API locally
func start BEIMA.Backend.csproj --csharp &
# Give enough time for local API to finish starting up
sleep 20
# Change directory to root
cd ../
# Change directory to frontend
cd BEIMA.Client
# Start up the frontend locally
npm start