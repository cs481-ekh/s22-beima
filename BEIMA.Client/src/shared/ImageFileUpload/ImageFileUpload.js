import { Form } from 'react-bootstrap';

// form controls for uploading images
const ImageFileUpload = ({type, multiple, onChange}) => {
  return (
    <div>
      <Form.Group key={type}>
          {multiple ?
              <Form.Control type="file" name={type} multiple={true} id={type} onChange={(event) => onChange(event)}/>
              : <Form.Control type="file" name={type} id={type}  onChange={(event) => onChange(event)}/>
          }
      </Form.Group>
    </div>
  );
}

export default ImageFileUpload