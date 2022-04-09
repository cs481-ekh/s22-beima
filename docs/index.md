# Boise State Energy Infrustructure Management Application (BEIMA)



Contributors:
<ul>
  <li>Ashlyn Adamson</li>
  <li>Joseph Moore</li>
  <li>Keelan Chen</li>
  <li>Kenneth Miller</li>
  <li>Thomas Hess</li>
</ul>


# Problem Statement

The Energy Infrastructure Management Department at Boise State University needs a system to easily track energy infrastructure information across campus. This information This application is a tool designed to map, record, display, and manage campus utility infrastructure data in a simple and effective manner.

# Solution
BEIMA satisfies this need by allowing university employees to take a location-tagged photo of a piece of equipment on their smartphone or tablet. Using a desktop browser, the user inputs the equipment’s relevant characteristics, the location-tagged photo, and additional documents if needed. The entered information is populated into a searchable database. The data can then be displayed in an organized and user-friendly interface via list and individual equipment views.

Lists can be filtered to show results that are relevant to the current need and exported on demand for reporting purposes.

# How it works
|||
|---|---|
|![dotnetLogo](https://user-images.githubusercontent.com/46760776/162549617-e8db237b-ddc7-4d74-9401-cec7192fd476.jpg)|A REST API was created using Mucrosoft's cross platform .NET framework to consume user requests, query the database, and return results to the client web application.
|![mongodbLogo](https://user-images.githubusercontent.com/46760776/162550192-6e8db356-5ed7-4797-9e87-0a4176ccf55f.png)|MongoDB allows for the flexibility needed to accomodate customized data fields to be added equipment categories/types.|
|![reactLogo](https://user-images.githubusercontent.com/46760776/162550382-801a4e99-ea9d-4eba-ae3f-9f0f94b387ed.png)|ReactJS renders the application to the user's browser.|
|![MINIO_wordmark](https://user-images.githubusercontent.com/46760776/162550479-260cee92-6e21-4e34-87df-70fba2641a94.png)|MinIO was selected to handle storage of all additional documents and images that the user uploads.|

# How it looks
