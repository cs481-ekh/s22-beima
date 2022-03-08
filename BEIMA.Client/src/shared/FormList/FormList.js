import { Form } from 'react-bootstrap';

const FormList = ({fields, errors}) => {
    return (
      <div>
        {fields.map(element =>
          <Form.Group key={element} id={element}>
            <Form.Label>{element}</Form.Label>
        <Form.Control id={"input" + element} type="text" name={element} placeholder={"Enter " + element} isInvalid={errors[element]}/>
              <Form.Control.Feedback type='invalid'> { errors[element] }</Form.Control.Feedback>
          </Form.Group>
        )} 
      </div>
    )
}

export default FormList