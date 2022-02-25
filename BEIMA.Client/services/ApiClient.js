//pull in the vars file to get the API URL
require('dotenv').config({ path: '../.env' })

//axios package includes it's own try catch and error output functions 

const axios = require('axios');

const API_URL = process.env.REACT_APP_API_URL;

/// <summary>
/// Gets a device with the specified ID
/// </summary>
/// <param name="deviceId">The device ID to retrieve from the database</param>
/// <returns>JSON object containing the HTTP status code and the API response</returns>
const GetDevice = async (deviceId) => {
  //performs the get and returns the data or error
  const response = await axios.get(API_URL + "device/" + deviceId).then(response => { 
      let stringdata = JSON.stringify(response.data);
      return JSON.parse('{ "status" : "' + response.status + '", "response" : ' + JSON.stringify(response.data) + '}');
    }).catch(function (error) {
        if (error.response) {
        return JSON.parse('{ "status" : "' + error.response.status + '", "response" : ' + JSON.stringify(error.response.data) + '}');
      }
  });
  
  return response;
}

//module.exports.<Custom Name> = <Function Name>
//OR
//module.exports.<Function Name> = <Function Name>
module.exports.GetDevice = GetDevice;