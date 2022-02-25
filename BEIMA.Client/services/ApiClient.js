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
  const dbCall = await axios.get(API_URL + "device/" + deviceId);
  const response = {
    status: dbCall.status,
    response: dbCall.data
  
  return response;
}

//module.exports.<Custom Name> = <Function Name>
//OR
//module.exports.<Function Name> = <Function Name>
module.exports.GetDevice = GetDevice;