# BSU Energy Infrastructure Mapping App (BEIMA)

The Energy Infrastructure Management Department at Boise State University needs a system to easily track energy infrastructure information across campus. The application needs to map, record, display, and manage campus utility infrastructure data in a simple and effective manner.

BEIMA satisfies this need by allowing university employees to take a location-tagged photo of a piece of equipment on their smartphone or tablet for upload. Using a desktop browser, the user inputs the equipment’s relevant characteristics, the location-tagged photo, and additional documents if needed. The entered information is populated into a searchable database so the user can access the interface and display it in an organized and user-friendly interface via lists and individual device views.

Lists can be filtered to show results that are relevant to the current need and exported on demand for reporting purposes.

## Tech Stack
BEIMA was built with a cloud-first mentality, with the capacity to run on-premises as well. This approach heavily influenced our decisions as to which technologies we chose for our tech stack.

### Frontend
We chose to build our web app using React. We chose React because of its ability to create rich and dynamic webpages with great performance. We wanted BEIMA to feel slick and modern, which meant having to create custom components with high interactivity. React allowed us to do exactly that, and we feel that we have accomplished those goals well.

### Backend
For our backend, we went with a serverless model, with the ability to substitute in on-premise software in place of serverless solutions.

#### REST API
We implemented our REST API in C# using the Azure Functions template. This granted us the flexibility of both deploying on the cloud as a serverless app, as well as deploying on-premises using the traditional server model. By deploying BEIMA as a serverless app, this allows the user to cut down on infrastructure costs, since serverless apps can be configured to run only when needed. This results in major cost savings over using a full VM to host all of our infrastructure. 

#### Database
For our database, we chose MongoDB. The main driver behind this choice was the necessity to dynamically create custom field values for different Device Types in BEIMA. Our customer wanted BEIMA to be scalable and extensible, which meant the ability to add custom devices and device types was necessary. This mirrors the entity-attribute-value data model, which is considered by many as an anti-pattern in SQL. 

Since MongoDB follows the NoSQL paradigm and is completely schema-less, we felt it was the appropriate database choice due to MongoDB's flexibility. We had to create a custom dynamic schema that allows end users to define their own device types, as well as all the custom fields they need. This choice paid off well, as MongoDB's schema-less flexibility allowed us to completely fulfill the customer's requirement for scalability and extensibility in adding custom Device Types.

Infrastructure wise, we used Azure Cosmos to host our database for testing purposes. MongoDB can also be run locally, which fulfills the on-premises requirement.

#### Storage Provider
Currently, BEIMA can be set up using two different storage providers, depending on if the app needs to be deployed on the cloud, or on-premises.

Azure Storage is our cloud storage provider. We chose Azure Storage because we are already working closely with the Microsoft tech stack. Since the REST API already uses Azure Functions, it follows closely that most users would also use Azure Storage. BEIMA is fully compatible with the Azure Storage API, and can be adapted for use with AWS S3 APIs using third party software.

MinIO is also supported as a storage provider when deploying on-premises. MinIO essentially acts like a cloud storage provider, but it can be deployed and accessed locally.

Since our storage service has been abstracted, any storage provider can be used with BEIMA with very little refactoring required.

[![beima Actions Status](https://github.com/cs481-ekh/s22-beima/actions/workflows/beima.yml/badge.svg)](https://github.com/cs481-ekh/s22-beima/actions)
