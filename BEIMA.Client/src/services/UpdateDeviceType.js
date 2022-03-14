import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs a JSON object to the database to update a device type
 *
 * @param The JSON of the device type to be updated
 * @return Error message or a succes indicator
 */
export default async function updateDeviceType(deviceTypeDetails) {
  //performs the post and returns an error message or a succes indicator
  const dbCall = await axios.post(API_URL + "device-type/" + deviceTypeDetails.id + "/update", deviceTypeDetails).catch(function (error) {
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