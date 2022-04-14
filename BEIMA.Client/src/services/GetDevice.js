import axios from 'axios';
import {getCurrentUser, logout} from './Authentication.js';
import * as Constants from '../Constants.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * Gets a JSON object from the DB with the provided device ID
 *
 * @param The device id in the database to retrieve
 * @return JSON document from the DB wrapped in a JSON object with the HTTP response code
 */
export default async function getDevice(deviceId) {
  let user = getCurrentUser();

  //performs the get and returns the data or error
  const dbCall = await axios.get(API_URL + "device/" + deviceId, {headers : {Authorization : `Bearer ${user.token}`}}).catch(function (error) {
    if (error.response) {
      return error.response;
    }
  });

  if(dbCall.status === Constants.HTTP_UNAUTH_RESULT){
    logout();
    return;
  }

  const response = {
    status: dbCall.status,
    response: dbCall.data
  }

  return response;
}
