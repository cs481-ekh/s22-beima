import { Card } from "react-bootstrap";
import './HelpPage.css'

const HelpPage = () => {
  return (
    <Card className="pageContent">
      <Card.Body>
        <h2>Overview</h2>
        The Beima App is your way to view and manage equipment all across campus. 
        <h5>Devices</h5>
      </Card.Body>
    </Card>
  );
}

export default HelpPage