import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * Gets a JSON object from the DB with the provided device type ID
 *
 * @param The device type id in the database to retrieve
 * @return JSON document from the DB wrapped in a JSON object with the HTTP response code
 */
const GetDeviceType = async(deviceTypeId) => {
  //performs the get and returns the data or error
  const deviceTypeCall = await axios.get(API_URL + "device-type/" + deviceTypeId).catch(function (error) {
      if (error.response) {
        return error.response;
    }
  });

  const response = {
    status: deviceTypeCall.status,
    response: deviceTypeCall.data
  }

  console.log(response);
  return response;
}

export default GetDeviceType
