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
# Add device to DB
PAYLOAD='{
    "deviceTypeId": "473830495728394823103456",
    "deviceTag": "Test",
    "manufacturer": "Test",
    "modelNum": "Test",
    "serialNum": "Test",
    "yearManufactured": 2022,
    "notes": "Test",
    "fields": {},
    "location": {
        "buildingId": "a19830495728394829103986",
        "notes": "Test",
        "latitude": "Test",
        "longitude": "Test"
    },
    "lastModified": {
        "date": "2022-03-03T23:42:21.098Z",
        "user": "Test"
    }
}'

RESPONSE=$(curl --header "Content-Type: application/json" --request POST --data "${PAYLOAD}" http://localhost:7071/api/device) || { echo "Cannot add device"; exit 1; }
# Run react tests
npm test || { echo "Frontend tests failed"; exit 1; }
npm run cypress || { echo "Cypress tests failed"; exit 1; }
# Delete added device
curl --header "Content-Type: application/json" --request POST http://localhost:7071/api/device/${RESPONSE}/delete || { echo "Cannot delete device"; exit 1; }