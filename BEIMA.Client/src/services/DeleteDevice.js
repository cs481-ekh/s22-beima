import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs an ObjectId for the API to delete
 *
 * @param The ID of the device to delete
 * @return Error message or a succes indicator
 */
export default async deleteDevice (deviceId) => {
  //performs the post and returns an error message or a succes indicator
  const dbCall = await axios.post(API_URL + "device/" + deviceId + "/delete").catch(function (error) {
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
