import axios from 'axios';
import {getCurrentUser} from './Authentication.js';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * GET API call to get a list of users in the database are returned
 *
 * @return Error message or a success indicator
 */
export default async function getUserList() {
  let user = getCurrentUser();

  const userListCall = await axios.get(API_URL + "user-list", {headers : {Authorization : `Bearer ${user.token}`}}).catch(function (error) {
    if (error.response) {
      return error.response;
  }
});

let combinedFields = userListCall.data.map(user => {
  return {
    id: user._id,
    name : `${user.firstName} ${user.lastName}`,
    username : user.username,
    role : user.role,
    lastModified: user.lastModified
  }
})

const response = {
  status: userListCall.status,
  response: combinedFields
}

return response;
}
