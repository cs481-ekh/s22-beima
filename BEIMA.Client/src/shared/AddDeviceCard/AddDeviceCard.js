import { Placeholder, Form, Card, Button, Dropdown } from 'react-bootstrap';
import { useCallback, useEffect, useState } from "react";
import {useDropzone} from 'react-dropzone';
import styles from './AddDeviceCard.module.css';

const defaultDeviceImage = [];

const defaultAdditionalDocs = [];

const LoadingContent = () => {
  return (
    <Placeholder animation="wave">
      <Placeholder xs={12} size="sm" bg="secondary" />
    </Placeholder>
  )
}

const ImageFileUpload = ({type, details, multiple, acceptTypes}) => {
  const {acceptedFiles, getRootProps, getInputProps} = useDropzone({
    accept: acceptTypes,
    multiple: multiple,
    noDrag: true
  });

  const fileList = acceptedFiles.map(file => (
      <li key={file.name}>
        {file.name}
      </li>
  ));

  return (
    <div>
      <section className={styles.fileupload}>
        <div {...getRootProps({className: 'dropzone'})}>
          <input {...getInputProps()} />
          <div>Click to select {type} {details}</div>
        </div>
      </section>
      <aside>
        <h6>{type} Uploaded:</h6>
        <ul>{fileList}</ul>
      </aside>
    </div>
  );
}

const FieldForm = ({fields}) => {
  return (
    <div>
      <Form>
        {fields.map(element =>
          <Form.Group>
            <Form.Label>{element}</Form.Label>
            <Form.Control type="text" placeholder={"Enter " + element} />
          </Form.Group>
        )}
        <br/>
        <Button variant="primary" type="submit" className={styles.addButton}>
          Add Device
        </Button>
      </Form> 
    </div>
  )
}

const AddDeviceCard = ({fields}) => {
  const [deviceFields, setDeviceFields] = useState(fields);
  const [deviceImage, setDeviceImage] = useState(defaultDeviceImage);
  const [deviceDocs, setDeviceDocs] = useState(defaultAdditionalDocs);

  return (
    <div>
      <Card>
        <Card.Body>
          <Dropdown>
            <Dropdown.Toggle variant="success" id="dropdown-basic" className={styles.button}>
              Select Device Type 
            </Dropdown.Toggle>
            <Dropdown.Menu>
              <Dropdown.Item href="#/action-1">Default Device Type</Dropdown.Item>
            </Dropdown.Menu>
          </Dropdown>
          <br/>
          <h4>Device Image</h4>
          <ImageFileUpload type="Device Image" details="(Only .png and .jpeg files accepted)" multiple={false} acceptTypes='image/png,image/jpeg'/>
          <h4>Additional Documents</h4>
          <ImageFileUpload type="Additional Documents" multiple={true}/>
          <h4>Fields</h4>
          <FieldForm fields={Object.keys(deviceFields)}/>
        </Card.Body>
      </Card>
    </div>
    
  )
}
export default AddDeviceCard