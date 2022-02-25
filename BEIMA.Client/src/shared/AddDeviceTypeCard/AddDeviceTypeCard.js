import { Placeholder, Form, Card, Button, Dropdown, ListGroup, InputGroup } from 'react-bootstrap';
import { useCallback, useEffect, useState } from "react";
import { IoMdCloseCircle } from "react-icons/io";
import styles from './AddDeviceTypeCard.module.css';

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

const AddDeviceTypeCard = ({attributes, fields}) => {
  const [deviceFields, setDeviceFields] = useState(fields);
  var field;

  function removeField(field) {
    setDeviceFields(deviceFields.filter(item => item !== field))
  }

  function addField(newField, event) {
    var newList = deviceFields.concat(newField);
    setDeviceFields(newList);
    event.target.form.elements.newField.value = "";
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
            <TypeAttributeForm attributes={Object.keys(attributes)}/>
          </Form>  
            <h5>Associated Fields</h5>
            <ListGroup>
              <TypeFieldList fields={deviceFields}/>
            </ListGroup>
            <Form>
              <Form.Group controlId='newField'>
                <Form.Label>Add Field</Form.Label>
                <Form.Control name="newField" type="text" placeholder="Enter Field Name" value={field} onChange={(event) => {field = event.target.value}}/> 
              </Form.Group>
              <Button variant="primary" type="button" className={styles.button} onClick={(event) => addField(field, event)}>
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