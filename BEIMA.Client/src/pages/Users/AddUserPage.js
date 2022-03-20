import styles from './AddUserPage.module.css';
import { useOutletContext, Link } from 'react-router-dom';
import { IoArrowBack } from "react-icons/io5";
import { Card, Button, Form, Row, Col } from 'react-bootstrap';
import FormListWithErrorFeedback from '../../shared/FormList/FormListWithErrorFeedback.js';
import { useEffect, useState } from "react";

const AddUserPage = () => {
  const mandatoryUserFields = {
    "Username" : "",
    "Password" : "",
    "Password Confirmation" : "",
    "First Name" : "",
    "Last Name" : "",
    "Role" : ""
  }
  
  const [userFields, setUserFields] = useState(mandatoryUserFields);
  const [setPageName] = useOutletContext();
  const [fullUserJSON, setFullUserJSON] = useState({});
  const [errors, setErrors] = useState({});
  
  useEffect(() => {
    setPageName('Add User')
  }, [setPageName])
  
  /*
  * updates the values in state when the user types
  * @param inputEvent the even fired when the user types
  */
  function updateFieldState(inputEvent){
    userFields[inputEvent.target.name] = inputEvent.target.value;
    setUserFields(userFields);
  }
  
  function checkPassword(){
    let passwordErrors = {};
    
    if(userFields["Password"] === '') {
      passwordErrors["Password"] = 'Pasword cannot be empty';
    }

    if(userFields["Password Confirmation"] === '') {
      passwordErrors["Password Confirmation"] = 'Pasword Confirmation cannot be empty';
    }
    
    if((!('Password' in passwordErrors) && !('Password Confirmation' in passwordErrors)) &&
       (userFields["Password"] !== userFields["Password Confirmation"])
      ) {
         passwordErrors["Password"] = 'Paswords do not match';
         passwordErrors["Password Confirmation"] = 'Paswords do not match';
    }
    
    return passwordErrors;
  }
  
  
  // gathers all the input and puts it into JSON
  function createJSON(addButtonEvent){
    let formFields = addButtonEvent.target.form.elements;
    let fieldValues = {};

    for(let i = 0; i < formFields.length; i++){
      let formName = formFields[i].name;
      let fieldNames = Object.keys(userFields);
      
      if(fieldNames.includes(formName)){
        let formJSON =  {[formName] : formFields[i].value};
        
        Object.assign(fieldValues, formJSON);
      }
    }
    
    let newErrors = checkPassword();
    
    //display errors when present or attempt insert when valid data is present
    if ( Object.keys(newErrors).length > 0 ) {
      setErrors(newErrors);
    } else {
      setFullUserJSON(fieldValues);
      
      
      // TODO replace with user api calls when ready
      console.log(fullUserJSON);
      
      
      
      for(let i = 0; i < formFields.length; i++){
        formFields[i].value = "";
      }
      setErrors({});
    }
  }
  
  return (
    <div className={styles.fieldform}>
      <Card>
        <Card.Body>
          <Form>
            <Row>
             <Col>
               <Link to="/users" className={styles.back} id="backUsers">
                 <IoArrowBack color='#fff' size={20} />
               </Link>
             </Col>
             <Col>
               <Button variant="primary" type="button" className={styles.addButton} id="addUser" onClick={(event) => createJSON(event)}>
                 Add User
               </Button>
             </Col>
            </Row>
            <br/>
            <h4>User Fields</h4>
            <div>
              <FormListWithErrorFeedback fields={Object.keys(userFields)} errors={errors} changeHandler={updateFieldState}/>
            </div>
         </Form>
        </Card.Body>
      </Card>
    </div>
  )
}
export default AddUserPage