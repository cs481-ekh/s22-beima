import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs a JSON object to the API for storage in DB
 *
 * @param The device details to add to the DB
 * @return Error message or the inserted device ID
 */
export default async function addDevice(deviceDetails, photo, files) {
  // setup the multiform request data
  let formData = new FormData();
  formData.append("data", JSON.stringify(deviceDetails));
  formData.append("photo", photo);
  const tempFiles = [...files]

  // add each additional file to the files key
  if(files){
    tempFiles.map((file, index) => {
      formData.append(`files`, file);
      return null;
    });
  }

  //performs the post and returns an error message or the inserted device ID
  const dbCall = await axios.post(API_URL + "device/", formData).catch(function (error) {
      if (error.response) {
        return error.response;
    }
  });

  const response = {
    status: dbCall.status,
    response: dbCall.data
  }

  return response;
}
