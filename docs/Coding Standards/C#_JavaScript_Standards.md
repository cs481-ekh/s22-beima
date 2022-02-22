# BEIMA C# and JavaScript/HTML Coding Standards

The following are coding standards any developer working on BEIMA should follow. 

These standards focus on C# and JavaScript/HTML languages.

## Indentation

Development done in C# should use 4 spaces for indentation, and development in JavaScript/HTML should use 2 spaces for indentation. 

## Headers

Method headers are most important in C# classes. The standard should follow the C# default method headers, using `///` and `<summary>`, `<param>` and `<returns>` tags.   
``` 
/// <summary>
/// Inserts a device into the "devices" collection
/// </summary>
/// <param name="doc">BsonDocument that contains the fully formed device document (including all required and optional fields)</param>
/// <returns>ObjectId of the newly inserted object if successful, null if failed</returns>
```

While method headers are not required for JavaScript development, it is advised to provide useful comments when needed.