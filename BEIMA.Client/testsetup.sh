#!/bin/bash

echo "Add device using curl"
echo
# Check cURL command if available (required), abort if does not exists
type curl >/dev/null 2>&1 || { echo >&2 "Required curl but it's not installed. Aborting."; exit 1; }
echo

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

export CYPRESS_DEVICE_ID=$RESPONSE
