import axios from 'axios';
import {getCurrentUser, logout} from './Authentication.js';
import * as Constants from '../Constants.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs an ObjectId for the API to delete
 *
 * @param The ID of the user to delete
 * @return Error message or a success indicator
 */
export default async function deleteUser(userId) {
  let user = getCurrentUser();

  //performs the post and returns an error message or a success indicator
  const dbCall = await axios.post(API_URL + "user/" + userId + "/delete", null, {headers : {Authorization : `Bearer ${user.token}`}}).catch(function (error) {
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
