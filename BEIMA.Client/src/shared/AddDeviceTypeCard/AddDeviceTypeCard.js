import { Placeholder, Form, Card, Button, Dropdown, ListGroup } from 'react-bootstrap';
import { useCallback, useEffect, useState } from "react";
import styles from './AddDeviceTypeCard.module.css';

const defaultDeviceFields = {
  "Building": "",
  "Longitude": "",
  "Latitude": "",
  "Device Type": "",
  "Device Tag": "",
  "Manufacturer": "",
  "Model Number": "",
  "Serial Number": "",
  "Year Manufactured": "",
  "Notes": ""
}

const typeAttributes = {
    "Name": "",
    "Description": "",
    "Notes": ""
}

const LoadingContent = () => {
  return (
    <Placeholder animation="wave">
      <Placeholder xs={12} size="sm" bg="secondary" />
    </Placeholder>
  )
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

const TypeFieldList = ({fields}) => {
  return (
    <div>
      {fields.map(element =>
        <ListGroup.Item>{element}</ListGroup.Item>
      )}
      <br/>
    </div>
  )
}

const AddDeviceTypeCard = () => {
  const [deviceFields, setDeviceFields] = useState(defaultDeviceFields);

  return (
    <div>
      <Card>
        <Card.Body>
          <Form>
            <h4>Device Type Information</h4>
            <TypeAttributeForm attributes={Object.keys(typeAttributes)}/>
            <h5>Associated Fields</h5>
            <ListGroup>
              <TypeFieldList fields={Object.keys(deviceFields)}/>
            </ListGroup>
            <Form.Label>Add Field</Form.Label>
            <Form.Control type="text" placeholder="Enter Field Name"/>
            <Button variant="primary" type="submit" className={styles.button}>
              Add Field
            </Button>
          </Form>
          <br/>
          <Button variant="primary" type="submit" className={styles.button}>
            Add Device Type
          </Button>
        </Card.Body>
      </Card>
    </div>
  )
}
export default AddDeviceTypeCard