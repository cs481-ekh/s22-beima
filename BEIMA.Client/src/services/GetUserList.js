import axios from 'axios';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * GET API call to get a list of users in the database are returned
 *
 * @return Error message or a success indicator
 */
export default async function getUserList() {
  const userListCall = await axios.get(API_URL + "user-list").catch(function (error) {
    if (error.response) {
      return error.response;
  }
});

const response = {
  status: userListCall.status,
  response: userListCall.data
}

return response;
}
