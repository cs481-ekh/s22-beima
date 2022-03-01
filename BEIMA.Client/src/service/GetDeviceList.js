import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * GET API call to get a list of devices in the database and filters which fields are returned
 *
 * @return JSON with list of all devices or error message on failure
 */
const GetDeviceList = async() => {
  const deviceListCall = await axios.get("http://localhost:7071/api/device_list").catch(function (error) {
      if (error.response) {
        return error.response;
    }
  });

  let filteredFields = deviceListCall.data.map((item) => {
    return {
      id: item._id,
      deviceType: item.deviceTypeId,
      buildingId: item.location.buildingId,
      lastModified: item.lastModified.date,
      deviceTag: item.deviceTag,
      serialNumber: item.serialNum,
      manufacturer: item.manufacturer,
      notes: item.notes
    }
  })

  const response = {
    status: deviceListCall.status,
    response: filteredFields
  }

  return response;
}

export default GetDeviceList