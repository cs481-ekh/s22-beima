#!/bin/bash
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
# Allow execution for scripts
chmod +x *.sh
# Call setup script
./testsetup.sh || { echo "Test setup failed"; exit 1; }
# Run react tests
npm test || { echo "Frontend tests failed"; exit 1; }
npm run cypress || { echo "Cypress tests failed"; exit 1; }
# Call cleanup script
./testcleanup.sh || { echo "Test cleanup failed"; exit 1; }