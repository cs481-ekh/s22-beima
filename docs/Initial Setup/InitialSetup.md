
## Initial setup

Install a git client for Windows
Install Node JS
Install Visual Studio 2022+ for .Net 6 functionality
Install MongoDb and Mongo Compass (GUI to view the database)
Install Minio

Clone project into chosen location

Place environment variable file ".env" at ./BEIMA.Client/.env and ensure all lines except those under the Local Dev heading are commented with " # "
Place the "local.settings.json" file at ./BEIMA.Backend/local.settings.json make sure CurrentEnv is set to dev-local
Valid CurrentEnv settings for local.settings.json are dev-local, deploy

Enter ./BEIMA.Client/ directory

Execute:
* npm install (to install dependencies)
* npm start (launches front end in browser)
* The URL will need "/beima" added to the end of it to access the site
  * It may be possible to update this behavior and have it launch with the addition already in place

Launch Visual Studio
* Open ./BEIMA.Backend/BEIMA.Backend.sln
* Run the project in Debug Any CPU
* API is ready to accept requests

Databases and Collections will be created in the database as documents are added.


Docker
* Change the CurrentEnv to 'deploy' in the local.settings.json file.
* Build images with '&lt;sudo&gt; docker-compose build'
* Run images with '&lt;sudo&gt; docker-compose up'
* Remove cached/intermediate containers with '&lt;sudo&gt; docker system prune'
* To test deployment locally, uncomment the proxy config in docker-compose.yaml and set the local.settings.json to "deploy"
