import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs a JSON object to the database to be updated
 *
 * @param The JSON of the user to be updated
 * @return Error message or a success indicator
 */
export default async function updateUser(userDetails) {
  //performs the post and returns an error message or a success indicator
  const dbCall = await axios.post(API_URL + "user/" + userDetails._id + "/update", userDetails).catch(function (error) {
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