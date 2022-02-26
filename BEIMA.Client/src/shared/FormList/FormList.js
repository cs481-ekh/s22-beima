import { Form } from 'react-bootstrap';

const FormList = ({fields}) => {
    return (
      <div>
        {fields.map(element =>
          <Form.Group key={element}>
            <Form.Label>{element}</Form.Label>
            <Form.Control type="text" name={element} placeholder={"Enter " + element}/>
          </Form.Group>
        )} 
      </div>
    )
}

export default FormList