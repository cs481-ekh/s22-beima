import axios from 'axios';
import {getCurrentUser, logout} from './Authentication.js';
import * as Constants from '../Constants.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs a JSON object to the database to be updated
 *
 * @param The JSON of the building to be updated
 * @return Error message or a success indicator
 */
export default async function updateBuilding(buildingDetails) {
  let user = getCurrentUser();

  //performs the post and returns an error message or a success indicator
  const dbCall = await axios.post(API_URL + "building/" + buildingDetails._id + "/update", buildingDetails, {headers : {Authorization : `Bearer ${user.token}`}}).catch(function (error) {
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