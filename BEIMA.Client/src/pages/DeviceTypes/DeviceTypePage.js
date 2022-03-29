import { useState, useEffect } from "react"
import {ItemCard} from "../../shared/ItemCard/ItemCard"
import * as Notifications from '../../shared/Notifications/Notification.js'
import styles from './DeviceTypePage.module.css'
import { useOutletContext, useParams, useNavigate } from 'react-router-dom';
import { Form, Card, Button, FormControl} from "react-bootstrap";
import { IoAdd } from "react-icons/io5";
import { v4 as uuidv4 } from 'uuid';
import GetDeviceType from '../../services/GetDeviceType';
import updateDeviceType from '../../services/UpdateDeviceType';
import deleteDeviceType from '../../services/DeleteDeviceType';
import * as Constants from '../../Constants';

const DeviceTypePage = () => {
  const [deviceType, setDeviceType] = useState(null)
  const [loading, setLoading] = useState(true)
  const [setPageName] = useOutletContext();

  const { typeId } = useParams();

  useEffect(() => {
    setPageName('View Device Type')
    const loadData = async () => {
      setLoading(true)
      let type = (await GetDeviceType(typeId)).response
      type['addedFields'] = [];
      type['deletedFields'] = [];
      setDeviceType(type)
      setLoading(false)
    }
    loadData()
  },[typeId, setPageName])

  /**
   * Renders a card styled field in a device type.
   * Allows the user to update the input inside of it
   * as well as delete the field in the higher level <DeviceTypePage/>
   * @param field: field uid
   * @param value: field value
   * @param editable: can this input be modified
   * @param deleteField: function to delete field off the device type
   * in higher level <DeviceTypePage/>
   * @returns 
   */
  const Field = ({field, value, editable, fieldChange, deleteField}) => {
    return (
      <Card>
        <Card.Body >
            <Form.Group className="mb-3" controlId={field}>
              <FormControl required type="text" disabled={!editable} size="sm" placeholder="Field Name" value={value} onChange={fieldChange}  maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH}/>
            </Form.Group>                
          { editable ? 
           <div className={styles.deleteButton}>
              <Button variant="danger" onClick={() => deleteField(field)}>
                Delete
              </Button>
            </div>
          : null}
        </Card.Body>
      </Card>
    )
  }  

  /**
   * Renders a custom form that enables a user
   * to update a device's type's fields
   * 
   * @param item: json device type
   * @param setitem: function to update item in higher level <DeviceTypePage/>
   * @returns 
   */
  const RenderItem = ({item, setItem}) => {
    const [editable, setEditable] = useState(false)
    
    const [description, setDescription] = useState(item.description)
    const [notes, setNotes] = useState(item.notes)
    const [fields, setFields] = useState({...item.fields})
    const [addedFields, setAddedFields] = useState({...item.addedFields})
    const [deletedFields, setDeletedFields] = useState([...item.deletedFields])
    const navigate = useNavigate();
    
    const handleSubmit = () => {
      // we keep track of the added fields as an object, but the endpoint takes in an array
      // so we grab the values from the addFields object and send it to the endpoint
      let newFields = Object.values(addedFields);

      const result = {
        id: typeId,
        name: item.name,
        description: description,
        notes: notes,
        fields: fields,
        newFields: newFields,
        deletedFields: deletedFields
      }
      //call endpoint
      updateDeviceType(result);
      setEditable(false);
    }

    const addField = () => {
      const key = uuidv4()
      let temp = {...addedFields}
      temp[key] = ""
      setAddedFields(temp)
    }

    const deleteField = (field) => {
      let tempF = {...fields}
      let tempAf = {...addedFields}
      let tempDf = [...deletedFields]

      if(field in tempF){
        delete tempF[field]
        tempDf.push(field)
        setFields(tempF)
        setDeletedFields(tempDf)
      } else {
        delete tempAf[field]
        setAddedFields(tempAf)
      }
    }

    const onDescriptionChange = (event) => {
      setDescription(event.target.value)
    }

    const onNotesChange = (event) => {
      setNotes(event.target.value)
    }

    const onFieldChange = (event) => {
      let id = event.target.id
      let temp = {...fields}
      temp[id] = event.target.value
      setFields(temp)
    }

    const onAddedFieldChange = (event) => {
      let id = event.target.id
      let temp = {...addedFields}
      temp[id] = event.target.value
      setAddedFields(temp)
    }

    const cancel = () => {
      setEditable(false)
      setDescription(item.description)
      setNotes(item.notes)
      setFields({...item.fields})
      setAddedFields({...item.addedFields})
      setDeletedFields([...item.deletedFields])
    }

    const attemptDeleteType = async (id) => {
      let deleteNotif = await Notifications.warning("Warning: Device Type Deletion", [`Are you sure you want to delete device ${item.name}?`]);
      if(deleteNotif.isConfirmed){
        let response = await deleteDeviceType(id);
        // the endpoint returns an error message if there is more than one device with that type
        // let's show that message to the user
        if(response.status === 200){
          Notifications.success("Device Type Deletion Successful", `Device Type ${item.name} successfully deleted.`);
          navigate('/deviceTypes');
        } else {
          Notifications.error("Unable to Delete Device Type", `Deletion of Device ${item.name} failed. ${response.response}`);
        }
      }
    }

    return (
      <Form className={styles.form}>
        <Form.Group className="mb-3">
        <Button variant="danger" id="deletebtn" onClick={() => attemptDeleteType(typeId)} className={styles.deleteButton}>
          Delete Device Type
        </Button>
          {editable ? 
           <div className={styles.buttonRow}>
              <Button id="savebtn" onClick={handleSubmit}>
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

        <Form.Group className="mb-3" controlId="description">
          <Form.Label><b>Description</b></Form.Label>
          <Form.Control required as="textarea" rows={3} placeholder="Device Type Description"  disabled={!editable} value={description} onChange={onDescriptionChange} maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH}/>
        </Form.Group>

        <Form.Group className="mb-3" controlId="notes">
          <Form.Label><b>Notes</b></Form.Label>
          <Form.Control required as="textarea" rows={1} placeholder="Device Type Notes"  disabled={!editable} value={notes}  onChange={onNotesChange} maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH}/>
        </Form.Group>

        <Form.Group  className="mb-3">
          <Form.Label><b>Mandatory Fields</b></Form.Label>
          <div className={styles.fields}>
            <Field field="manId" value="Manufacturer" editable={false}/>
            <Field field="modId" value="Model Number" editable={false}/>
            <Field field="serId" value="Serial Number" editable={false}/>
            <Field field="yearId" value="Year Manufactured" editable={false}/>
          </div>
        </Form.Group>
        
        <Form.Group  className={[styles.between, "mb-3" ].join(' ')}>
          <Form.Label><b>Additional Fields</b></Form.Label>
          {editable ? 
            <Button variant="primary" id="addbtn" onClick={addField} >
             <IoAdd size={20} color="#fff"/>
            </Button>
          : null}
        </Form.Group>
        <Form.Group id="additionalfields">
          <div className={styles.fields}>
            {Object.keys(fields).map((field, i) => <Field key={i} field={field} editable={editable} value={fields[field]} fieldChange={onFieldChange} deleteField={deleteField}/>)}
            {Object.keys(addedFields).map((field, i) => <Field key={i} field={field} editable={editable} value={addedFields[field]} fieldChange={onAddedFieldChange} deleteField={deleteField}/>)}
          </div>  
        </Form.Group>
      </Form>
    )
  }

  return (
    <div className={styles.item} id="deviceTypeContent">
      <ItemCard 
        title={loading ? 'Loading' : deviceType.name}
        RenderItem={<RenderItem item={deviceType} setItem={setDeviceType}/>} 
        loading={loading}
        route="/deviceTypes"
      />
    </div>
  )
}

export default DeviceTypePage