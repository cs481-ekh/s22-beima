import { Dropdown } from 'react-bootstrap';
import styles from './FilledDropDown.module.css'

const FilledDropDown = ({dropDownText, items, selectFunction, buttonStyle, dropDownId}) => {
  return (
    <Dropdown id={dropDownId} onSelect={selectFunction}>
      <Dropdown.Toggle variant="success" id="dropdown-basic" className={[buttonStyle, styles.dropdownButton].join(' ')}>
        {dropDownText}
      </Dropdown.Toggle>
      <Dropdown.Menu >
        {items.length > 0 &&
          items.map(item => (
          <Dropdown.Item className={styles.dropdownItem} eventKey={item.id} value={item.id} key={item.id}>{item.name}</Dropdown.Item>
        ))}
      </Dropdown.Menu>
    </Dropdown>
  )
}

export default FilledDropDown;