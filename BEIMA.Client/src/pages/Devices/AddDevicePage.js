import { useOutletContext } from 'react-router-dom';
import { Card, Button, Row, Col, Form } from 'react-bootstrap';
import { useEffect, useState } from "react";
import styles from './AddDevicePage.module.css';
import FormListWithErrorFeedback from '../../shared/FormList/FormListWithErrorFeedback.js';
import FilledDropDown from '../../shared/DropDown/FilledDropDown.js';
import ImageFileUpload from '../../shared/ImageFileUpload/ImageFileUpload.js';
import * as Notifications from '../../shared/Notifications/Notification.js';
import GetDeviceTypeList from '../../services/GetDeviceTypeList.js';
import GetDeviceType from '../../services/GetDeviceType.js';
import AddDevice from '../../services/AddDevice.js';
import * as Constants from '../../Constants.js';
import GetBuildingList from '../../services/GetBuildingList.js';

/*
* Creates the page and houses the data manipulation fucntions
*/
const AddDevicePage = () => {
  // this will be appended to with custom fields from the API call when selecting the device type
  const mandatoryDeviceFields = {
    "Longitude": "",
    "Latitude": "",
    "Device Tag": "",
    "Manufacturer": "",
    "Model Number": "",
    "Serial Number": "",
    "Year Manufactured": "",
    "Notes": ""
  }
  
  const noDeviceTypeObj = { name: 'Select Device Type'};
  const noBuildingObj = { name : 'Select Building' };
  const [deviceFields, setDeviceFields] = useState(mandatoryDeviceFields);
  const [errors, setErrors] = useState({});
  const [setPageName] = useOutletContext();
  const [deviceImage, setDeviceImage] = useState(null);
  const [deviceAdditionalDocs, setAdditionalDocs] = useState([]);
  const [deviceTypes, setDeviceTypes] = useState([]);
  const [buildings, setBuildings] = useState([]);
  const [selectedDeviceType, setSelectedDeviceType] = useState(noDeviceTypeObj);
  const [selectedBuilding, setSelectedBuilding] = useState(noBuildingObj);
  const [deviceTypeDropDownStyle, setDeviceTypeDropDownStyle] = useState(styles.button);
  const [buildingDropDownStyle, setBuildingDropDownStyle] = useState(styles.button);
  
  useEffect(() => {
    setPageName('Add Device')
    const loadData = async () => {
      let types = await getDeviceTypes();
      let buildings = await getBuildingList();
      setDeviceTypes(types);
      setBuildings(buildings);
    }
   loadData()
  },[setPageName])
  
  /*
  * gets the list of buildings from the database
  * @return the object containing building list
  */
  const getBuildingList = async () => {
    const buildingList = await GetBuildingList();
    
    if(!(buildingList.status === Constants.HTTP_SUCCESS)){
      Notifications.error("Unable to get building list for dropdown", `Contact support.`);
      return;
    }
    
    let data = buildingList.response.map((item) => { return { name: item.name, id : item.id} });
    
    return data;
  }
  
  /*
  * gets the list of device types from the database
  * @return the object containing device type names and IDs
  */
  const getDeviceTypes = async () => {
    const deviceTypeData = await GetDeviceTypeList();
    
    if(!(deviceTypeData.status === Constants.HTTP_SUCCESS)){
      Notifications.error("Unable to get device type list for dropdown", `Contact support.`);
      return;
    }
    
    let data = deviceTypeData.response.map((item) => { return { name: item.name, id : item.id} });
    
    return data;
  }
  
  /*
  * gets the extra fields from the DB and adds them to the form
  * @param the type of device that was selected
  */
  const getFieldsForTypeId = async (deviceTypeId) => {
    const deviceTypeFields = await GetDeviceType(deviceTypeId);
    
    //change the dropdown text and store the fields for their keys
    setSelectedDeviceType(deviceTypeFields.response);
    setDeviceTypeDropDownStyle(styles.dropDownSelected);
    
    //retireve the values from teh response to label the form elements
    let fieldLabels = Object.values(deviceTypeFields.response.fields);
    
    //append the custom labels to the form generation object
    for(let i = 0; i < fieldLabels.length; i++) {
      deviceFields[fieldLabels[i]] = "";
    }

    //add them to the forms errors lists
    setDeviceFields(deviceFields);
    setErrors({});
  }
  
  /*
  * sets the state for the selected building from the dropdown
  */
  function changeSelectedBuilding(buildingId) {
    let building = buildings.find(buildings => {
      return buildings.id === buildingId;
    })
    setSelectedBuilding(building);
    setBuildingDropDownStyle(styles.dropDownSelected);
  }
  
  /*
  * converts the user friendly field name to the form needed by the database
  * in cases where the name needed is another key it will retrieve it from stored values
  * @param the form element's name
  * @return either the converted string or an object to get the converted string/stored value
  */
  function convertToDbFriendly(formName) {
    let result = {};
    //if it's a custom field name get custom field ID
    if(Object.values(selectedDeviceType.fields).includes(formName)){
      let dbId = Object.keys(selectedDeviceType.fields).find(key => selectedDeviceType.fields[key] === formName);
      result = {'fieldDbId': dbId};
    } else if (formName === 'Latitude' || formName === 'Longitude') {
      //put location values in their nested place
      result = {'location': {'type' : formName.toLowerCase(formName)}};
    } else {
      //make first letter lower case
      let dbKey = formName[0].toLowerCase() + formName.slice(1);
      //Number needs to be Num where applicable
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
  
  /*
  * gathers data from the form and saves the device to the DB
  * @param the Add Device button click event
  */
  async function saveDeviceToDb(addButtonEvent) {
    if (selectedDeviceType.name === 'Select Device Type'){
      Notifications.error("Device Type not selected", 'A device Type selection is required.');
      return;
    }
    
    const formFields = addButtonEvent.target.form.elements;
    const dbJson = await createJSON(formFields);
    if(dbJson){
      AddDevice(dbJson, deviceImage, deviceAdditionalDocs).then(response => {
        //reset the form or show a message regarding insertion failure
        if(response.status === Constants.HTTP_SUCCESS){
          setErrors({});
          setSelectedDeviceType(noDeviceTypeObj);
          setDeviceTypeDropDownStyle(styles.button);
          setSelectedBuilding(noBuildingObj);
          setBuildingDropDownStyle(styles.button);
          for(let i = 0; i < formFields.length; i++){
            formFields[i].value = "";
          }
          Notifications.success("Add Device Successful", "Adding Device completed successfully.");
        } else {
          Notifications.error("Unable to Add Device", `Adding Device failed.`);
        }
      })
    }
  }
  
  /*
  * gathers all the input and puts it into JSON, files are just assigned to state variables for now
  * @param the fields on the form from the button click event
  * @return the compiled JSON
  */
  async function createJSON(formFields){
    //sets up base db object/clears errors
    let dbJson = {fields: {}, location: { 'notes' : ""}};
    let newErrors = {};
    let warnings = [];
    
    //loops through visible fields on the form
    for(let i = 0; i < formFields.length; i++){
      let formName = formFields[i].name;
      let fieldNames = Object.keys(deviceFields);
      
      //lat lon validation
      if (formName === 'Latitude' || formName === 'Longitude') {
        const coordMax = formName === 'Latitude' ? Constants.MAX_LATITUDE : Constants.MAX_LONGITUDE;
        if(!(isFinite(deviceFields[formName]) && Math.abs(deviceFields[formName]) <= coordMax)) {
          newErrors[formName] = `${formName} value is invalid. Must be a decimal between -${coordMax} and ${coordMax}.`;
        }
      }

      //year manufactured validation
      if (formName === 'Year Manufactured') {
        if(formFields[i].value.match(/[^\d]/)) {
          newErrors[formName] = `${formName} value is invalid. Must be a numeric value.`;
        } else if (formFields[i].value.length < 4 && formFields[i].value.length > 0){
          newErrors[formName] = `${formName} value is invalid. Year must be 4 characters long.`;
        }
      }
      
      //construct the current JSON element to be added to the insert
      if(fieldNames.includes(formName)){
        let formJSON = '';
        let jsonKey = convertToDbFriendly(formName);
        
        if(formFields[i].value.length === 0){
          warnings.push(`${formName} field is empty<br/>`);
        }
        
        //straight append when it's not part of a nested element
        //otherwise it attaches it to the nested element
        if(!(typeof jsonKey == 'object')){
          formJSON =  {[jsonKey] : deviceFields[formName]};
          Object.assign(dbJson, formJSON);
        } else if ('fieldDbId' in jsonKey){
          dbJson.fields[jsonKey.fieldDbId] = deviceFields[formName];
        } else if ('location' in jsonKey){
          dbJson.location[jsonKey.location.type] = deviceFields[formName];
        }
      }
    }
    
    //won't deploy to azure without these until they're used elsewhere
    console.log(deviceImage);
    console.log(deviceAdditionalDocs);
    
    let isConfirmed = true;
    
    //display errors when present or attempt insert when valid data is present
    if ( Object.keys(newErrors).length > 0 ) {
      setErrors(newErrors);
      return false;
    } else if (warnings.length > 0) {
      isConfirmed = (await Notifications.warning('Warning', warnings)).isConfirmed;
    }
    
    if(isConfirmed) {
      dbJson.deviceTypeId = selectedDeviceType._id;
      dbJson.location.buildingId = selectedBuilding.id;
      
      return dbJson;
    }
  }

  const setImage = (event) => {
    if(event.target.files && event.target.files[0]){
      setDeviceImage(event.target.files[0])
    }
  }

  const setDocuments = (event) => {
    if(event.target.files){
      setAdditionalDocs(event.target.files)
    }
  }
  
  return (
    <div className={styles.fieldform}>
      <Card>
        <Card.Body>
          <Form >
            <Row className={styles.buttonGroup}>
              <Col>
                <FilledDropDown dropDownText={selectedDeviceType.name} items={deviceTypes} selectFunction={getFieldsForTypeId} buttonStyle={deviceTypeDropDownStyle} dropDownId={"typeDropDown"} />
              </Col>
              <Col>
                  <Button variant="primary" type="button" className={styles.addButton} id="addDevice" onClick={saveDeviceToDb}>
                  Add Device
                </Button>
              </Col>
            </Row>
            <br/>
            <h4>Device Image</h4>
            <ImageFileUpload type="Device Image" multiple={false} onChange={setImage}/>
            <br/>
            <h4>Additional Documents</h4>
            <ImageFileUpload type="Additional Documents" multiple={true} onChange={setDocuments}/>
            <br/>
            <h4>Fields</h4>
            <div>
              <FilledDropDown dropDownText={selectedBuilding.name} items={buildings} selectFunction={changeSelectedBuilding} buttonStyle={buildingDropDownStyle} dropDownId={"typeDropDown"} />
            </div>
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
