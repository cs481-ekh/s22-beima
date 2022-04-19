import axios from 'axios';
import {getCurrentUser, logout} from './Authentication.js';
import * as Constants from '../Constants.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * GET API call to get a list of devices in the database and filters which fields are returned
 *
 * @return JSON with list of all devices or error message on failure
 */
const GetDeviceList = async(deviceTypeParams = [], buildingParams = []) => {
  let user = getCurrentUser();
  
  let queryString = "?"
  deviceTypeParams.forEach(val => {
    queryString += `deviceType=${val}&`
  })

  buildingParams.forEach(val => {
    queryString += `building=${val}&`
  })

  const deviceListCall = await axios.get(API_URL + "device-list" + queryString, {headers : {Authorization : `Bearer ${user.token}`}}).catch(function (error) {
      if (error.response) {
        return error.response;
    }
  });

  if(deviceListCall.status === Constants.HTTP_UNAUTH_RESULT){
    logout();
    return;
  }

  let filteredFields = deviceListCall.data.map((item) => {
    return {
      id: item._id,
      deviceType: item.deviceTypeId,
      buildingId: item.location.buildingId,
      lastModified: item.lastModified.date,
      deviceTag: item.deviceTag,
      serialNumber: item.serialNum,
      manufacturer: item.manufacturer,
      notes: item.notes,
      latitude: item.location.latitude,
      longitude: item.location.longitude
    }
  })

  const response = {
    status: deviceListCall.status,
    response: filteredFields
  }

  return response;
}

export default GetDeviceList