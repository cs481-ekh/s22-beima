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
          <Form.Control type="text" name={element} placeholder={"Enter " + element}/>
        </Form.Group>
      )} 
      <br/>
    </div>
  )
}

const AddDeviceTypeCard = ({attributes, fields}) => {
  const [deviceFields, setDeviceFields] = useState(fields);
  const [typeAttributes, setAttributes] = useState(attributes);
  let fullTypeJSON = {};
  let field;

  function removeField(field) {
    setDeviceFields(deviceFields.filter(item => item !== field))
  }

  function addField(newField, event) {
    let newList = deviceFields.concat(newField);
    setDeviceFields(newList);
    event.target.form.elements.newField.value = "";
  }

  function addFields() {
    let i = 0;
    let fieldsJSON = deviceFields.map(field => {
      let keyName = "field" + i;
      i++;
      return {[keyName]: field};
    })

    return {"Fields" : fieldsJSON};
  }

  function createJSON(event){
    let formFields = event.target.form.elements;
    let attributeValues = {};
    for(let i = 0; i < formFields.length; i++){
      let temp = formFields[i].name;
      let temp2 = Object.keys(typeAttributes);
      if(temp2.includes(temp)){
        let formJSON =  {[temp] : formFields[i].value};
        formFields[i].value = "";
        Object.assign(attributeValues, formJSON);
      }
    }

    setAttributes(attributeValues);
    let fields = addFields();
    fullTypeJSON = Object.assign(typeAttributes, fields);
    console.log(fullTypeJSON);
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
            <div>
              <Button variant="primary" type="button" className={styles.addButton} onClick={(event) => createJSON(event)}>
                Add Device Type
              </Button>
              <h4>Device Type Information</h4>
            </div>
            <TypeAttributeForm attributes={Object.keys(typeAttributes)}/>
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
          
        </Card.Body>
      </Card>
    </div>
  )
}
export default AddDeviceTypeCard