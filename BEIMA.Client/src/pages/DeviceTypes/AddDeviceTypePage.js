import { Form, Card, Button, ListGroup } from 'react-bootstrap';
import { useOutletContext } from 'react-router-dom';
import { useState, useEffect } from "react";
import { IoMdCloseCircle } from "react-icons/io";
import styles from './AddDeviceTypePage.module.css';
import FormList from '../../shared/FormList/FormList.js';
import AddDeviceType from '../../services/AddDeviceType';
import * as Constants from '../../Constants';

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
      "Notes": ""
  }

  const [customDeviceFields, setCustomDeviceFields] = useState([]);
  const [typeAttributes] = useState(typeFields);
  const [setPageName] = useOutletContext();
  const [errorMessage, setErrorMessage] = useState('');
  const [isInvalid, setIsInvalid] = useState(false);
  let fullTypeJSON = {};
  let field;

  useEffect(() => {
      setPageName('Add Device Type')
  });

  // allows the user to remove a field they added
  function removeField(field) {
    setCustomDeviceFields(customDeviceFields.filter(item => item !== field))
  }

  // adds field to list
  function addField(newField, event) {
    if(newField === undefined || !newField.replace(/\s/g, '').length){
      setErrorMessage('Custom Field cannot be empty!');
      setIsInvalid(true);
      return;
    }
    let foundItem = customDeviceFields.map((item) => {return item === newField});
    if(foundItem.length > 0 && foundItem.includes(true)){
      setErrorMessage('Custom Field already exists!');
      setIsInvalid(true);
      return;
    }
    setIsInvalid(false);
    let newList = customDeviceFields.concat(newField);
    setCustomDeviceFields(newList);
    event.target.form.elements.newFieldForm.value = "";
  }

  // gathers input and puts it into JSON
  async function createJSON(addButtonEvent){
    let formFields = addButtonEvent.target.form.elements;
    let attributeValues = {};
    for(let i = 0; i < formFields.length; i++){
      let formName = formFields[i].name;
      let attributeNames = Object.keys(typeAttributes);
      if(attributeNames.includes(formName)){
        let formJSON =  {[formName.toLowerCase()] : formFields[i].value};
        formFields[i].value = "";
        Object.assign(attributeValues, formJSON);
      }
    }

    let fieldsJSON = {"fields" : customDeviceFields};
    fullTypeJSON = Object.assign(attributeValues, fieldsJSON);
    setCustomDeviceFields([]);
    await AddDeviceType(fullTypeJSON);
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
          <br/>
          <h6>Custom Fields</h6>
          <ListGroup id="customFields">
            <TypeFieldList fields={customDeviceFields} mandatory={false} />
          </ListGroup>
          <br/>
          <Form>
            <Form.Group>
              <Form.Label>Add Custom Field</Form.Label>
              <Form.Control name="newFieldForm" type="text" placeholder="Enter Field Name" id="newField" isInvalid={isInvalid} value={field} onChange={(event) => {field = event.target.value; setIsInvalid(false)}} maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH}/> 
              <Form.Control.Feedback type='invalid'>{errorMessage}</Form.Control.Feedback>
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