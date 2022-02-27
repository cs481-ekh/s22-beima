import { Form } from 'react-bootstrap';

const FormList = ({fields}) => {
    return (
      <div>
        {fields.map(element =>
          <Form.Group key={element} id={element}>
            <Form.Label>{element}</Form.Label>
            <Form.Control id={"input" + element} type="text" name={element} placeholder={"Enter " + element}/>
          </Form.Group>
        )} 
      </div>
    )
}

export default FormList