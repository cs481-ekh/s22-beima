import { useEffect, useState } from "react"
import { Card, Button, Form } from 'react-bootstrap';
import styles from  "./BuildingList.module.css"
import ItemList from "../../shared/ItemList/ItemList";
import { useOutletContext, useNavigate } from 'react-router-dom';

const BuildingListPage = () => {
  const [buildings, setBuildings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [setPageName] = useOutletContext();
  let navigate = useNavigate();

  useEffect(() => {
    setPageName('Buildings')
  },[setPageName])
  
  const mockCall = async () => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(1000)
    let data = []
    for(let i = 0; i < 5; i++){
      data.push({
        _id: i,
        name: `Test Building #${i}`,
        notes: 'Somewhere on campus'
      })
    }
    // Map data into format supported by list
    let mapped = data.map(item => {
      return {
        id: item._id,
        name: item.name,
        notes: item.notes
      }
    })
    return mapped
  }

  useEffect(() => {
    const loadData = async () => {
      setLoading(true)
      let buildings = await mockCall()
      setLoading(false)
      setBuildings(buildings)
    }
    loadData()
  },[])

  /**
   * Renders a custom description of the item's fields
   * @param item: json item 
   * @returns html
   */
  const RenderItem = (item) => {
    // add these back to the returned object when we can call to the buildings and device type DBs
    // <div>Location: {item.buildingName}</div>
    //<div>Device Type: {item.deviceType}</div> 
    return (
      <div className={styles.details}>
        <div>Notes: {item.notes}</div> 
      </div>
    )
  }

  return (
    <div className={styles.fieldform}>
      <Card>
        <Button variant="primary" type="button" className={styles.addButton} id="addNewBuilding" onClick={() => navigate(`addBuilding`)}>
          Add Building
        </Button>
        <ItemList list={buildings} RenderItem={RenderItem} loading={loading}/>
      </Card>
    </div>
  )
}

export default BuildingListPage