import { useOutletContext } from 'react-router-dom';
import {  Card, Button, Dropdown, Row, Col, Form } from 'react-bootstrap';
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
  const [setPageName] = useOutletContext();
  const [deviceImage, setDeviceImage] = useState();
  const [deviceAdditionalDocs, setAdditionalDocs] = useState();
  const [fullDeviceJSON, setFullDeviceJSON] = useState({});

  useEffect(() => {
    setPageName('Add Device')
  })

  // gathers all the input and puts it into JSON, files are just assigned to state variables for now
  function createJSON(addButtonEvent){
    let formFields = addButtonEvent.target.form.elements;
    let fieldValues = {};

    for(let i = 0; i < formFields.length; i++){
      let formName = formFields[i].name;
      let fieldNames = Object.keys(deviceFields);

      if(fieldNames.includes(formName)){
        let formJSON =  {[formName] : formFields[i].value};
        formFields[i].value = "";
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
    console.log(deviceImage);
    console.log(deviceAdditionalDocs);
    
    validateLatLon();
  } 
  
  function validateLatLon(){
  console.clear();
  console.log("validate");
  console.log(fullDeviceJSON);
  console.log(fullDeviceJSON.Latitude);
  
      fullDeviceJSON.Latitude = "43.603007486265035";    
    fullDeviceJSON.Longitude = "-116.1959187981161";
  let validLatLonRegex = /^((\-?|\+?)?\d+(\.\d+)?),\s*((\-?|\+?)?\d+(\.\d+)?)$/;
  
  console.log(validLatLonRegex.test(fullDeviceJSON.Longitude));
  console.log(fullDeviceJSON.Latitude.match(validLatLonRegex));
  
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
            <FormList fields={Object.keys(deviceFields)}/>
          </Form>
        </Card.Body>
      </Card>
    </div>
  )
}
export default AddDevicePage