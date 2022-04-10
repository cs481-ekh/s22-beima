import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs a JSON object to the database to be updated
 *
 * @param The JSON of the building to be updated
 * @return Error message or a success indicator
 */
export default async function updateBuilding(buildingDetails, token) {
  //performs the post and returns an error message or a success indicator
  const dbCall = await axios.post(API_URL + "building/" + buildingDetails._id + "/update", buildingDetails, {headers : {Authorization : `Bearer ${token}`}}).catch(function (error) {
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