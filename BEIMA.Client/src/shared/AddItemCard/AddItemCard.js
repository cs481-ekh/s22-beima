import { Placeholder, Form } from 'react-bootstrap'
import { useEffect, useState } from "react"

  const defaultDeviceFields = {
    "Location": "",
    "Device Type": "",
    "Device Tag": "",
    "Manufacturer": "",
    "Model Number": "",
    "Serial Number": "",
    "Year Manufactured": "",
    "Notes": ""
  }
  const devicePhoto = "";

const LoadingContent = () => {
  return (
    <Placeholder animation="wave">
      <Placeholder xs={12} size="sm" bg="secondary" />
    </Placeholder>
  )
}

const FieldForm = () => {
  const [deviceFields, setDeviceFields] = useState(defaultDeviceFields);

  var temp = Object.keys(deviceFields);
  return (
    <Form>
      {Object.keys(deviceFields).map(element =>
        <Form.Group controlId={element}>
          <Form.Label>{element}</Form.Label>
          <Form.Control type="text" placeholder={element} />
        </Form.Group>
      )}
    </Form>
  )
}

export default FieldForm