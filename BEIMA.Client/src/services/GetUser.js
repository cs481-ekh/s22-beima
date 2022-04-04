import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * GETs a JSON object to the API for storage in DB
 *
 * @param The user ID
 * @return Error message or the user details
 */
export default async function getUser(userID) {
  //performs the get and returns an error message or the user details
  const dbCall = await axios.post(API_URL + "user/" + userID).catch(function (error) {
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
