import axios from 'axios';
import {getCurrentUser, logout} from './Authentication.js';
import * as Constants from '../Constants.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * GET API call to get a report of all devices in the database
 *
 * @return the report in the form of bytes
 */
const GetAllDeviceDevicesReport = async() => {
  let user = getCurrentUser();

  const deviceReportCall = await axios.get(API_URL + "report/devices", {headers : {Authorization : `Bearer ${user.token}`}, responseType: 'blob'}).catch(function (error) {
      if (error.response) {
        return error.response;
    }
  });

  if(deviceReportCall.status === Constants.HTTP_UNAUTH_RESULT){
    logout();
    return;
  }
  
  if (deviceReportCall.status !== Constants.HTTP_SUCCESS){
    const res = {
      status: deviceReportCall.status
    }
    return res
  }

  const blob = new Blob([deviceReportCall.data], {type: 'application/zip'})
  const url = window.URL.createObjectURL(blob)
  const link = document.createElement('a');
  link.href = url;
  link.download = "All Devices"
  document.body.appendChild(link);
  link.click()

  const res = {
    status: Constants.HTTP_SUCCESS
  }
  return res
}

export default GetAllDeviceDevicesReport