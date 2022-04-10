import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * GET API call to get a list of device types in the database and return only basic fields
 *
 * @return JSON with list of all devices or error message on failure
 */
const GetDeviceTypeList = async(token) => {
  const deviceTypeListCall = await axios.get(API_URL + "device-type-list", {headers : {Authorization : `Bearer ${token}`}}).catch(function (error) {
      if (error.response) {
        return error.response;
    }
  });

  let filteredFields = deviceTypeListCall.data.map((item) => {
    return {
      id: item._id,
      name: item.name,
      description: item.description,
      notes: item.notes,
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