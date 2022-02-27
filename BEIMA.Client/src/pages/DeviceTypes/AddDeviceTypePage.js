import { Form, Card, Button, ListGroup } from 'react-bootstrap';
import { useOutletContext } from 'react-router-dom';
import { useState, useEffect } from "react";
import { IoMdCloseCircle } from "react-icons/io";
import styles from './AddDeviceTypePage.module.css';
import FormList from '../../shared/FormList/FormList.js'

const AddDeviceTypePage = () => {
  // will be replaced with API call to get default fields
  const defaultDeviceFields = [
    "Building",
    "Longitude",
    "Latitude",
    "Location Notes",
    "Device Tag",
    "Manufacturer",
    "Model Number",
    "Serial Number",
    "Year Manufactured",
    "Device Notes"
  ]

  // basic fields for all device types
  const typeFields = {
      "Name": "",
      "Description": "",
      "Device Type Notes": ""
  }

  const [deviceFields, setDeviceFields] = useState([]);
  const [typeAttributes] = useState(typeFields);
  const [setPageName] = useOutletContext();
  let fullTypeJSON = {};
  let field;

  useEffect(() => {
      setPageName('Add Device Type')
  });

  // allows the user to remove a field they added
  function removeField(field) {
    setDeviceFields(deviceFields.filter(item => item !== field))
  }

  // adds field to list
  function addField(newField, event) {
    let newList = deviceFields.concat(newField);
    setDeviceFields(newList);
    event.target.form.elements.newFieldForm.value = "";
  }

  // gathers input and puts it into JSON
  function createJSON(object){
    let formFields = object.target.form.elements;
    let attributeValues = {};
    for(let i = 0; i < formFields.length; i++){
      let formName = formFields[i].name;
      let attributeNames = Object.keys(typeAttributes);
      if(attributeNames.includes(formName)){
        let formJSON =  {[formName] : formFields[i].value};
        formFields[i].value = "";
        Object.assign(attributeValues, formJSON);
      }
    }

    let fieldsJSON = {"Fields" : deviceFields};
    fullTypeJSON = Object.assign(attributeValues, fieldsJSON);
    setDeviceFields([]);
  }

  // list for fields
  const TypeFieldList = ({fields, mandatory}) => {
    return (
      <div>
        {fields.map(element =>
          <div key={element}>
            <ListGroup.Item id={element}>
                {element}
                {mandatory ?
                null
                : <IoMdCloseCircle className={styles.listButton} id={"remove" + element} onClick={() => removeField(element)}></IoMdCloseCircle>}
            </ListGroup.Item>
          </div>
        )}
        <br/>
      </div>
    )
  }

  return (
    <div className={styles.typeform}>
      <Card>
        <Card.Body>
          <Form>
            <div>
              <Button variant="primary" type="button" className={styles.addButton} id="addDeviceType" onClick={(event) => createJSON(event)}>
                Add Device Type
              </Button>
              <h4>Device Type Information</h4>
            </div>
            <FormList fields={Object.keys(typeAttributes)}/>
          </Form>  
          <br/>
          <h5>Associated Fields</h5>
          <h6>Mandatory Fields</h6>
          <ListGroup id="mandatoryFields">
            <TypeFieldList fields={defaultDeviceFields} mandatory={true} />
          </ListGroup>
          <h6>Custom Fields</h6>
          <ListGroup id="customFields">
            <TypeFieldList fields={deviceFields} mandatory={false} />
          </ListGroup>
          <Form>
            <Form.Group>
              <Form.Label>Add Custom Field</Form.Label>
              <Form.Control name="newFieldForm" type="text" placeholder="Enter Field Name" id="newField" value={field} onChange={(event) => {field = event.target.value}}/> 
            </Form.Group>
            <Button variant="primary" type="button" className={styles.button} id="addField" onClick={(event) => addField(field, event)}>
              Add Field
            </Button>
          </Form>
          <br/>
        </Card.Body>
      </Card>
    </div>
  )
}
export default AddDeviceTypePage