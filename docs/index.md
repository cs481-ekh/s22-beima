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
|![mongodbLogo-92high](https://user-images.githubusercontent.com/46760776/162550602-d6637e10-d723-4ac9-8a5a-bcf44388218c.png)|MongoDB allows for the flexibility needed to accomodate customized data fields to be added equipment categories/types.|
|![reactLogo-92high](https://user-images.githubusercontent.com/46760776/162550616-a64e2ffc-827c-42e3-9586-0fef48deef15.png)|ReactJS renders the application to the user's browser.|
|![MINIO_wordmark-92high](https://user-images.githubusercontent.com/46760776/162550629-f63193b7-6d9c-445c-9087-12e9409b6c6a.png)|MinIO was selected to handle storage of all additional documents and images that the user uploads.|

# How it looks
