import axios from 'axios';
import * as Constants from '../Constants.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * GET API call to get a report of all devices in the database
 *
 * @return the report in the form of bytes
 */
const GetAllDeviceDevicesReport = async() => {
  const deviceReportCall = await axios.get(API_URL + "report/devices", {responseType: 'blob'}).catch(function (error) {
      if (error.response) {
        return error.response;
    }
  });

  if (deviceReportCall.status !== Constants.HTTP_SUCCESS){
    return deviceReportCall.status;
  }

  const blob = new Blob([deviceReportCall.data], {type: 'application/zip'})
  const url = window.URL.createObjectURL(blob)
  const link = document.createElement('a');
  link.href = url;
  link.download = "All Devices"
  document.body.appendChild(link);
  link.click()
}

export default GetAllDeviceDevicesReport