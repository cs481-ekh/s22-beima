import axios from 'axios';
import {getCurrentUser} from './Authentication.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs a JSON object to the API for storage in DB
 *
 * @param The user details to add to the DB
 * @return Error message or the inserted building ID
 */
export default async function addBuilding(buildingDetails) {
  let user = getCurrentUser();

  //performs the post and returns an error message or the inserted building ID
  const dbCall = await axios.post(API_URL + "building", buildingDetails, {headers : {Authorization : `Bearer ${user.token}`}}).catch(function (error) {
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
