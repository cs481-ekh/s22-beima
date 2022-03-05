import { useOutletContext } from 'react-router-dom';
import { Card, Button, Dropdown, Row, Col, Form } from 'react-bootstrap';
import { useEffect, useState } from "react";
import styles from './AddDevicePage.module.css';
import FormList from '../../shared/FormList/FormList.js';
import ImageFileUpload from '../../shared/ImageFileUpload/ImageFileUpload.js';

const AddDevicePage = () => {
  // this will be replaced with API call based on selected device type to get the fields
  const defaultDeviceFields = {
    "Building": "",
    "Longitude": "",
    "Latitude": "",
    "Device Tag": "",
    "Manufacturer": "",
    "Model Number": "",
    "Serial Number": "",
    "Year Manufactured": "",
    "Notes": ""
  }

  const [deviceFields, setDeviceFields] = useState(defaultDeviceFields);
  const [form, setForm] = useState({})
  const [setPageName] = useOutletContext();
  const [deviceImage, setDeviceImage] = useState();
  const [deviceAdditionalDocs, setAdditionalDocs] = useState();
  const [fullDeviceJSON, setFullDeviceJSON] = useState({});
  const [errors, setErrors] = useState({})
  
  const setField = (field, value) => {
    setForm({
      ...form,
      [field]: value
    })
    // Check and see if errors exist, and remove them from the error object:
    if ( !!errors[field] ) setErrors({
      ...errors,
      [field]: null
    })
  }

  useEffect(() => {
    setPageName('Add Device')
  })
  
//  const handleSubmit = e => {
//    e.preventDefault()
//    // get our new errors
//    //const newErrors = findFormErrors()
//    // Conditional logic:
//    if ( Object.keys(newErrors).length > 0 ) {
//      // We got errors!
//      setErrors(newErrors)
//    } else {
//      // No errors! Put any logic here for the form submission!
//      alert('Thank you for your feedback!')
//    }
//  }
  
  // gathers all the input and puts it into JSON, files are just assigned to state variables for now
  function createJSON(addButtonEvent){
    let newErrors = {};
    
    let formFields = addButtonEvent.target.form.elements;
    let fieldValues = {};

    for(let i = 0; i < formFields.length; i++){
      let formName = formFields[i].name;
      let fieldNames = Object.keys(deviceFields);

      if(fieldNames.includes(formName)){
        let formJSON =  {[formName] : formFields[i].value};
        
        
        if (formName=='Latitude' || formName=='Longitude') {
          newErrors = validateLatLon(newErrors, formName, formFields[i].value);
        }

        


      
        
        //console.log(formName);
        //console.log(formFields[i].value);
        //console.log(formJSON);
        //formFields[i].value = "";
        Object.assign(fieldValues, formJSON);
      } else if (formName === "Device Image"){
        setDeviceImage(formFields[i].files[0]);
        formFields[i].value = "";
      } else if (formName === "Additional Documents"){
        setAdditionalDocs(formFields[i].files);
        formFields[i].value = "";
      }
    }
    

    
    

    setDeviceFields(defaultDeviceFields);
    setFullDeviceJSON(fieldValues);
    console.log(fullDeviceJSON);
    //findFormErrors(fieldNames);
  }
  
  function validateLatLon(newErrors, formName, value){
    const maxLat = 90;
    const maxLon = 180;
    const curentMaxVal = formName == 'Latitude' ? maxLat : maxLon;
    
    let errorText = '<latLon> is invalid. Must be a decimal number between -<value> and <value>';
    
    //test that it's a number, and inside the bounds for each
    let valueOk = isFinite(value) && Math.abs(value) <= (formName == 'Latitude' ? maxLat : maxLon);
    
    if (!valueOk) {
      newErrors[formName] = errorText.replace('<latLon>', formName).replaceAll('<value>', curentMaxVal);
    }

    return newErrors;
  }
  

  return (
    <div className={styles.fieldform}>
      <Card>
        <Card.Body>
          <Form>
            <Row className={styles.buttonGroup}>
              <Col>
              <Dropdown id="typeDropDown">
                <Dropdown.Toggle variant="success" id="dropdown-basic" className={styles.button}>
                  Select Device Type 
                </Dropdown.Toggle>
                <Dropdown.Menu>
                  <Dropdown.Item href="#/action-1">Default Device Type</Dropdown.Item>
                </Dropdown.Menu>
              </Dropdown>
              </Col>
              <Col>
              <Button variant="primary" type="button" className={styles.addButton} id="addDevice" onClick={(event) => createJSON(event)}>
                Add Device
              </Button>
              </Col>
            </Row>
            <br/>
            <h4>Device Image</h4>
            <ImageFileUpload type="Device Image" multiple={false} />
            <br/>
            <h4>Additional Documents</h4>
            <ImageFileUpload type="Additional Documents" multiple={true}/>
            <br/>
            <h4>Fields</h4>
            {Object.keys(deviceFields).map(element =>
                <Form.Group key={element} id={element}>
                  <Form.Label>{element}</Form.Label><Form.Control.Feedback type='invalid'> { errors.element } </Form.Control.Feedback>
                  <Form.Control id={"input" + element} type="text" name={element} placeholder={"Enter " + element} onChange={ e => setField(element, e.target.value) } isInvalid={ !!errors.element }/>
                </Form.Group>
              )}
          </Form>
        </Card.Body>
      </Card>
    </div>
  )
}
export default AddDevicePage