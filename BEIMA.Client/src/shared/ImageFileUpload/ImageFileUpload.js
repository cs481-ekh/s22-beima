import { Form } from 'react-bootstrap';

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