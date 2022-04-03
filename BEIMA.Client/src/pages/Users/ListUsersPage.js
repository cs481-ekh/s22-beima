import { useOutletContext, useNavigate } from 'react-router-dom';
import { Button, Row, Col } from 'react-bootstrap';
import { useEffect, useState } from "react";
import styles from './ListUsersPage.module.css';
import ItemList from "../../shared/ItemList/ItemList";
import GetUserList from '../../services/GetUserList.js';

const ListUsersPage = () => {
  const [userList, setUserList] = useState([]);
  const [setPageName] = useOutletContext();
  const [loading, setLoading] = useState(true);
  
  let navigate = useNavigate();
  
  useEffect(() => {
    setPageName('List Users')
    const loadData = async () => {
      setLoading(true)
      let users = (await GetUserList()).response;
      setLoading(false)
      setUserList(users)
    }
   loadData()
  },[setPageName])
  
  /**
   * Renders a custom description of the item's fields
   * @param item: json item 
   * @returns html
   */
  const RenderItem = (item) => {
    return (
      <div className={styles.details}>
        <div>Username: {item.username}</div>
        <div>Role: {item.role}</div> 
      </div>
    )
  }
  
  return (
    <div className={styles.fieldform} id="userListContent">
      <Row>
        <Col>
          <Button variant="primary" type="button" className={styles.addButton} id="addNewUser" onClick={() => navigate(`addUser`)}>
            Add New User
          </Button>
        </Col>
      </Row>
      <br/>
      <ItemList list={userList} RenderItem={RenderItem} loading={loading}/>
    </div>
  )
}
export default ListUsersPage
