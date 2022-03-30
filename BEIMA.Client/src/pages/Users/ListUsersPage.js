import { useOutletContext, useNavigate } from 'react-router-dom';
import { Button, Row, Col } from 'react-bootstrap';
import { useEffect, useState } from "react";
import styles from './ListUsersPage.module.css';
import ItemList from "../../shared/ItemList/ItemList";

const ListUsersPage = () => {
  const [userList, setUserList] = useState([]);
  const [setPageName] = useOutletContext();
  const [loading, setLoading] = useState(true);
  
  let navigate = useNavigate();
  
  useEffect(() => {
    setPageName('List Users')
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
        username: `UserName${i}`,
        role: `User${i}'s assigned role`,
        firstName: `TestFirst`,
        lastName: `TestLast ${i}`,
        lastModified: {
          date: new Date().getDate() + {i},
          user: `UserName #${i - 1}`
        }
      })
    }
    // Map data into format supported by list
    let mapped = data.map(({id, username, role, firstName, lastName, lastModified}) => {
      return {
        id: id,
        name: `${firstName} ${lastName}`,
        username: username,
        role: role,
        firstName: firstName,
        lastName : lastName,
        lastModified : lastModified
      }
    })
    return mapped
  }
  
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
