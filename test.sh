#!/bin/bash
# Change directory to Minio
cd ./Minio
# Start up the minio server locally
./minio server ./storage &
# Change directory to root
cd ../
# Change directory to BEIMA.Backend.Test
cd ./BEIMA.Backend
# Start up the backend API locally
func start BEIMA.Backend.csproj --csharp &
# Give enough time for local API to finish starting up
sleep 20
# Run all backend tests
dotnet test || { echo "Backend tests failed"; exit 1; }
# Frontend Cypress tests are not ran here due to them causing too many inconsistent and hard to solve failures