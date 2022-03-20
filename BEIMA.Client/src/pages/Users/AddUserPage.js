import styles from './AddUserPage.module.css';
import { useOutletContext, Link } from 'react-router-dom';
import { IoArrowBack } from "react-icons/io5";
import { Card, Button, Form, Row, Col } from 'react-bootstrap';
import FormList from '../../shared/FormList/FormList.js';
import { useEffect, useState } from "react";

const AddUserPage = () => {
  const mandatoryUserFields = {
    "Username" : "",
    "First Name" : "",
    "Last Name" : "",
    "Role" : ""
  }
  
  const [userFields] = useState(mandatoryUserFields);
  const [setPageName] = useOutletContext();
  const [fullUserJSON, setFullUserJSON] = useState({});
  
  useEffect(() => {
    setPageName('Add User')
  }, [setPageName])
  
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
        formFields[i].value = "";
      }
    }

    setFullUserJSON(fieldValues);
    
    // replace with user api calls when ready
    console.log(fullUserJSON);
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
              <FormList fields={Object.keys(mandatoryUserFields)} />
            </div>
         </Form>
        </Card.Body>
      </Card>
    </div>
  )
}
export default AddUserPage