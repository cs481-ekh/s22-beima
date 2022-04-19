import { Dropdown } from 'react-bootstrap';
import styles from './FilledDropDown.module.css'

const FilledDropDown = ({dropDownText, items, selectFunction, buttonStyle, dropDownId, editable}) => {
  if(editable == null) {
    editable = true;
  }
  return (
    <Dropdown id={dropDownId} onSelect={selectFunction}>
      <Dropdown.Toggle disabled={!editable} variant="success" id="dropdown-basic" className={[buttonStyle, styles.dropdownButton].join(' ')}>
        {dropDownText}
      </Dropdown.Toggle>
      <Dropdown.Menu >
        {items.length > 0 &&
          items.map(item => (
          <Dropdown.Item disabled={!editable} className={styles.dropdownItem} eventKey={item.id} value={item.id} key={item.id}>{item.name}</Dropdown.Item>
        ))}
      </Dropdown.Menu>
    </Dropdown>
  )
}

export default FilledDropDown;