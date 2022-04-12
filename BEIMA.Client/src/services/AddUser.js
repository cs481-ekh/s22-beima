import axios from 'axios';
import {getCurrentUser, logout} from './Authentication.js';
import * as Constants from '../Constants.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs a JSON object to the API for storage in DB
 *
 * @param The user details to add to the DB
 * @return Error message or the inserted user ID
 */
export default async function addUser(userDetails) {
  let user = getCurrentUser();

  //performs the post and returns an error message or the inserted user ID
  const dbCall = await axios.post(API_URL + "user", userDetails, {headers : {Authorization : `Bearer ${user.token}`}}).catch(function (error) {
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
