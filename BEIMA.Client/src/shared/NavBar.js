import logo from './BSU-logo.png';
import { Row, Col, Container } from 'react-bootstrap';
import { useState } from "react";
import { getCurrentUser } from '../services/Authentication';
import './shared.css';
import {Link} from "react-router-dom";
import { logout } from '../services/Authentication';

const NavBar = () => {
  const [currentUser] = useState(getCurrentUser);

  return (
    <div className="sharedNavBar">
      <Container>
        <Col>
          <div className="logo"><img src={logo} className="bsu-logo" alt="bsu-logo" /></div>
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
                <Link to="/addDevice" className="sharedText">+ Add Device</Link>
              </Row>
              <Row className="pageLink">
                <Link to="/addDeviceType" className="sharedText">+ Add Device Type</Link>
              </Row>
              <Row className="pageLink">
                <Link to="/buildings" className="sharedText">Buildings</Link>
              </Row>
              <Row className="pageLink">
                <Link to="/map" className="sharedText">Map</Link>
              </Row>
              {currentUser.Role === 'admin' ?
              <Row className="pageLink">
                <Link to="/users" className="sharedText">Users</Link>
              </Row>
              :<></>}
              <Row className="pageLink">
                <Link to="help" className="sharedText">? Help</Link>
              </Row>
              <Row className="pageLink">
                <Link to="about" className="sharedText">About</Link>
              </Row>
              <Row className="pageLink">
                <div className="sharedText logout" onClick={() => logout()}>Logout</div>
              </Row>
            </Col>
          </Row>
        </Col>
      </Container>

    </div>
  );
}

export default NavBar;