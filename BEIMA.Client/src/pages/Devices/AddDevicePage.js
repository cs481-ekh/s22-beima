import { useOutletContext } from 'react-router-dom';
import { Card, Button, Row, Col, Form } from 'react-bootstrap';
import { useEffect, useState } from "react";
import styles from './AddDevicePage.module.css';
import FormListWithErrorFeedback from '../../shared/FormList/FormListWithErrorFeedback.js';
import FilledDropDown from '../../shared/DropDown/FilledDropDown.js';
import ImageFileUpload from '../../shared/ImageFileUpload/ImageFileUpload.js';
import GetDeviceTypeList from '../../services/GetDeviceTypeList.js';


const AddDevicePage = () => {
  // this will be replaced with API call based on selected device type to get the fields
  const currentDeviceFields = {
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

  const [deviceFields, setDeviceFields] = useState(currentDeviceFields);
  const [errors, setErrors] = useState(currentDeviceFields);
  const [setPageName] = useOutletContext();
  const [deviceImage, setDeviceImage] = useState();
  const [deviceAdditionalDocs, setAdditionalDocs] = useState();
  const [fullDeviceJSON, setFullDeviceJSON] = useState({});
  const [deviceTypes, setDeviceTypes] = useState([]);
  
  useEffect(() => {
    setPageName('Add Device')
  }, [setPageName])
  
  useEffect(() => {
    const loadData = async () => {
      let types = await getDeviceTypes();
      setDeviceTypes(types);
    }
   loadData()
  },[])
  
  const getDeviceTypes = async () => {
    const deviceTypeData = await GetDeviceTypeList();
    
    let data = deviceTypeData.response.map((item) => { return { name: item.name, id : item.id} });
    
    return data
  }
  
  const getFieldsForTypeId = async (deviceTypeId) => {

  }
  
  // gathers all the input and puts it into JSON, files are just assigned to state variables for now
  function createJSON(addButtonEvent){
    let formFields = addButtonEvent.target.form.elements;
    let fieldValues = {};
    let newErrors = {};

    for(let i = 0; i < formFields.length; i++){
      let formName = formFields[i].name;
      let fieldNames = Object.keys(deviceFields);

      if(fieldNames.includes(formName)){
        let formJSON =  {[formName] : formFields[i].value};
        
        //lat lon validation
        if (formName === 'Latitude' || formName === 'Longitude') {
          const coordMax = formName === 'Latitude' ? 90 : 180;
          if(!(isFinite(formFields[i].value) && Math.abs(formFields[i].value) <= coordMax)) {
            newErrors[formName] = `${formName} value is invalid. Must be a decimal between -${coordMax} and ${coordMax}.`;
          }
        }
        
        Object.assign(fieldValues, formJSON);
      } else if (formName === "Device Image"){
        setDeviceImage(formFields[i].files[0]);
        formFields[i].value = "";
      } else if (formName === "Additional Documents"){
        setAdditionalDocs(formFields[i].files);
        formFields[i].value = "";
      }
    }

    setDeviceFields(currentDeviceFields);
    setFullDeviceJSON(fieldValues);
    
    //won't deploy to azure without these until they're used elsewhere
    console.log(fullDeviceJSON);
    console.log(deviceImage);
    console.log(deviceAdditionalDocs);

    if ( Object.keys(newErrors).length > 0 ) {
      setErrors(newErrors);
    } else {
      setErrors({});
      for(let i = 0; i < formFields.length; i++){
        formFields[i].value = "";
      }
      
    }
  }
  
  return (
    <div className={styles.fieldform}>
      <Card>
        <Card.Body>
          <Form>
            <Row className={styles.buttonGroup}>
              <Col>
                <FilledDropDown items={deviceTypes} selectFunction={getFieldsForTypeId} buttonStyle={styles.button} dropDownId={"typeDropDown"} />
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
            <div>
              <FormListWithErrorFeedback fields={Object.keys(deviceFields)} errors={errors} />
            </div>
          </Form>
        </Card.Body>
      </Card>
    </div>
  )
}
export default AddDevicePage
