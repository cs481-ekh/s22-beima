import axios from 'axios';
import {getCurrentUser} from './Authentication.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs a JSON object to the database to update a device type
 *
 * @param The JSON of the device type to be updated
 * @return Error message or a succes indicator
 */
export default async function updateDeviceType(deviceTypeDetails) {
  let user = getCurrentUser();

  //performs the post and returns an error message or a succes indicator
  const dbCall = await axios.post(API_URL + "device-type/" + deviceTypeDetails.id + "/update", deviceTypeDetails, {headers : {Authorization : `Bearer ${user.token}`}}).catch(function (error) {
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