import axios from 'axios';
import {getCurrentUser, logout} from './Authentication.js';
import * as Constants from '../Constants.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs a JSON object to the database to be updated
 *
 * @param The JSON of the device to be updated
 * @return Error message or a succes indicator
 */
export default async function updateDevice(deviceDetails, photo, files) {
  let user = getCurrentUser();

  // setup the multiform request data
  let formData = new FormData();
  formData.append("data", JSON.stringify(deviceDetails));
  formData.append("photo", photo);
  const tempFiles = [...files]

  // add each additional file to the files key
  if(files){
    tempFiles.map((file) => {
      formData.append(`files`, file);
      return null;
    });
  }

  //performs the post and returns an error message or a succes indicator
  const dbCall = await axios.post(API_URL + "device/" + deviceDetails._id + "/update", formData, {headers : {Authorization : `Bearer ${user.token}`}}).catch(function (error) {
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