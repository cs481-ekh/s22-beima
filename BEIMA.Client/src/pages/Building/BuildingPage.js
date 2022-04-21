import { useOutletContext, useParams, useNavigate} from 'react-router-dom';
import { useEffect, useState  } from "react"
import {ItemCard} from "../../shared/ItemCard/ItemCard"
import styles from './BuildingPage.module.css'
import { Form, Card, Button, FormControl } from "react-bootstrap";
import * as Constants from '../../Constants';
import GetBuilding from '../../services/GetBuilding.js';
import UpdateBuilding from '../../services/UpdateBuilding';
import DeleteBuilding from '../../services/DeleteBuilding';
import * as Notifications from '../../shared/Notifications/Notification.js';

const BuildingPage = () => {
  const [setPageName] = useOutletContext();
  const [loading, setLoading] = useState(true);
  const [building, setBuilding] = useState(null);
  const [buildingChanged, setBuildingChanged] = useState(false);

  const { id } = useParams();

  useEffect(() => {
    const loadData = async() => {
      const building = (await GetBuilding(id)).response;
      setBuilding(building)
      setLoading(false)
      setBuildingChanged(false);
    }
   loadData();
    setPageName('View Building')
  },[setPageName, buildingChanged, id])

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

    const [buildingId] = useState(building._id)
    const [name, setName] = useState(building.name)
    const [number, setNumber] = useState(building.number)
    const [lat, setLat] = useState(building.location.latitude)
    const [long, setLong] = useState(building.location.longitude)
    const [notes, setNotes] = useState(building.notes)
    const navigate = useNavigate();

    const updateBuildingCall = async () => {
      const newBuilding = {
        _id:buildingId,
        name:name,
        number:number,
        notes:notes,
        location: {
          latitude:lat,
          longitude:long
        }
      }

      // Call Update Building
      let updateResult = await UpdateBuilding(newBuilding);
      if(updateResult.status === Constants.HTTP_SUCCESS){
        Notifications.success("Update Building Successful", `Building ${name} updated successfully.`);
        setEditable(false);
        setLoading(true);
        setBuildingChanged(true);
      } else {
        Notifications.error("Unable to Update Building", `Update of Building ${name} failed.`);
      }
    }

    const deleteBuildingCall = async () => {
      let deleteNotif = await Notifications.warning("Warning: Building Deletion", [`Are you sure you want to delete Building ${name}?`]);
      if(deleteNotif.isConfirmed){
        let deleteResult = await DeleteBuilding(buildingId);
        if(deleteResult.status === Constants.HTTP_SUCCESS){
          Notifications.success("Building Deletion Successful", `Building ${name} successfully deleted.`);
          navigate('/buildings');
        } else if (deleteResult.status === Constants.HTTP_CONFLICT_RESULT){
          Notifications.error("Unable to Delete Building", `${deleteResult.response}`);
        } else {
          Notifications.error("Unable to Delete Building", `Deletion of Building ${name} failed.`);
        }
      }
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
      } else if (target === 'buildingName'){
        setName(value)
      }else if (target === 'buildingLatitude'){
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
          <Button variant="danger" id="deletebtn" className={styles.deleteButton} onClick={() => deleteBuildingCall()}>
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

        <FormItem editable={editable} id="buildingName" label="Building Name" value={name} onChange={onChange}/>
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