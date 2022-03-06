import { useOutletContext } from 'react-router-dom';
import {  Card, Button, Dropdown, Row, Col, Form } from 'react-bootstrap';
import DropdownButton from 'react-bootstrap/DropdownButton';
import { useEffect, useState } from "react";
import styles from './AddDevicePage.module.css';
import FormList from '../../shared/FormList/FormList.js';
import ImageFileUpload from '../../shared/ImageFileUpload/ImageFileUpload.js';
import GetDeviceTypeList from '../../services/GetDeviceTypeList.js';
//import 

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
  const [loading, setLoading] = useState(true);
  const [deviceAdditionalDocs, setAdditionalDocs] = useState();
  const [fullDeviceJSON, setFullDeviceJSON] = useState({});
  const [deviceTypes, setDeviceTypes] = useState([]);
  
  useEffect(() => {
    setPageName('Add Device');
    const loadData = async () => {
      setLoading(true);
      let types = await getDeviceTypes();
      setLoading(false);
      setDeviceTypes(types);
    }
   loadData()
  },[])
  
  const getDeviceTypes = async () => {
    const deviceTypeData = await GetDeviceTypeList();
    let data = []
    for(let i = 0; i < deviceTypeData.response.length; i++){
      data.push({
        name: deviceTypeData.response[i].name,
        id: deviceTypeData.response[i]._id
      });
    }
    return data
  }
  
  const getFieldsForTypeId = (deviceTypeId) => {
    console.log(deviceTypeId);
    //const deviceFieldData = await GetDeviceTypeList();
    //setDeviceFields();
  }

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

    setDeviceFields({defaultDeviceFields});
    setFullDeviceJSON(fieldValues);
    console.log(fullDeviceJSON);
    console.log(deviceImage);
    console.log(deviceAdditionalDocs);
  } 
  
  return (
    <div className={styles.fieldform}>
      <Card>
        <Card.Body>
          <Form>
            <Row className={styles.buttonGroup}>
              <Col>
                <Dropdown id="typeDropDown" onSelect={getFieldsForTypeId}>
                  <Dropdown.Toggle variant="success" id="dropdown-basic" className={styles.button}>
                    Select Device Type 
                  </Dropdown.Toggle>
                  <Dropdown.Menu >
                    {deviceTypes.length > 0 &&
                      deviceTypes.map(item => (
                      <Dropdown.Item  eventKey={item.id} value={item.id} key={item.id}>{item.name}</Dropdown.Item>
                    ))}
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