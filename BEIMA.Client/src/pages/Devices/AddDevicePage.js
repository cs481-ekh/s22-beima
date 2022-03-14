import { useOutletContext } from 'react-router-dom';
import { Card, Button, Row, Col, Form } from 'react-bootstrap';
import { useEffect, useState } from "react";
import styles from './AddDevicePage.module.css';
import FormListWithErrorFeedback from '../../shared/FormList/FormListWithErrorFeedback.js';
import FilledDropDown from '../../shared/DropDown/FilledDropDown.js';
import ImageFileUpload from '../../shared/ImageFileUpload/ImageFileUpload.js';
import GetDeviceTypeList from '../../services/GetDeviceTypeList.js';
import GetDeviceType from '../../services/GetDeviceType.js';
import AddDevice from '../../services/AddDevice.js';


const AddDevicePage = () => {
  // this will be replaced with API call based on selected device type to get the fields
  const mandatoryDeviceFields = {
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

  const [deviceFields, setDeviceFields] = useState(mandatoryDeviceFields);
  const [errors, setErrors] = useState(mandatoryDeviceFields);
  const [setPageName] = useOutletContext();
  const [deviceImage, setDeviceImage] = useState();
  const [deviceAdditionalDocs, setAdditionalDocs] = useState();
  const [deviceTypes, setDeviceTypes] = useState([]);
  const [selectedDeviceType, setSelectedDeviceType] = useState('Select Device Type');
  const [selectedDeviceTypeId, setSelectedDeviceTypeId] = useState();
  const [selectedDeviceFields, setSelectedDeviceFields] = useState();
  
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
  
  /*
  * gets the list of device types from the database
  */
  const getDeviceTypes = async () => {
    const deviceTypeData = await GetDeviceTypeList();
    
    let data = deviceTypeData.response.map((item) => { return { name: item.name, id : item.id} });
    
    return data;
  }
  
  /*
  * gets the extra fields from the DB and adds them to the form
  * @param the type of device that was selected
  */
  const getFieldsForTypeId = async (deviceTypeId) => {
    //console.log(deviceFields);
    const deviceTypeFields = await GetDeviceType(deviceTypeId);
    
    //change the dropdown text and store the fields for their keys
    setSelectedDeviceType(deviceTypeFields.response.name);
    setSelectedDeviceFields(deviceTypeFields.response.fields);
    setSelectedDeviceTypeId(deviceTypeId);
    
    //retireve the values from teh response to label the form elements
    let fieldLabels = Object.values(deviceTypeFields.response.fields);
    
    //append the fields to the current list
    for(let i = 0; i < fieldLabels.length; i++) {
      deviceFields[fieldLabels[i]] = "";
    }

    //onsole.log(deviceFields);
    //add them to the forms and errors list
    setDeviceFields(deviceFields);
    setErrors({});
  }
  
  /*
  * converts the user friendly field name to the form needed by the database
  * in cases where the name needed is another key it will retrieve it from stored values
  * @param the form element's name
  * @return either the converted string or an object to get the converted string/stored value
  */
  function convertToDbFriendly(formName) {
    let result = {};
    /*TODO work in building to get the building ID for insertion*/
    if(Object.values(selectedDeviceFields).includes(formName)){
      let dbId = Object.keys(selectedDeviceFields).find(key => selectedDeviceFields[key] === formName);
      result = {'fieldDbId': dbId};
    } else if (formName == 'Latitude' || formName == 'Longitude') {
      result = {'location': {'type' : formName.toLowerCase(formName)}};
    } else {
      //make first letter lower case
      let dbKey = formName[0].toLowerCase() + formName.slice(1);
      dbKey = dbKey.replace('Number', 'Num');
      //remove spaces
      dbKey = dbKey.replace(/\s+/g, '');
      result = dbKey;
    }
    
    return result;
  }
  
  /*
  * updates the values in state when the user types
  * @param inputEvent the even fired when the user types
  */
  function updateFieldState(inputEvent){
    deviceFields[inputEvent.target.name] = inputEvent.target.value;
    setDeviceFields(deviceFields);
  }
  
  // gathers all the input and puts it into JSON, files are just assigned to state variables for now
  function createJSON(addButtonEvent){
    //temporary until we have large error signaling figured out
    if (selectedDeviceType === 'Select Device Type'){
      alert ('No device type selected');
      return;
    }
    
    let formFields = addButtonEvent.target.form.elements;
    
    let dbJson = {fields: {}, location: { 'notes' : ""}};
    let newErrors = {};
    
    for(let i = 0; i < formFields.length; i++){
      let formName = formFields[i].name;
      let fieldNames = Object.keys(deviceFields);
      
      //lat lon validation
      if (formName === 'Latitude' || formName === 'Longitude') {
        const coordMax = formName === 'Latitude' ? 90 : 180;
        if(!(isFinite(formFields[i].value) && Math.abs(formFields[i].value) <= coordMax)) {
          newErrors[formName] = `${formName} value is invalid. Must be a decimal between -${coordMax} and ${coordMax}.`;
        }
      }
      
      if(fieldNames.includes(formName)){
        let formJSON = '';
        let jsonKey = convertToDbFriendly(formName);
        if(!(typeof jsonKey == 'object')){
          formJSON =  {[jsonKey] : deviceFields[formName]};
          Object.assign(dbJson, formJSON);
        } else if ('fieldDbId' in jsonKey){
          dbJson.fields[jsonKey.fieldDbId] = formFields[i].value;
        } else if ('location' in jsonKey){
          dbJson.location[jsonKey.location.type] = formFields[i].value;
        }
      } else if (formName === "Device Image"){
        setDeviceImage(formFields[i].files[0]);
        formFields[i].value = "";
      } else if (formName === "Additional Documents"){
        setAdditionalDocs(formFields[i].files);
        formFields[i].value = "";
      }
    }
    
    //won't deploy to azure without these until they're used elsewhere
    console.log(deviceImage);
    console.log(deviceAdditionalDocs);
    
    if ( Object.keys(newErrors).length > 0 ) {
      setErrors(newErrors);
    } else {
      setErrors({});
      setSelectedDeviceType('Select Device Type');
      for(let i = 0; i < formFields.length; i++){
        formFields[i].value = "";
      }
      
      /*Need ability to get a building ID, probably a DD on the page*/
      dbJson.location.buildingId = 'a19830495728394829103986';
      dbJson.deviceTypeId = selectedDeviceTypeId;

      AddDeviceToDb(dbJson);
    }
  }

  const AddDeviceToDb = async (dbJson) => {
    const addResponse = await AddDevice(dbJson);
    /*TODO push error to display*/
    if(!(addResponse.status == 200)){
      alert('API responded with: ' + addResponse.status + ' ' + addResponse.response);
    }
  }
  
  return (
    <div className={styles.fieldform}>
      <Card>
        <Card.Body>
          <Form >
            <Row className={styles.buttonGroup}>
              <Col>
                <FilledDropDown dropDownText={selectedDeviceType} items={deviceTypes} selectFunction={getFieldsForTypeId} buttonStyle={styles.button} dropDownId={"typeDropDown"} />
              </Col>
              <Col>
                  <Button variant="primary" type="button" className={styles.addButton} id="addDevice" onClick={createJSON}>
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
              <FormListWithErrorFeedback fields={Object.keys(deviceFields)} errors={errors} changeHandler={updateFieldState}/>
            </div>
          </Form>
        </Card.Body>
      </Card>
    </div>
  )
}
export default AddDevicePage
