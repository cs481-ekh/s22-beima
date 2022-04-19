import { Form } from 'react-bootstrap';
import * as Constants from '../../Constants'

const FormList = ({fields, errors, changeHandler}) => {
    return (
      <div>
        {fields.map(element =>
          <Form.Group key={element} id={element}>
            <Form.Label>{element}</Form.Label>
            {element.toString().toLowerCase().includes("year") ?
              <Form.Control id={"input" + element} type="text" name={element} placeholder={"Enter " + element} isInvalid={errors[element]} maxLength={Constants.MAX_YEAR_LENGTH} onChange={changeHandler}/>
            : <Form.Control as="textarea" rows="1" id={"input" + element} type={element.toLowerCase().includes("password") ? 'password' : 'text'} name={element} placeholder={"Enter " + element} isInvalid={errors[element]} maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH} onChange={changeHandler}/>}
              <Form.Control.Feedback type='invalid'> { errors[element]}</Form.Control.Feedback>
          </Form.Group>
        )} 
      </div>
    )
}

export default FormList