import { Form, Card, Button, ListGroup } from 'react-bootstrap';
import { useOutletContext } from 'react-router-dom';
import { useState, useEffect } from "react";
import { IoMdCloseCircle } from "react-icons/io";
import styles from './AddDeviceTypePage.module.css';
import FormList from '../../shared/FormList/FormList.js'

const AddDeviceTypePage = () => {
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

  const typeAttributes1 = {
      "Name": "",
      "Description": "",
      "Device Type Notes": ""
  }

  const [deviceFields, setDeviceFields] = useState([]);
  const [typeAttributes] = useState(typeAttributes1);
  const [setPageName] = useOutletContext();
  let fullTypeJSON = {};
  let field;

  useEffect(() => {
      setPageName('Add Device Type')
  },[]);

  function removeField(field) {
    setDeviceFields(deviceFields.filter(item => item !== field))
  }

  function addField(newField, event) {
    let newList = deviceFields.concat(newField);
    setDeviceFields(newList);
    event.target.form.elements.newFieldForm.value = "";
  }

  function createJSON(event){
    let formFields = event.target.form.elements;
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

  const TypeFieldList = ({fields, mandatory}) => {
    return (
      <div>
        {fields.map(element =>
          <div>
            <ListGroup.Item>
                {element}
                {mandatory ?
                null
                : <IoMdCloseCircle className={styles.listButton} onClick={() => removeField(element)}></IoMdCloseCircle>}
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
              <Button variant="primary" type="button" className={styles.addButton} onClick={(event) => createJSON(event)}>
                Add Device Type
              </Button>
              <h4>Device Type Information</h4>
            </div>
            <FormList fields={Object.keys(typeAttributes)}/>
          </Form>  
          <br/>
          <h5>Associated Fields</h5>
          <h6>Mandatory Fields</h6>
          <ListGroup>
            <TypeFieldList fields={defaultDeviceFields} mandatory={true}/>
          </ListGroup>
          <h6>Custom Fields</h6>
          <ListGroup>
            <TypeFieldList fields={deviceFields} mandatory={false}/>
          </ListGroup>
          <Form>
            <Form.Group controlId='newField'>
              <Form.Label>Add Custom Field</Form.Label>
              <Form.Control name="newFieldForm" type="text" placeholder="Enter Field Name" value={field} onChange={(event) => {field = event.target.value}}/> 
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
export default AddDeviceTypePage