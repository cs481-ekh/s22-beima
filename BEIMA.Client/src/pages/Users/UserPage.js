import { useEffect, useState } from "react";
import { useOutletContext, useParams} from 'react-router-dom';
import { Form, Card, Button, FormControl } from "react-bootstrap";
import {ItemCard} from "../../shared/ItemCard/ItemCard"
import styles from './UserPage.module.css'
import * as Constants from '../../Constants';

const UserPage = () => {
  const [setPageName] = useOutletContext();
  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState(null);
  
  useEffect(() => {
    setPageName('View User');
  }, [setPageName])
  
  const { userId } = useParams();
  
  const mockCall = async(userId) => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms));
    await sleep(500);
    const userData = {
      id: "0",
      role: "User's assigned role",
      firstName: "firstName",
      lastName: "lastName",
      username: "userName",
    }
    
    //combine first and last for full name
    userData["name"] = `${userData.firstName} ${userData.lastName}`;
    
    return userData;
  }
  
  useEffect(() => {
    const loadData = async () => {
      const user = await mockCall(userId);
      setUser(user);
      setLoading(false);
    }
   loadData();

  },[userId])
  
   /**
   * Renders an card styled input that lets a user change a field's input
   * 
   * @param editable: can this input be used
   * @param id: id that should be set on the input
   * @param label: label of the input
   * @param value: value of the input
   * @param onChange: function to update value of the field in higher level <RenderItem>
   * @returns 
   */
  const FormCard = ({editable, id, label, value, onChange }) => {
    return (
      <Card>
        <Card.Body >
          <Form.Group className="mb-3" controlId={id}>
            <Form.Label>{label}</Form.Label>
            <FormControl required type="text" disabled={!editable} size="sm" value={value} onChange={onChange} maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH}/>
          </Form.Group>                
        </Card.Body>
      </Card>
    )
  }
  
  /**
   * Renders an input that lets a user change a field's input
   * 
   * @param editable: can this input be used
   * @param id: id that should be set on the input
   * @param label: label of the input
   * @param value: value of the input
   * @param onChange: function to update value of the field in higher level <RenderItem>
   * @returns 
   */
  const FormItem = ({editable, id, label, value, onChange }) => {
    return (
      <Form.Group className="mb-3" controlId={id}>
        <Form.Label><b>{label}</b></Form.Label>
        <Form.Control required type="text" disabled={!editable} size="sm" value={value}  onChange={onChange} maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH}/>
      </Form.Group>
    )
  }

  
  /**
   * Renders a custom form that enables a user
   * to update a user's fields
   * @param user: user json
   * @returns renderable item
   */
  const RenderItem = ({user}) => {
    const [editable, setEditable] = useState(false);

    const [userId] = useState(user.id);
    const [username, setUserName] = useState(user.username);
    const [firstName, setFirstName] = useState(user.firstName);
    const [lastName, setLastName] = useState(user.lastName);
    const [role, setRole] = useState(user.role);

    const updateUserCall = () => {
      const newUser = {
        _id:userId,
        username: username,
        firstName: firstName,
        lastName : lastName,
        role: role,
      }

      // Call Update User
      console.log(newUser);
      setEditable(false)
    }
    
    const deleteUserCall = () => {
      //TODO integrate with styled error/confirm message
      alert("you sure?");
    }

    const cancel = () => {      
      setUserName(user.username);
      setFirstName(user.firstName);
      setLastName(user.firstName);
      setRole(user.role);
      setEditable(false);
    }

    const onChange = (event) => {
      const target = event.target.id
      const value = event.target.value
      if(target === 'userName'){
        setUserName(value);
      } else if (target === 'userFirstName'){
        setFirstName(value);
      }else if (target === 'userLastName'){
        setLastName(value);
      } else if (target === 'userRole'){
        setRole(value);
      }
    }

    return (
      <Form className={styles.form}>
        <Form.Group className="mb-3">
          <Button variant="danger" id="deletebtn" className={styles.deleteButton} onClick={() => deleteUserCall()}>
              Delete User
          </Button>
          {editable ? 
          <div className={styles.buttonRow}>
              <Button id="savebtn" onClick={() => updateUserCall()}>
                Save
              </Button>
              <Button variant="secondary" id="cancelbtn" onClick={cancel}>
                Cancel
              </Button>
          </div>
          : 
            <Button variant="primary" id="editbtn" onClick={() => setEditable(true)}>
              Edit
            </Button>
          }
        </Form.Group>
        
        <div className={[styles.fields,'mb-3'].join(' ')}>
          <FormCard editable={editable} id="userName" label="Username" value={username} onChange={onChange} />
          <FormCard editable={editable} id="userFirstName" label="First Name" value={firstName} onChange={onChange} />
          <FormCard editable={editable} id="userLastName" label="Last Name" value={lastName} onChange={onChange}/>
          <FormCard editable={editable} id="userRole" label="Role" value={role} onChange={onChange}/>
        </div>
  
      </Form>
    )
  }

  return (
    <div className={styles.item} id="userPageContent">
      <ItemCard 
        title={loading ? 'Loading' : `${user.name}`}
        RenderItem={<RenderItem user={user}/>} 
        loading={loading}
        route="/users"
      />
    </div>
  )
}
export default UserPage