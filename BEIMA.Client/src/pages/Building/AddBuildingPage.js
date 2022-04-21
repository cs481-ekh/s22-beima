import { useOutletContext, Link } from 'react-router-dom';
import { IoArrowBack } from "react-icons/io5";
import { Card, Button, Form, Row, Col } from 'react-bootstrap';
import { useEffect, useState } from "react";
import styles from './AddBuildingPage.module.css';
import FormListWithErrorFeedback from '../../shared/FormList/FormListWithErrorFeedback.js';
import * as Constants from '../../Constants.js';
import AddBuilding from '../../services/AddBuilding.js';
import * as Notifications from '../../shared/Notifications/Notification.js';

const AddBuildingPage = () => {
  // this will be replaced with API call based on selected device type to get the fields
  const allBuildingFields = {
    "Name": "",
    "Number": "",
    "Longitude": "",
    "Latitude": "",
    "Notes": ""
  }

  const [buildingFields] = useState(allBuildingFields);
  const [errors, setErrors] = useState({});
  const [setPageName] = useOutletContext();
  const [location, setLocation] = useState({});
  
  useEffect(() => {
    setPageName('Add Building')
  }, [setPageName])
  
  // gathers all the input and puts it into JSON, files are just assigned to state variables for now
  async function createJSON(formFields){
    let fieldValues = {};
    let newErrors = {};
    let warnings = [];

    for(let i = 0; i < formFields.length; i++){
      let formName = formFields[i].name;
      let fieldNames = Object.keys(buildingFields);
      
      if(fieldNames.includes(formName)){
        let formJSON;

        if(formFields[i].value.length === 0){
          warnings.push(`"${formName}" field is empty<br/>`);
        }
        
        //lat lon validation
        if (formName === 'Latitude' || formName === 'Longitude') {
          const coordMax = formName === 'Latitude' ? Constants.MAX_LATITUDE : Constants.MAX_LONGITUDE;
          if(!(isFinite(formFields[i].value) && Math.abs(formFields[i].value) <= coordMax)) {
            newErrors[formName] = `${formName} value is invalid. Must be a decimal between -${coordMax} and ${coordMax}.`;
          } else {
            let newLocation = Object.assign(location, {[formName] : formFields[i].value});
            setLocation(newLocation);
          }
        } else {
          formJSON =  {[formName] : formFields[i].value};
        }
        Object.assign(fieldValues, formJSON);
      }
    }
    fieldValues["Location"] = location;

    let isConfirmed = true;
    if ( Object.keys(newErrors).length > 0 ) {
      setErrors(newErrors);
      return;
    } else if (warnings.length > 0){
      isConfirmed = (await Notifications.multiWarning('Warning', warnings)).isConfirmed;
    } 
    
    if(isConfirmed) {
      setErrors({});
      return fieldValues;
    }
  }
    
  async function saveBuildingToDB(addButtonEvent){
    let formFields = addButtonEvent.target.form.elements;
    let fullJSON = await createJSON(formFields);
  
    if(fullJSON && Object.keys(errors).length === 0){
      AddBuilding(fullJSON).then(response => {
        if(response.status === Constants.HTTP_SUCCESS){
          for(let i = 0; i < formFields.length; i++){
            formFields[i].value = "";
          }
          Notifications.success("Add Building Successful", "Adding Building completed successfully.");
        } else {
          Notifications.error("Unable to Add Building", "Adding Building failed.");
        }
      })
    } 
  }
     
  return (
    <div className={styles.fieldform}>
      <Card>
        <Card.Body>
          <Form>
            <Row>
              <Col>
                <Link to="/buildings" className={styles.back} id="backBuildings">
                  <IoArrowBack color='#fff' size={20} />
                </Link>
              </Col>
              <Col>
                <Button variant="primary" type="button" className={styles.addButton} id="addBuilding" onClick={(event) => saveBuildingToDB(event)}>
                  Add Building
                </Button>
              </Col>
            </Row>
            <br/>
            <h4>Building Fields</h4>
            <div>
              <FormListWithErrorFeedback fields={Object.keys(buildingFields)} errors={errors} />
            </div>
          </Form>
        </Card.Body>
      </Card>
    </div>
  )
}
export default AddBuildingPage
