import axios from 'axios';
import jwt_decode from 'jwt-decode';
const API_URL = process.env.REACT_APP_API_URL;

/**
 * POSTs a JSON object with user credentials to the login endpoint
 * If the login was successful, then the jwt is saved in the browser storage
 * and the currentUser constant is set
 *
 * @param credentials user details to login
 * @return Error message
 */
export default async function login(credentials) {
  const login = await axios.post(API_URL + "login", credentials).catch(function (error){
    if(error.response){
      return error.response;
    }
  });

    jwt_decode(login.response).then(decoded => {
      const user = {
        id : decoded._id,
        token : login.response,
        username : decoded.username,
        firstName : decoded.firstName,
        lastName : decoded.lastName,
        role : decoded.role,
        remember : credentials.remember
      };

      if(credentials.remember){
        localStorage.setItem("currentUser", login.response);
      } else {
        sessionStorage.setItem("currentUser", login.response);
      }

      return user;
    });
  
    return login.response;
}

/**
 * Remove the token from the browser storage
 * @param {*} remember boolean
 */
export default function logout(remember){
  if(remember){
    localStorage.removeItem("currentUser");
  } else {
    sessionStorage.removeItem("currentUser");
  }
  
  return {};
}

/**
 * Checks if the token is in localStorage and sets the currentUser constant
 * This is used for if a user selects remember me, but closes their browser
 * before the timeout occurs
 * 
 */
export default function checkAndSetCurrentUser(){
  let token = localStorage.getItem("currentUser");

  if(token) {
    jwt_decode(token).then(decoded => {
      const user = {
        id : decoded._id,
        token : login.response,
        username : decoded.username,
        firstName : decoded.firstName,
        lastName : decoded.lastName,
        role : decoded.role,
        remember : credentials.remember
      };

      return user;
    });
  }
}
