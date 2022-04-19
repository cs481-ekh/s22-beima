import { useEffect, useState } from "react";
import { useOutletContext, useParams, useNavigate} from 'react-router-dom';
import { Form, Card, Button, FormControl } from "react-bootstrap";
import {ItemCard} from "../../shared/ItemCard/ItemCard"
import styles from './UserPage.module.css'
import * as Constants from '../../Constants';
import GetUser from '../../services/GetUser.js';
import UpdateUser from '../../services/UpdateUser.js';
import DeleteUser from '../../services/DeleteUser.js';
import * as Notifications from '../../shared/Notifications/Notification.js';
import { getCurrentUser } from "../../services/Authentication";
import FilledDropDown from '../../shared/DropDown/FilledDropDown.js';

const UserPage = () => {
  const availableRoles = [{name: "admin", id: "admin"}, {name: "user", id: "user"}];
  const noRoleObj = { name : 'Select Role' };

  const [setPageName] = useOutletContext();
  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState(null);
  const [userChanged, setUserChanged] = useState(false);
  const [currentUser, setCurrentUser] = useState(null);

  const { id } = useParams();
  
  useEffect(() => {
    setPageName('View User');
    const loadData = async () => {
      const user = (await GetUser(id)).response;
      const currentUser = await getCurrentUser();
      setUser(user);
      setCurrentUser(currentUser);
      setLoading(false);
      setUserChanged(false);
    }
   loadData();

  },[id, userChanged, setPageName])
  
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
* Renders a card that allows for input validation.
* 
* @param editable: can this input be used
* @param id: id that should be set on the input
* @param label: label of the input
* @param value: value of the input
* @param onChange: function to update value of the field in higher level <RenderItem>
* @param hidden: boolean of whether the card should be hidden (default: false)
* @returns 
*/
  const FormCardPassword = ({ editable, id, label, value, onChange, hidden }) => {
    return (
      <Card hidden={hidden}>
        <Card.Body >
          <Form.Group className="mb-3" controlId={id}>
            <Form.Label>{label}</Form.Label>
            <FormControl required type="password" disabled={!editable} size="sm" value={value} onChange={onChange} maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH} />
          </Form.Group>
        </Card.Body>
      </Card>
    )
  }

  /**
   * Renders a card with a dropdown field input. 
   * 
   * @param editable: can this input be used
   * @param id: id that should be set on the input
   * @param label: label of the input
   * @param value: value of the input
   * @param onChange: function to update value of the field in higher level <RenderItem>
   * @returns 
   */
   const FormCardDropdown = ({editable, id, label, dropDownText, items, onChange, buttonStyle }) => {
    return (
      <Card>
        <Card.Body >
          <Form.Group className="mb-3" controlId={id}>
            <Form.Label>{label}</Form.Label>
            <FilledDropDown editable={editable} dropDownText={dropDownText} items={items} selectFunction={onChange} buttonStyle={buttonStyle} dropDownId={"typeDropDown"} />
          </Form.Group>                
        </Card.Body>
      </Card>
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

    const [userId] = useState(user._id);
    const [username, setUsername] = useState(user.username);
    const [firstName, setFirstName] = useState(user.firstName);
    const [lastName, setLastName] = useState(user.lastName);
    const [password, setPassword] = useState("");
    const [passwordConfirm, setPasswordConfirm] = useState("");
    const [role, setRole] = useState(user.role);
    const navigate = useNavigate();

    const [selectedRole, setSelectedRole] = useState(user.role !== "" ? { name : user.role, id: user.role } : noRoleObj);
    const [roleDropDownStyle, setRoleDropDownStyle] = useState(user.role !== "" && user.role !== "Select Role" ? styles.dropDownSelected : styles.button);

    const updateUserCall = async () => {
      const newUser = {
        _id: userId,
        username: username,
        firstName: firstName,
        lastName : lastName,
        password: password,
        role: role,
      }

      // Verify the new password meets requirements
      let requirements = new RegExp(Constants.PASSWORD_REGEX);
      if (password.length !== 0 && (!(requirements.test(password)) || (password.length < 8))){
        Notifications.error("Unable to Update User", "Password does not meet requirements. See Help page for more information.");
        return;
      }

      // Verify the Confirm New Password field matches the New Password field
      if(password !== passwordConfirm){
        Notifications.error("Unable to Update User", "Passwords do not match.");
        return;
      }

      // Call Update User
      let updateResult = await UpdateUser(newUser);
      if(updateResult.status === Constants.HTTP_SUCCESS){
        Notifications.success("Update User Successful", `User ${username} updated successfully.`);
        setEditable(false);
        setLoading(true);
        setUserChanged(true);
      } else {
        Notifications.error("Unable to Update User", `Update of User ${username} failed.`);
      }
    }
    
    const deleteUserCall = async () => {
      let currentUser = getCurrentUser();
      if(currentUser.Id === userId){
        Notifications.error("Unable to Delete User", `Deletion of the current user is not allowed.`);
      } else {
        let deleteNotif = await Notifications.warning("Warning: User Deletion", [`Are you sure you want to delete User ${username}?`]);
        if(deleteNotif.isConfirmed){
          let deleteResult = await DeleteUser(userId);
          if(deleteResult.status === Constants.HTTP_SUCCESS){
            Notifications.success("User Deletion Successful", `User ${username} successfully deleted.`);
            navigate('/users');
          } else if (deleteResult.status === Constants.HTTP_CONFLICT_RESULT){
            Notifications.error("Unable to Delete User", `${deleteResult.response}`);
          } else {
            Notifications.error("Unable to Delete User", `Deletion of User ${username} failed.`);
          }
        }
      }
    }

    const cancel = () => {      
      setUsername(user.username);
      setFirstName(user.firstName);
      setLastName(user.firstName);
      setPassword("");
      setPasswordConfirm("");
      setRole(user.role);
      changeSelectedRole(user.role);
      setRoleDropDownStyle(user.role !== "" && user.role !== "Select Role" ? styles.dropDownSelected : styles.button)
      setEditable(false);
    }

    const onChange = (event) => {
      const target = event.target.id
      const value = event.target.value
      if(target === 'userName'){
        setUsername(value);
      } else if (target === 'userFirstName'){
        setFirstName(value);
      }else if (target === 'userLastName'){
        setLastName(value);
      } else if(target === "userPassword"){
        setPassword(value);
      } else if(target === "userPasswordConfirm"){
        setPasswordConfirm(value);
      }
    }

    /*
    * sets the state for the selected building from the dropdown
    */
    function changeSelectedRole(roleId) {
      let role = availableRoles.find(role => {
        return role.id === roleId;
      })
      if(roleId === "" || roleId === "Select Role" || roleId === null){
        role = noRoleObj;
        setRoleDropDownStyle(styles.button);
      } else {
        setRoleDropDownStyle(styles.dropDownSelected)
      }
      setSelectedRole(role);
      setRole(role.id);
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
          <FormCard editable={editable} id="userLastName" label="Last Name" value={lastName} onChange={onChange} />
          <FormCardPassword editable={editable} id="userPassword" label="New Password" value={password} onChange={onChange} hidden={currentUser.Role !== "admin" || !editable} />
          <FormCardPassword editable={editable} id="userPasswordConfirm" label="Confirm New Password" value={passwordConfirm} onChange={onChange} hidden={currentUser.Role !== "admin" || !editable} />
          <FormCardDropdown editable={editable} id="userRole" label="Role" dropDownText={selectedRole.name} items={availableRoles} onChange={changeSelectedRole} buttonStyle={roleDropDownStyle}></FormCardDropdown>
        </div>
  
      </Form>
    )
  }

  return (
    <div className={styles.item} id="userPageContent">
      <ItemCard 
        title={loading ? 'Loading' : `${user.firstName} ${user.lastName}`}
        RenderItem={<RenderItem user={user}/>} 
        loading={loading}
        route="/users"
      />
    </div>
  )
}
export default UserPage