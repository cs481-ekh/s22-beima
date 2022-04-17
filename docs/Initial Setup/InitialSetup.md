
## Initial setup

Install a git client for Windows
Install Node JS
Install Visual Studio 2022+ for .Net 6 functionality
Install MongoDb and Mongo Compass (GUI to view the database)
Install Minio

Clone project into chosen location

Place environment variable file ".env" in ./BEIMA.Client/.env
Place the "local.settings.json" file in ./BEIMA.Backend/local.settings.json

Enter ./BEIMA.Client/ directory

Execute:
* npm install (to install dependencies)
* npm start (launches front end in browser)

Launch Visual Studio
* Open ./BEIMA.Backend/BEIMA.Backend.sln
* Run the project in Debug Any CPU
* API is ready to accept requests

Databases and Collections will be created in the database as documents are added.
