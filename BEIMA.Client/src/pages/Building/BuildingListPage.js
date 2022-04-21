import { useEffect, useState } from "react"
import { Button, Row, Col} from 'react-bootstrap';
import styles from  "./BuildingList.module.css"
import ItemList from "../../shared/ItemList/ItemList";
import { useOutletContext, useNavigate } from 'react-router-dom';
import GetBuildingList from '../../services/GetBuildingList.js';

const AddBuildingButton = () => {
  let navigate = useNavigate();
  return (
    <Button variant="primary" type="button" className={styles.addButton} id="addNewBuilding" onClick={() => navigate(`addBuilding`)}>
      Add New Building
    </Button>
  )
}

const BuildingListPage = () => {
  const [buildings, setBuildings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [setPageName] = useOutletContext();

  useEffect(() => {
    const loadData = async () => {
      setLoading(true)
      let buildings = (await GetBuildingList()).response;
      setLoading(false)
      setBuildings(buildings)
    }
    loadData()
    setPageName('Buildings')
  },[setPageName])
  

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
         
        </Col>
      </Row>
      <br/>
      <ItemList list={buildings} RenderItem={RenderItem} loading={loading} addButton={<AddBuildingButton/>}/>
    </div>
  )
}

export default BuildingListPage