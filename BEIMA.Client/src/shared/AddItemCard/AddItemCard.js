import { Placeholder, Form, Card, Button } from 'react-bootstrap';
import { useCallback, useEffect, useState } from "react";
import {useDropzone} from 'react-dropzone';
import styles from './AddItemCard.module.css';

const defaultDeviceFields = {
  "Building": "",
  "Longitude": "",
  "Latitude": "",
  "Device Type": "",
  "Device Tag": "",
  "Manufacturer": "",
  "Model Number": "",
  "Serial Number": "",
  "Year Manufactured": "",
  "Notes": ""
}
const defaultDeviceImage = [];

const defaultAdditonalDocs = [];

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
        <Button variant="primary" type="submit">
          Save Device
        </Button>
      </Form> 
    </div>
  )
}

const AddItemCard = () => {
  const [deviceFields, setDeviceFields] = useState(defaultDeviceFields);
  const [deviceImage, setDeviceImage] = useState(defaultDeviceImage);
  const [deviceDocs, setDeviceDocs] = useState(defaultAdditonalDocs);

  return (
    <Card>
      <Card.Body>
        <h4>Device Image</h4>
        <ImageFileUpload type="Device Image" details="(Only .png and .jpeg files accepted)" multiple={false} acceptTypes='image/png,image/jpeg'/>
        <h4>Additional Documents</h4>
        <ImageFileUpload type="Additional Documents" multiple={true}/>
        <h4>Fields</h4>
        <FieldForm fields={Object.keys(deviceFields)}/>
      </Card.Body>
    </Card>
  )
}
export default AddItemCard