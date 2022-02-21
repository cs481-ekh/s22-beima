//used for determining environment the connection runs against
var DEBUG = true;
var debugUrl = 'http://localhost';
var debugPort = 7071
var prodUrl = '';
var prodPort = '';

//axios package includes it's own try catch and error output functions 
//and is declared globally so each request doesn't create its own connection
const axios = require('axios');

//set up the API url to use
const host = DEBUG ? debugUrl : prodUrl;
const port = DEBUG ? debugPort : prodPort;
const apiUrl = host + ":" + port + "/api";

/// <summary>
/// Gets a device with the specified ID
/// </summary>
/// <param name="deviceId">The device ID to retrieve from the database</param>
/// <returns>The device with the specified ID from the DB API in JSON format, or the error message from the API</returns>
async function GetDevice(deviceId) {
  //perform the get and returns the data or error
  axios.get(apiUrl + "/device/?id=" + deviceId).then(response => {
      return response.data;
    }).catch(function (error) {
      if (error.response) {
        return error.response.data;
      }
  });
}

/// <summary>
/// Inserts a device and returns the new device that was created
/// </summary>
/// <param name="newDevice">JSON string representing the device to insert</param>
/// <returns>The inserted device and associated data, or the error message from the API</returns>
async function InsertDevice(newDevice) {
  //perform the post with the insert data in the body and return result
  axios.post(apiUrl + "/device/?operation=insert", newDevice).then((response) => {
    return response.data;
  }).catch(function (error) {
    if (error.response) {
      return error.response.data;
    }
   });
}

/// <summary>
/// deletes a device with the specified ID
/// </summary>
/// <param name="objectId">The device ID to delete from the database</param>
/// <returns>A success indicator, or the error message from the API</returns> TODO: make sure this is correct
async function DeleteDevice(deviceId) {
  axios.post(apiUrl + "/device/?operation=delete", deviceId).then((response) => {
    return response.data;
  }).catch(function (error) {
    if (error.response) {
      return error.response.data;
    }
  });
}

/// <summary>
/// Updates a device and returns the new device data that was updated
/// </summary>
/// <param name="existingDevice">JSON string representing the device to update</param>
/// <returns>The updated device and associated data, or the error message from the API</returns>
async function UpdateDevice(existingDevice) {
  axios.post(host + ":" + port + "/api/device/?operation=update", existingDevice).then((response) => {
    return response.data;
  }).catch(function (error) {
    if (error.response) {
      return error.response.data;
    }
  });
}

// Export to make them available outside
module.exports.GetDevice = GetDevice;
module.exports.InsertDevice = InsertDevice;
module.exports.DeleteDevice = DeleteDevice;
module.exports.UpdateDevice = UpdateDevice;