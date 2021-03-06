import axios from 'axios';
import {getCurrentUser, logout} from './Authentication.js';
import * as Constants from '../Constants.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * GETs a JSON object to the API for storage in DB
 *
 * @param The user ID
 * @return Error message or the user details
 */
export default async function getUser(userID) {
  let user = getCurrentUser();

  //performs the get and returns an error message or the user details
  const dbCall = await axios.get(API_URL + "user/" + userID, {headers : {Authorization : `Bearer ${user.token}`}}).catch(function (error) {
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
