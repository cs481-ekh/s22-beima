#!/bin/bash
# Change directory to BEIMA.Backend.Test
cd ./BEIMA.Backend.Test
# Run backend unit tests
dotnet test ./BEIMA.Backend.Test.csproj || { echo "Unit tests failed"; exit 1; }
# Navigate to backend project
cd ../
cd ./BEIMA.Backend
# Start the backend API locally
func start BEIMA.Backend.csproj --csharp &
# Switch to FT directory
cd ../
cd ./BEIMA.Backend.FT
# Give enough time for local API to finish starting up
sleep 15
# Run backend FTs
dotnet test ./BEIMA.Backend.FT.csproj || { echo "FT tests failed"; exit 1; }
# Go back to root directory
cd ../
# Change directories to BEIMA.Client
cd ./BEIMA.Client || { echo "Changing directories failed"; exit 1; }
# Run react tests
npm test
npm run cypress
