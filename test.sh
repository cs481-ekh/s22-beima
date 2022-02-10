#!/bin/bash
# Run unit tests
dotnet test ./BEIMA.Backend.Test/BEIMA.Backend.Test.csproj || { echo "Running Unit tests failed"; exit 1; }
# Change directories to BEIMA.Client
cd ./BEIMA.Client || { echo "Changing directories failed"; exit 1; }
# Run react tests
npm test