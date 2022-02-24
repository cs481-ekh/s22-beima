import { Placeholder, Form, Card, Button } from 'react-bootstrap';
import { useEffect, useState } from "react";
import Dropzone, {useDropzone} from 'react-dropzone';
import styles from './AddItemCard.module.css';

const fileTypes = ["JPG", "PNG"];

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
const devicePhoto = [];

const LoadingContent = () => {
  return (
    <Placeholder animation="wave">
      <Placeholder xs={12} size="sm" bg="secondary" />
    </Placeholder>
  )
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

  return (
    <Card>
      <Card.Body>
      <h4>Device Image</h4>
        <Dropzone onDrop={fileTypes => console.log(fileTypes)}>
          {({getRootProps, getInputProps}) => (
            <section>
              <div {...getRootProps()} className={styles.fileupload}>
                <input {...getInputProps()} />
                <div>Drag 'n' drop device image here, or click to select device image</div>
              </div>
            </section>
          )}
        </Dropzone>
        <br/>
        <h4>Additonal Documents</h4>
        <Dropzone onDrop={fileTypes => console.log(fileTypes)}>
          {({getRootProps, getInputProps}) => (
            <section>
              <div {...getRootProps()} className={styles.fileupload}>
                <input {...getInputProps()} />
                <div>Drag 'n' drop additional documents here, or click to select additional documents</div>
              </div>
            </section>
          )}
        </Dropzone>
        <br/>
        <h4>Fields</h4>
        <FieldForm fields={Object.keys(deviceFields)}/>
      </Card.Body>
    </Card>
  )
}

export default AddItemCard