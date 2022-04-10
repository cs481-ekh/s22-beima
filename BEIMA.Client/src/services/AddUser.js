import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs a JSON object to the API for storage in DB
 *
 * @param The user details to add to the DB
 * @return Error message or the inserted user ID
 */
export default async function addUser(userDetails, token) {
  //performs the post and returns an error message or the inserted user ID
  const dbCall = await axios.post(API_URL + "user", userDetails, {headers : {Authorization : `Bearer ${token}`}}).catch(function (error) {
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
