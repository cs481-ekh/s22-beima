import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * GET API call to get a list of devices in the database 
 *
 * @return JSON with list of all devices or error message on failure
 */
const GetDeviceList = async() => {
  const deviceListCall = await axios.get(API_URL + "device_list/").catch(function (error) {
      if (error.response) {
        return error.response;
    }
  });

  const response = {
    status: deviceListCall.status,
    response: deviceListCall.data
  }

  return response;
}

export default GetDeviceList