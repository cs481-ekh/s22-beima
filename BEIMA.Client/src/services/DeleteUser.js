import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs an ObjectId for the API to delete
 *
 * @param The ID of the user to delete
 * @return Error message or a success indicator
 */
export default async function deleteUser(userId) {
  //performs the post and returns an error message or a success indicator
  const dbCall = await axios.post(API_URL + "user/" + userId + "/delete").catch(function (error) {
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
