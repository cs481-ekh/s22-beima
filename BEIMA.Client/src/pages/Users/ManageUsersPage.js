import { useOutletContext } from 'react-router-dom';
import { Card, Button, Row, Col, Form } from 'react-bootstrap';
import { useEffect, useState } from "react";
import styles from './ManageUsersPage.module.css';
import FilledDropDown from '../../shared/DropDown/FilledDropDown.js';
import FormListWithErrorFeedback from '../../shared/FormList/FormListWithErrorFeedback.js';


const ManageUserPage = () => {
  const mandatoryUserFields = {
    "Username": "",
    "Role": "",
    "First Name": "",
    "Last Name": ""
  }
  
  const [userFields, setUserFields] = useState({});
  const [errors, setErrors] = useState({});
  const [userList, setUserList] = useState([]);
  const [setPageName] = useOutletContext();
  const [loading, setLoading] = useState(true);
  const [selectedUser, setSelectedUser] = useState({});
  const [selectedUserName, setSelectedUserName] = useState('Select User');
  const [ddStyle, setDdStyle] = useState(styles.button);
  const [formIsHidden, setFormIsHidden] = useState(true);
  const [currentUserOperation, setCurrentUserOperation] = useState();

  
  useEffect(() => {
    setPageName('Manage Users')
  }, [setPageName])

  useEffect(() => {
    const loadData = async () => {
      setLoading(true)
      let users = await mockCall()
      setLoading(false)
      setUserList(users)
    }
   loadData()
  },[])
  
  const mockCall = async () => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(1000)
    let data = []
    for(let i = 0; i < 5; i++){
      data.push({
        id: 74+i,
        name: `User #${i}`,
        username: `UserName # #${i}`,
        firstName: `User #${i} First`,
        lastName: `User #${i} Last`,
        lastModified: {
          date: new Date().getDate() + {i},
          user: `UserName #${i - 1}`
        }
      })
    }
    // Map data into format supported by list
    let mapped = data.map(({id, name, username, firstName, lastName, lastModified}) => {
      return {
        id: id,
        name: name,
        username: username,
        firstName: firstName,
        lastName : lastName,
        lastModified : lastModified
      }
    })
    return mapped
  }

  function setUserDetails(userId) {
    for(let i = 0; i < userList.length; i++) {
      if(userList[i].id == userId){
        setSelectedUser(userList[i]);
        setSelectedUserName(userList[i].name);
      }
    }
    setDdStyle(styles.ddSelected);
  }

  function addUser() {
    setUserFields(mandatoryUserFields);
    setCurrentUserOperation("Add");
    setFormIsHidden(false);
  }

  function updateUser() {
    //make sure a user is selected
    //get user details
    //populate fields setUserFields(result)
    setUserFields(mandatoryUserFields);
    setCurrentUserOperation("Update");
    setFormIsHidden(false);
  }

  function deleteUser() {
    //make sure a user is selected
    //get user's details
    //confirm delete?
    alert('You sure?');
    //delete user
  }
  
  function saveUser() {
    //save user details back to db
    
    setSelectedUserName('Select User');
    setDdStyle(styles.button);
    setFormIsHidden(true);
    
  }
  
  return (
    <div className={styles.fieldform}>
       <Card>
          <Card.Body>
             <Form>
                <Row>
                  <h4>Change Existing User</h4>
                </Row>
                <Row>
                  <Col>
                    <FilledDropDown dropDownText={selectedUserName} items={userList} selectFunction={setUserDetails} buttonStyle={ddStyle} dropDownId={"userDropDown"} />
                  </Col>
                </Row>
                <br />
                <Row className={styles.buttonGroup}>
                  <Col>
                    <Button variant="primary" type="button" className={styles.button} id="updateUser" onClick={(event) => updateUser(event)}>
                      Update User
                    </Button>
                  </Col>
                  <Col>
                    <Button variant="primary" type="button" className={styles.button} id="deleteUser" onClick={(event) => deleteUser(event)}>
                      Delete User
                    </Button>
                  </Col>
                </Row>
                <br/>
                <h4>Add New User</h4>
                <div>
                   <Button variant="primary" type="button" className={styles.button} id="addUser" onClick={(event) => addUser(event)}>
                   Add User
                   </Button>
                </div>
             </Form>
          </Card.Body>
       </Card>
       <br />
      {formIsHidden ? "" : 
        <Card>
          <Card.Body>
            <Form>
              <h4>{currentUserOperation} User Details</h4>
              <FormListWithErrorFeedback fields={Object.keys(userFields)} errors={errors} />
            </Form>
            <br />
            <div>
              <Button variant="primary" type="button" className={styles.button} id="saveUser" onClick={(event) => saveUser(event)}>
                Save User
              </Button>
            </div>
          </Card.Body>
        </Card>
      }
    </div>
  )
}
export default ManageUserPage
