import { Card, Form, Button, Spinner } from "react-bootstrap";
import { useOutletContext } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { login } from "../../services/Authentication";
import styles from './LoginPage.module.css';
import * as Constants from '../../Constants.js';
import * as Notifications from '../../shared/Notifications/Notification.js';

const LoginPage = () => {
  const [setPageName] = useOutletContext();
  const [username, setUsername] = useState("")
  const [validUsername, setValidUsername] = useState(false)
  const [password, setPassword] = useState("")
  const [validPassword, setValidPassword] = useState(false)
  const [remember, setRemember] = useState(false)
  const [touched, setTouched] = useState(false)
  const [submitting, setSubmitting] = useState(false)

  useEffect(() => {
    setPageName('BEIMA Login')
  }, [setPageName])


  const ValidateUsername = (username) => {
    let valid = true
    if(username.length < 1){
      valid = false
    }
    return valid
  }

  const ValidatePassword = (password) => {
    let valid = true
    if(password.length < 1){
      valid = false
    }
    return valid
  }
  
  const HandleUsername = (event) => {
    let username = event.target.value
    let isValid = ValidateUsername(username)
    setUsername(username)
    setValidUsername(isValid)
  }

  const HandlePassword = (event) => {
    let password = event.target.value
    let isValid = ValidatePassword(password)
    setPassword(password)
    setValidPassword(isValid)
  }

  const HandleRemember = (event) => {
    let val = event.target.value
    if(val === "on"){
      setRemember(true)
    } else {
      setRemember(false)
    }
  }

  const Authenticate = async () => {
    setTouched(true)
    setSubmitting(true)
    if(!validPassword || !validUsername){
      setSubmitting(false)
      return
    }

    const user = {
      username: username,
      password: password,
      remember: remember
    }

    let loginAttempt = await login(user);
    if(loginAttempt.status === Constants.HTTP_SUCCESS){
      window.location.reload(false);
    } else {
      Notifications.error("Login Attempt Failed", `${loginAttempt.response}`);
    }
    setSubmitting(false);
  }

  return (
    <Card className={styles.pageContent}>
      <Card.Body>
        <Form onSubmit={(e) => {e.preventDefault(); Authenticate();}}>
          <Form.Group className="mb-3" controlId="username">
            <Form.Label>Username</Form.Label>
            <Form.Control 
              type="text" 
              required 
              isInvalid={touched && !validUsername}
              placeholder="Enter Username" 
              onChange={HandleUsername}
            />
            <Form.Control.Feedback type="invalid">Invalid Username</Form.Control.Feedback>            
          </Form.Group>

          <Form.Group className="mb-3" controlId="password">
            <Form.Label>Password</Form.Label>
            <Form.Control 
              type="password" 
              required
              isInvalid={touched && !validPassword} 
              placeholder="Password" 
              onChange={HandlePassword}
            />
            <Form.Control.Feedback type="invalid">Invalid Password</Form.Control.Feedback>
          </Form.Group>
          <Form.Group className="mb-3" controlId="rememberMe">
            <Form.Check type="checkbox" label="Remember Me" onChange={HandleRemember} />
          </Form.Group>         
          
          <Button variant="primary" className={styles.orageBtn} disabled={touched && (!validUsername || !validPassword)} type="submit" id="submitBtn">
          {submitting ? <Spinner animation="border" size="sm"/> : "Login"}
          </Button>
                 
        </Form>
      </Card.Body>
    </Card>
  );
}

export default LoginPage
