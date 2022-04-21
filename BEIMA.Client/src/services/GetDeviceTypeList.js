import axios from 'axios';
import {getCurrentUser, logout} from './Authentication.js';
import * as Constants from '../Constants.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * GET API call to get a list of device types in the database and return only basic fields
 *
 * @return JSON with list of all devices or error message on failure
 */
const GetDeviceTypeList = async() => {
  let user = getCurrentUser();

  const deviceTypeListCall = await axios.get(API_URL + "device-type-list", {headers : {Authorization : `Bearer ${user.token}`}}).catch(function (error) {
      if (error.response) {
        return error.response;
    }
  });

  if(deviceTypeListCall.status === Constants.HTTP_UNAUTH_RESULT){
    logout();
    return;
  }

  let filteredFields = deviceTypeListCall.data.map((item) => {
    return {
      id: item._id,
      name: item.name,
      description: item.description,
      notes: item.notes,
      numDevices : item.count,
      lastModified: item.lastModified.date
    }
  })

  const response = {
    status: deviceTypeListCall.status,
    response: filteredFields
  }

  return response;
}

export default GetDeviceTypeList