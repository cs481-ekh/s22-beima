import { useOutletContext, useParams, useNavigate } from 'react-router-dom';
import { useEffect, useState, useRef } from "react"
import {ItemCard} from "../../shared/ItemCard/ItemCard"
import styles from './BuildingPage.module.css'
import { Form, Card, Button, FormControl, Image} from "react-bootstrap";
import { TiDelete } from "react-icons/ti";
import * as Constants from '../../Constants';

const BuildingPage = () => {
  const [setPageName] = useOutletContext();
  const [loading, setLoading] = useState(true);
  const [building, setBuilding] = useState(null);

  useEffect(() => {
    setPageName('View Building')
  },[setPageName])

  const { id } = useParams();

  const mockBuildingCall = async(id) => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(500)
    const building = {
      id: "5",
      name: `Test Building #5`,
      number: "2987",
      latitude: "23.352",
      longitude: "95.2973",
      notes: 'Somewhere on campus'
    }
    return building
  }
  
  useEffect(() => {
    const loadData = async() => {
      const building = await mockBuildingCall(id)
  
      setBuilding(building)
      setLoading(false)
    }
   loadData();
  },[id]) 

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
   * to update a device's fields, image, and documents
   * @param device: device json
   * @param setDevice: function to set device in higher level Device Page
   * @param deviceType: device type json object
   * @param image: image url
   * @param setImage: function to set image in higher level Device Page
   * @param documents: list of document names
   * @param setDocuments: function to set document names in higher level Device Page
   * @returns 
   */
  const RenderItem = ({building}) => {
    const [editable, setEditable] = useState(false)

    const [buildingId] = useState(building.id)
    const [number, setNumber] = useState(building.number)
    const [lat, setLat] = useState(building.latitude)
    const [long, setLong] = useState(building.longitude)
    const [notes, setNotes] = useState(building.notes)

    const navigate = useNavigate();

    const updateBuildingCall = () => {
      const newBuilding = {
        _id:buildingId,
        number:number,
        notes:notes,
        latitude:lat,
        longitude:long
      }

      // Hit endpoints here
      console.log(newBuilding)
      setEditable(false)
    }

    const cancel = () => {      
      setNumber(building.number)
      setLat(building.latitude)
      setLong(building.longitude)
      setNotes(building.notes)
      setEditable(false)
    }

    const onChange = (event) => {
      const target = event.target.id
      const value = event.target.value
      if(target === 'buildingNotes'){
        setNotes(value)
      } else if (target === 'buildingLatitude'){
        setLat(value)
      } else if (target === 'buildingLongitude'){
        setLong(value)
      } else if (target === 'buildingNumber'){
        setNumber(value)
      }
    }

    return (
      <Form className={styles.form}>
        <Form.Group className="mb-3">
          <Button variant="danger" id="deletebtn" className={styles.deleteButton}>
              Delete Building
          </Button>
          {editable ? 
          <div className={styles.buttonRow}>
              <Button id="savebtn" onClick={() => updateBuildingCall()}>
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

        <FormItem editable={editable} id="buildingNotes" label="Building Notes" value={notes} onChange={onChange}/>
        
        
        <div className={[styles.fields,'mb-3'].join(' ')}>
          <FormCard editable={editable} id="buildingNumber" label="Number" value={number} onChange={onChange} />
          <FormCard editable={editable} id="buildingLatitude" label="Latitude" value={lat} onChange={onChange} />
          <FormCard editable={editable} id="buildingLongitude" label="Longitude" value={long} onChange={onChange}/>
        </div>

      </Form>
    )
  }

  return (
    <div className={styles.item} id="buildingPageContent">
      <ItemCard 
        title={loading ? 'Loading' : `${building.name}`}
        RenderItem={<RenderItem building={building}/>} 
        loading={loading}
        route="/buildings"
      />
    </div>
  )
}

export default BuildingPage