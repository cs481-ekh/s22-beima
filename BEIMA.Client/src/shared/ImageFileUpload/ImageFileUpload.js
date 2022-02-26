import { Form } from 'react-bootstrap';

// form controls for uploading images
const ImageFileUpload = ({type, multiple}) => {
  return (
    <div>
      <Form.Group key={type}>
          {multiple ?
              <Form.Control type="file" name={type} multiple={true}/>
              : <Form.Control type="file" name={type}/>
          }
      </Form.Group>
    </div>
  );
}

export default ImageFileUpload