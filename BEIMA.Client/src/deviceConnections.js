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
      //console.log(response.data);
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
  //console.log(newDevice);

  axios.post(apiUrl + "/device/?operation=insert", newDevice).then((response) => {
    //console.log(`Status: ${response.status}`);
    //console.log('Body: ', response.data);

    return response.data;
  }).catch(function (error) {
    if (error.response) {
      console.log(error.response.data);
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
    //console.log(`Status: ${response.status}`);
    //console.log('Body: ', response.data);

    return response.data;
  }).catch(function (error) {
    if (error.response) {
      console.log(error.response.data);
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
  //console.log(existingDevice);

  axios.post(host + ":" + port + "/api/device/?operation=update", existingDevice).then((response) => {
    //console.log(`Status: ${response.status}`);
    //console.log('Body: ', response.data);

    return response.data;
  }).catch(function (error) {
    if (error.response) {
      console.log(error.response.data);
      return error.response.data;
    }
  });
}


//GetDevice('620aeb23f50067dd0535bab').catch(console.error);
//GetDevice('620aeb23f50067dd0535bab3').catch(console.error);
//GetDevice('620b24c100319b2622228230').catch(console.error);
//InsertDevice(JSON.parse("[{ \"name\": \"deviceTypeId\", \"value\": \"testInsert\" },{ \"name\": \"serialNumber\", \"value\": \"insert12345\" }]")).catch(console.error);
//DeleteDevice('620b24c100319b2622228230').catch(console.error);
//UpdateDevice('[{ name: \'_id\', value: \'620aeb22f50067dd0535bab1\' },{ name: \'deviceTypeId\', value: \'a\' },{ name: \'serialNumber\', value: \'b12345\' }]').catch(console.error);
//UpdateDevice('[{ name: \'serialNumber\', value: \'b12345\' }]').catch(console.error);

// Export to make them available outside
module.exports.GetDevice = GetDevice;
module.exports.InsertDevice = InsertDevice;
module.exports.DeleteDevice = DeleteDevice;
module.exports.UpdateDevice = UpdateDevice;