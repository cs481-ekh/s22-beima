import { useOutletContext, Link } from 'react-router-dom';
import { IoArrowBack } from "react-icons/io5";
import { Card, Button, Form, Row, Col } from 'react-bootstrap';
import { useEffect, useState } from "react";
import styles from './AddBuildingPage.module.css';
import FormListWithErrorFeedback from '../../shared/FormList/FormListWithErrorFeedback.js';


const AddBuildingPage = () => {
  // this will be replaced with API call based on selected device type to get the fields
  const mandatoryBuildingFields = {
    "Name": "",
    "Longitude": "",
    "Latitude": "",
    "Notes": ""
  }

  const [buildingFields] = useState(mandatoryBuildingFields);
  const [errors, setErrors] = useState(mandatoryBuildingFields);
  const [setPageName] = useOutletContext();
  const [fullBuildingJSON, setFullBuildingJSON] = useState({});
  
  useEffect(() => {
    setPageName('Add Building')
  }, [setPageName])
  
  // gathers all the input and puts it into JSON, files are just assigned to state variables for now
  function createJSON(addButtonEvent){
    let formFields = addButtonEvent.target.form.elements;
    let fieldValues = {};
    let newErrors = {};

    for(let i = 0; i < formFields.length; i++){
      let formName = formFields[i].name;
      let fieldNames = Object.keys(buildingFields);
      
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
      }
    }

    setFullBuildingJSON(fieldValues);
    
    // replace with building api calls when ready
    console.log(fullBuildingJSON);

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
            <Row>
              <Col>
                <Link to="/buildings" className={styles.back}>
                  <IoArrowBack color='#fff' size={20} />
                </Link>
              </Col>
              <Col>
                <Button variant="primary" type="button" className={styles.addButton} id="addDevice" onClick={(event) => createJSON(event)}>
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
