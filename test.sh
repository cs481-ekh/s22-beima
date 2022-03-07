#!/bin/bash
# Change directory to Minio
cd ./Minio
# Start up the minio server locally
./minio server ./teststorage &
# Change directory to BEIMA.Backend.Test
cd ./BEIMA.Backend
# Start up the backend API locally
func start BEIMA.Backend.csproj --csharp &
# Give enough time for local API to finish starting up
sleep 20
# Run all backend tests
dotnet test || { echo "Backend tests failed"; exit 1; }
# Change directories to BEIMA.Client
cd ../BEIMA.Client || { echo "Changing directories failed"; exit 1; }
# Run react tests
npm test
npm run cypress
