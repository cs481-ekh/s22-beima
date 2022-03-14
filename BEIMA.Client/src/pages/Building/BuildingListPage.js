import { useEffect, useState } from "react"
import { Button, Row, Col} from 'react-bootstrap';
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
        number: "2987",
        notes: 'Somewhere on campus',
      })
    }
    // Map data into format supported by list
    let mapped = data.map(item => {
      return {
        id: item._id,
        name: item.name,
        number: item.number,
        notes: item.notes,
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
    return (
      <div className={styles.details}>
        <div>Number: {item.number}</div>
        <div>Notes: {item.notes}</div> 
      </div>
    )
  }

  return (
    <div className={styles.fieldform} id="buildingListContent">
      <Row>
        <Col>
          <Button variant="primary" type="button" className={styles.addButton} id="addNewBuilding" onClick={() => navigate(`addBuilding`)}>
            Add New Building
          </Button>
        </Col>
      </Row>
      <br/>
      <ItemList list={buildings} RenderItem={RenderItem} loading={loading}/>
    </div>
  )
}

export default BuildingListPage