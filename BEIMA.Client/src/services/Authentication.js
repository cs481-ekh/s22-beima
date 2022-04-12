import axios from 'axios';
import jwt_decode from 'jwt-decode';
import * as Constants from '../Constants.js';
const API_URL = process.env.REACT_APP_API_URL;

const defaultUser = {
  Id : '',
  Token : '',
  Username : '',
  Role : ''
};

/**
 * POSTs a JSON object with user credentials to the login endpoint
 * If the login was successful, then the jwt is saved in the browser storage
 * and the currentUser constant is set
 *
 * @param credentials user details to login
 * @return Error message
 */
export async function login(credentials) {
  const login = await axios.post(API_URL + "login", credentials).catch(function (error){
    if(error.response){
      return error.response;
    }
  });

  if(login.status === Constants.HTTP_SUCCESS){
    let user = jwt_decode(login.data);

    if(credentials.remember){
      localStorage.setItem("currentUser", login.data);
    } else {
      sessionStorage.setItem("currentUser", login.data);
    }

    return user;
  }
  return login.response;
}

/**
 * Remove the token from the browser storage
 * @param {*} remember boolean
 */
export function logout(remember){
  if(remember){
    localStorage.removeItem("currentUser");
  } else {
    sessionStorage.removeItem("currentUser");
  }
  
  return {};
}

/**
 * Gets the JWT from the browser storage and returns the user JSON object
 * associated with the JWT token
 * 
 */
export function getCurrentUser(){
  let token;
  let user = defaultUser;

  if(localStorage.getItem("currentUser")){
    token = localStorage.getItem("currentUser");
  } else if(sessionStorage.getItem("currentUser")){
    token = sessionStorage.getItem("currentUser");
  }

  if(token) {
    user = jwt_decode(token);
    user.token = token;
  }

  console.log(user);
  return user;
}
