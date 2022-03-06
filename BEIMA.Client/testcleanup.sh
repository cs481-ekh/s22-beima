#!/bin/bash

# Removing device from DB
PAYLOAD=%CYPRESS_DEVICE_ID%
curl --header "Content-Type: application/json" --request POST http://localhost:7071/api/device/${PAYLOAD}/delete || { echo "Cannot delete device"; exit 1; }