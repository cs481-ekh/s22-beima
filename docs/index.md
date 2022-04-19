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
![biema-login](https://user-images.githubusercontent.com/46760776/162551255-ad90162f-d89e-4cb1-85d1-005bad5a50c4.jpg)
![beima-devices](https://user-images.githubusercontent.com/46760776/162551260-e21f9dae-fb72-40df-8640-8492dae05674.jpg)
![beima-device-types](https://user-images.githubusercontent.com/46760776/162551266-ae547e22-dbfc-4084-ba5b-d14762110486.jpg)
![beima-add-device](https://user-images.githubusercontent.com/46760776/162551272-ab928bb6-2008-4cf8-a300-25fbc596f42a.jpg)
![beima-add-device-type](https://user-images.githubusercontent.com/46760776/162551276-6de9a271-b5a9-43ec-8f61-6f449686ecfd.jpg)
