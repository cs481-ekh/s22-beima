import { Form } from 'react-bootstrap';
import * as Constants from '../../Constants'

const FormList = ({fields, errors, changeHandler}) => {
    return (
      <div>
        {fields.map(element =>
          <Form.Group key={element} id={element}>
            <Form.Label>{element}</Form.Label>
            <Form.Control id={"input" + element} type="text" name={element} placeholder={"Enter " + element} isInvalid={errors[element]} maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH} onChange={changeHandler} />
              <Form.Control.Feedback type='invalid'> { errors[element]}</Form.Control.Feedback>
          </Form.Group>
        )} 
      </div>
    )
}

export default FormList