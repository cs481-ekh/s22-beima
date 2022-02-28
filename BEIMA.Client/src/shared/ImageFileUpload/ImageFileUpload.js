import { Form } from 'react-bootstrap';

// form controls for uploading images
const ImageFileUpload = ({type, multiple}) => {
  return (
    <div>
      <Form.Group key={type}>
          {multiple ?
              <Form.Control type="file" name={type} multiple={true} id={type}/>
              : <Form.Control type="file" name={type} id={type}/>
          }
      </Form.Group>
    </div>
  );
}

export default ImageFileUpload