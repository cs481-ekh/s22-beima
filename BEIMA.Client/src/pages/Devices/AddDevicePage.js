import { useOutletContext } from 'react-router-dom';
import {  Card, Button, Dropdown, Row, Col, Form } from 'react-bootstrap';
import { useCallback, useEffect, useState } from "react";
import styles from './AddDevicePage.module.css';
import ImageFileUpload from '../../shared/ImageFileUpload/ImageFileUpload';
import FormList from '../../shared/FormList/FormList.js'



const AddDevicePage = () => {
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

  const defaultDeviceImage = [];
  const defaultAdditionalDocs = [];

  const [deviceFields, setDeviceFields] = useState(defaultDeviceFields);
  const [deviceImage, setDeviceImage] = useState(defaultDeviceImage);
  const [deviceDocs, setDeviceDocs] = useState(defaultAdditionalDocs);
  const [setPageName] = useOutletContext();
  let fullDeviceJSON = {};

  useEffect(() => {
    setPageName('Add Device')
  },[])

  function createJSON(event){
    let formFields = event.target.form.elements;
    let fieldValues = {};
    for(let i = 0; i < formFields.length; i++){
      let formName = formFields[i].name;
      let fieldNames = Object.keys(deviceFields);
      if(fieldNames.includes(formName)){
        let formJSON =  {[formName] : formFields[i].value};
        formFields[i].value = "";
        Object.assign(fieldValues, formJSON);
      }
    }

    let fieldsJSON = {"Fields" : deviceFields};
    fullDeviceJSON = Object.assign(fieldValues, fieldsJSON);
    setDeviceFields([]);
  }

  return (
    <div className={styles.fieldform}>
      <Card>
        <Card.Body>
          <Form>
            <Row className={styles.buttonGroup}>
              <Col>
              <Dropdown>
                <Dropdown.Toggle variant="success" id="dropdown-basic" className={styles.button}>
                  Select Device Type 
                </Dropdown.Toggle>
                <Dropdown.Menu>
                  <Dropdown.Item href="#/action-1">Default Device Type</Dropdown.Item>
                </Dropdown.Menu>
              </Dropdown>
              </Col>
              <Col>
              <Button variant="primary" type="submit" className={styles.addButton} onClick={(event) => createJSON(event)}>
                Add Device
              </Button>
              </Col>
            </Row>
            <br/>
            <h4>Device Image</h4>
            <ImageFileUpload type="Device Image" details="(Only .png and .jpeg files accepted)" multiple={false} acceptTypes='image/png,image/jpeg'/>
            <h4>Additional Documents</h4>
            <ImageFileUpload type="Additional Documents" multiple={true}/>
            <h4>Fields</h4>
            <FormList fields={Object.keys(deviceFields)}/>
          </Form>
        </Card.Body>
      </Card>
    </div>
  )
}
export default AddDevicePage