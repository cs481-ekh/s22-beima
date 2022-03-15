import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs a JSON object representing a new device type to the API for storage in DB
 *
 * @param The device type details to add to the DB
 * @return Error message or the inserted device type ID
 */
export default async function addDeviceType(deviceTypeDetails) {
  //performs the post and returns an error message or the inserted device ID
  const dbCall = await axios.post(API_URL + "device-type", deviceTypeDetails).catch(function (error) {
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
