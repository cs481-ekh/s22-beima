import styles from './AddUserPage.module.css';
import { useOutletContext, Link } from 'react-router-dom';
import { IoArrowBack } from "react-icons/io5";
import { Card, Button, Form, Row, Col } from 'react-bootstrap';
import FormListWithErrorFeedback from '../../shared/FormList/FormListWithErrorFeedback.js';
import FilledDropDown from '../../shared/DropDown/FilledDropDown.js';
import { useEffect, useState } from "react";
import AddUser from '../../services/AddUser.js';
import * as Constants from '../../Constants.js';
import * as Notifications from '../../shared/Notifications/Notification.js';

const AddUserPage = () => {
  const mandatoryUserFields = {
    "Username" : "",
    "Password" : "",
    "Password Confirmation" : "",
    "First Name" : "",
    "Last Name" : ""
  }

  const availableRoles = [{name: "admin", id: "admin"}, {name: "user", id: "user"}];
  const noRoleObj = { name : 'Select Role' };
  
  const [userFields, setUserFields] = useState(mandatoryUserFields);
  const [selectedRole, setSelectedRole] = useState(noRoleObj);
  const [roleDropDownStyle, setRoleDropDownStyle] = useState(styles.button);
  const [setPageName] = useOutletContext();
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
    let requirements = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[-+_!@#$%^&*.,?]).+$");
    
    if(userFields["Password"] === '') {
      passwordErrors["Password"] = 'Password cannot be empty';
    } else if (!(requirements.test(userFields["Password"])) || userFields["Password"].length < 8){
      passwordErrors["Password"] = 'Password does not meet requirements. See Help page for more information.';
    }

    if(userFields["Password Confirmation"] === '') {
      passwordErrors["Password Confirmation"] = 'Password Confirmation cannot be empty';
    } else if (!(requirements.test(userFields["Password Confirmation"]))){
      passwordErrors["Password Confirmation"] = 'Password does not meet requirements. See Help page for more information.';
    }
    
    if((!('Password' in passwordErrors) && !('Password Confirmation' in passwordErrors)) &&
       (userFields["Password"] !== userFields["Password Confirmation"])
      ) {
         passwordErrors["Password"] = 'Passwords do not match';
         passwordErrors["Password Confirmation"] = 'Passwords do not match';
    }
    
    return passwordErrors;
  }

    /*
  * converts the user friendly field name to the form needed by the database
  * in cases where the name needed is another key it will retrieve it from stored values
  * @param the form element's name
  * @return either the converted string or an object to get the converted string/stored value
  */
    function convertToDbFriendly(formName) {
      let result = {};
      
      //make first letter lower case
      let dbKey = formName[0].toLowerCase() + formName.slice(1);
      //Number needs to be Num where applicable
      dbKey = dbKey.replace('Number', 'Num');
      //remove spaces
      dbKey = dbKey.replace(/\s+/g, '');
      result = dbKey;
      
      return result;
    }
    
  
  // gathers all the input and puts it into JSON
  function createJSON(formFields){
    let fieldValues = {};
    
    let newErrors = checkPassword();
    
    for(let i = 0; i < formFields.length; i++){
      let formName = formFields[i].name;
      let fieldNames = Object.keys(userFields);
      
      if(fieldNames.includes(formName)){
        let jsonKey = convertToDbFriendly(formName);
        if(formFields[i].value === '') {
          newErrors[formName] = `${formName} cannot be blank`;
        }
        
        let formJSON =  {[jsonKey] : formFields[i].value};
        
        Object.assign(fieldValues, formJSON);
      }
    }
    
    //display errors when present or attempt insert when valid data is present
    if ( Object.keys(newErrors).length > 0 ) {
      setErrors(newErrors);
    } else {
      fieldValues.role = selectedRole.name;
      setErrors({});
      return fieldValues;
    }
  }

  async function saveUserToDb(addButtonEvent){
    let formFields = addButtonEvent.target.form.elements;
    let fullJSON = createJSON(formFields);
    
    if(fullJSON && Object.keys(errors).length === 0){
      AddUser(fullJSON).then(response => {
        if(response.status === Constants.HTTP_SUCCESS){
          for(let i = 0; i < formFields.length; i++){
            formFields[i].value = "";
          }
          setUserFields(mandatoryUserFields);
          setSelectedRole(noRoleObj);
          Notifications.success("Add User Successful", "Adding User completed successfully.");
        } else {
          Notifications.error("Unable to Add User", "Adding User failed.");
        }
      })
    }
  }

  /*
  * sets the state for the selected building from the dropdown
  */
  function changeSelectedRole(roleId) {
    let role = availableRoles.find(role => {
      return role.id === roleId;
    })
    setSelectedRole(role);
    setRoleDropDownStyle(styles.dropDownSelected);
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
               <Button variant="primary" type="button" className={styles.addButton} id="addUser" onClick={(event) => saveUserToDb(event)}>
                 Add User
               </Button>
             </Col>
            </Row>
            <br/>
            <h4>User Fields</h4>
            <div>
              <FormListWithErrorFeedback fields={Object.keys(userFields)} errors={errors} changeHandler={updateFieldState}/>
              <label>Role</label>
              <FilledDropDown dropDownText={selectedRole.name} items={availableRoles} selectFunction={changeSelectedRole} buttonStyle={roleDropDownStyle} dropDownId={"typeDropDown"} />
            </div>
         </Form>
        </Card.Body>
      </Card>
    </div>
  )
}
export default AddUserPage