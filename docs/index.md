# Boise State Energy Infrustructure Mapping Application (BEIMA)



Contributors:
<ul>
  <li>Ashlyn Adamson</li>
  <li>Joseph Moore</li>
  <li>Keelan Chen</li>
  <li>Kenneth Miller</li>
  <li>Thomas Hess</li>
</ul>


# Problem Statement

The Energy Infrastructure Management Department at Boise State University needs a system to easily track energy infrastructure information across campus. The application needs to map, record, display, and manage campus utility infrastructure data in a simple and effective manner.

# Solution
BEIMA satisfies this need by allowing university employees to take a location-tagged photo of a piece of equipment on their smartphone or tablet for upload. Using a desktop browser, the user inputs the equipment’s relevant characteristics, the location-tagged photo, and additional documents if needed. The entered information is populated into a searchable database so the user can access the interface and display it in an organized and user-friendly interface via lists and individual device views.

Lists can be filtered to show results that are relevant to the current need and exported on demand for reporting purposes.

# How it works

|Technology/Framework|Purpose|
|---|---|
|![dotnetLogo](https://user-images.githubusercontent.com/46760776/162549617-e8db237b-ddc7-4d74-9401-cec7192fd476.jpg)|A REST API was created using Microsoft's cross platform .NET framework to consume user requests, query the database, and return results to the client web application.
|![mongodbLogo-92high](https://user-images.githubusercontent.com/46760776/162550602-d6637e10-d723-4ac9-8a5a-bcf44388218c.png)|MongoDB's flexible document storage allows us to accomodate customized data field requirements on Device Types and individual Devices.|
|![reactLogo-92high](https://user-images.githubusercontent.com/46760776/162550616-a64e2ffc-827c-42e3-9586-0fef48deef15.png)|ReactJS renders the application in the user's browser.|
|![MINIO_wordmark-92high](https://user-images.githubusercontent.com/46760776/162550629-f63193b7-6d9c-445c-9087-12e9409b6c6a.png)|MinIO was selected to handle storage of all additional documents and images that the user uploads.|

# How it looks
![ioLogin](https://user-images.githubusercontent.com/46760776/164535744-4949ecd7-f45e-41e0-8e31-834899eed55b.png)
![ioDeviceList](https://user-images.githubusercontent.com/46760776/164535771-a5582fb5-4159-4e92-b900-d9041cbac231.png)
![ioDeviceTypes](https://user-images.githubusercontent.com/46760776/164535786-417920bf-6179-4805-b5dd-ad3a4ce45b29.png)
![ioDeviceList](https://user-images.githubusercontent.com/46760776/164535804-133d5498-b319-4704-bbd3-a6de97049ed9.png)
![ioAddDevice](https://user-images.githubusercontent.com/46760776/164535814-250e9340-a3c5-4037-b932-58dc48f1cd2e.png)
![ioMap](https://user-images.githubusercontent.com/46760776/164535830-66933392-ade0-4d14-b8a0-16ce94572a00.png)

