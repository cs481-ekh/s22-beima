import { Placeholder, Form, Card, Button, Dropdown, ListGroup } from 'react-bootstrap';
import { useCallback, useEffect, useState } from "react";
import { IoMdCloseCircle } from "react-icons/io";
import styles from './AddDeviceTypeCard.module.css';

const defaultDeviceFields = [
  "Building",
  "Longitude",
  "Latitude",
  "Location Notes",
  "Device Type",
  "Device Tag",
  "Manufacturer",
  "Model Number",
  "Serial Number",
  "Year Manufactured",
  "Device Notes"
]

const typeAttributes = {
    "Name": "",
    "Description": "",
    "Device Type Notes": ""
}

const TypeAttributeForm = ({attributes}) => {
  return (
    <div>
      {attributes.map(element =>
        <Form.Group>
          <Form.Label>{element}</Form.Label>
          <Form.Control type="text" placeholder={"Enter " + element} />
        </Form.Group>
      )}
      <br/>
    </div>
  )
}


const AddDeviceTypeCard = () => {
  const [deviceFields, setDeviceFields] = useState(defaultDeviceFields);
  const [field, setField] = useState('');

  function removeField(field) {
    setDeviceFields(deviceFields.filter(item => item !== field))
  }

  function addField(newField) {
    setDeviceFields(deviceFields.push(newField));
  }

  const TypeFieldList = ({fields}) => {
    return (
      <div>
        {fields.map(element =>
          <div>
            <ListGroup.Item>
              {element}
              <IoMdCloseCircle className={styles.listButton} onClick={() => removeField(element)}></IoMdCloseCircle>
            </ListGroup.Item>
          </div>
        )}
        <br/>
      </div>
    )
  }

  return (
    <div>
      <Card>
        <Card.Body>
          <Form>
            <h4>Device Type Information</h4>
            <TypeAttributeForm attributes={Object.keys(typeAttributes)}/>
          </Form>  
            <h5>Associated Fields</h5>
            <ListGroup>
              <TypeFieldList fields={deviceFields}/>
            </ListGroup>
            <Form onSubmit={() => addField()}>
              <Form.Group controlId='newField'>
                <Form.Label>Add Field</Form.Label>
                <Form.Control type="text" placeholder="Enter Field Name" onChange={setField}/> 
              </Form.Group>
              <Button variant="primary" type="button" className={styles.button} onClick={() => addField(field)}>
                Add Field
              </Button>
            </Form>
          <br/>
          <Button variant="primary" type="submit" className={styles.addButton}>
            Add Device Type
          </Button>
        </Card.Body>
      </Card>
    </div>
  )
}
export default AddDeviceTypeCard