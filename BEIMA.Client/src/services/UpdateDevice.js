import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs a JSON object to the database to be updated
 *
 * @param The JSON of the device to be updated
 * @return Error message or a succes indicator
 */
export default async function updateDevice(deviceDetails) {
  //performs the post and returns an error message or a succes indicator
  const dbCall = await axios.post(API_URL + "device/" + deviceDetails._id + "/update", deviceDetails).catch(function (error) {
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