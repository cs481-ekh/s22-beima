import { Dropdown } from 'react-bootstrap';

const FilledDropDown = ({items, selectFunction, buttonStyle}) => {
  return (
    <Dropdown onSelect={selectFunction}>
      <Dropdown.Toggle variant="success" id="dropdown-basic" className={buttonStyle}>
        Select Device Type 
      </Dropdown.Toggle>
      <Dropdown.Menu >
        {items.length > 0 &&
          items.map(item => (
          <Dropdown.Item value={item.id} key={item.id}>{item.name}</Dropdown.Item>
        ))}
      </Dropdown.Menu>
    </Dropdown>
  )
}

export default FilledDropDown;