import { Form } from 'react-bootstrap';
import * as Constants from '../../Constants'

const FormList = ({fields}) => {
    return (
      <div>
        {fields.map(element =>
          <Form.Group key={element} id={element}>
            <Form.Label>{element}</Form.Label>
            <Form.Control id={"input" + element} type="text" name={element} placeholder={"Enter " + element} maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH}/>
          </Form.Group>
        )} 
      </div>
    )
}

export default FormList