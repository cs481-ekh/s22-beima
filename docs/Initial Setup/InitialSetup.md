
## Initial setup

Install a git client for Windows
Install Node JS
Install visual studio 2022+ for .Net 6 functionality
Install MongoDb and Mongo Compass (included with download)

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

Launch Mongo Compass
 * Click the + sign at the bottom of the left pane to add a database
 * Database name: beima
 * Collection name: deviceTypes
This will create the DB with one of the required collections.
Once that is completed the + sign inline with the DB name to add the remaining required collections to mirror the production environment.
* DB is ready to accept documents for all collections created