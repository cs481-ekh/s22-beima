#!/bin/bash

# Change directories to BEIMA.Client
cd /BEIMA.Client || { echo "Changing directories failed"; exit 1; }
# Run react tests
npm test