import axios from 'axios';
import * as Constants from '../Constants.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * GET API call to get a report of all devices in the database
 *
 * @return the report in the form of bytes
 */
const GetAllDeviceDevicesReport = async() => {
  const deviceReportCall = await axios.get(API_URL + "report/devices").catch(function (error) {
      if (error.response) {
        return error.response;
    }
  });

  if (deviceReportCall.status !== Constants.HTTP_SUCCESS){
    return deviceReportCall.status;
  }

  var blob = new Blob([deviceReportCall.response]);
  var link = document.createElement('a');
  link.href = window.URL.createObjectURL(blob);
  link.download = "All devices";
  link.click();

  console.log("end of report service function")
}

export default GetAllDeviceDevicesReport