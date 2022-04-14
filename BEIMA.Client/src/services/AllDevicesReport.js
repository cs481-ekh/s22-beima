import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * GET API call to get a report of all devices in the database
 *
 * @return the report in the form of bytes
 */
const GetAllDeviceDevicesReport = async() => {
  const deviceReportCall = await axios.get(API_URL + "device-list").catch(function (error) {
      if (error.response) {
        return error.response;
    }
  });

  const response = {
    status: deviceReportCall.status,
    response: deviceReportCall.data
  }

  return response;
}

export default GetAllDeviceDevicesReport