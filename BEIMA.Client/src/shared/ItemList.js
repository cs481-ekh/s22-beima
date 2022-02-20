import { Card, Accordion } from "react-bootstrap"

const ListCard = ({title, list}) => {
  return (
    <Card>
      <Card.Body>
        <Card.Title>{title}</Card.Title>
        <Accordion>
          {}

        </Accordion>
      </Card.Body>
    </Card>
  )
}

export default ListCard