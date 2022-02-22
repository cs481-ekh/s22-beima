import logo from './BSU-logo.png';
import { Row, Col, Container } from 'react-bootstrap';
import './shared.css';
import {Link} from "react-router-dom"

const NavBar = () => {
  return (
    <div className="sharedNavBar">
      <Container>
        <Col>
          <Row className="logo"><img src={logo} className="bsu-logo" alt="bsu-logo" /></Row>
          <Row><div className="sharedHeader">BEIMA</div></Row>
          <Row>
            <Col className="pageLinks">
              <Row className="pageLink">
                <Link to="/devices" className="sharedText">Devices</Link>
              </Row>
              <Row className="pageLink">
                <Link to="/deviceTypes" className="sharedText">Device Types</Link>
              </Row>
              <Row className="pageLink">
                <Link to="Add Device" className="sharedText">+ Add Device</Link>
              </Row>
              <Row className="pageLink">
                <Link to="Add Device Types" className="sharedText">+ Add Device Type</Link>
              </Row>
              <Row className="pageLink">
                <Link to="help" className="sharedText">? Help</Link>
              </Row>
            </Col>
          </Row>
        </Col>
      </Container>
    </div>
  );
}

export default NavBar;