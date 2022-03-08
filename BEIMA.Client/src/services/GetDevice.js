import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * Gets a JSON object from the DB with the provided device ID
 *
 * @param The device id in the database to retrieve
 * @return JSON document from the DB wrapped in a JSON object with the HTTP response code
 */
export default async function getDevice(deviceId) {
  //performs the get and returns the data or error
    const dbCall = await axios.get(API_URL + "device/" + deviceId).catch(function (error) {
      if (error.response) {
        return error.response;
      }
    });

    const response = {
      status: dbCall.status,
      response: dbCall.data
    }

    return response;
}
