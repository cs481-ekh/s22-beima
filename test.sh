#!/bin/bash
# Change directory to BEIMA.Backend.Test
cd ./BEIMA.Backend.Test
# Run backend unit tests
dotnet test ./BEIMA.Backend.Test.csproj || { echo "Unit tests failed"; exit 1; }
# Switch to FT directory
cd ../BEIMA.Backend.FT
# Run backend FTs
npm run build
dotnet test ./BEIMA.Backend.FT.csproj || { echo "FT tests failed"; exit 1; }
# Go back to root directory
cd ../
# Change directories to BEIMA.Client
cd ./BEIMA.Client || { echo "Changing directories failed"; exit 1; }
# Run react tests
npm test
npm run cypress