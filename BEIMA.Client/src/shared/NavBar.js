import logo from './BSU-logo.png';
import { Row, Col, Container } from 'react-bootstrap';
import './shared.css';

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
                <a href="Devices" className="sharedText">Devices</a>
              </Row>
              <Row className="pageLink">
                <a href="Device Types" className="sharedText">Device Types</a>
              </Row>
              <Row className="pageLink">
                <a href="Add Device" className="sharedText">+ Add Device</a>
              </Row>
              <Row className="pageLink">
                <a href="Add Device Types" className="sharedText">+ Add Device Type</a>
              </Row>
              <Row className="pageLink">
                <a href="Help" className="sharedText">? Help</a>
              </Row>
            </Col>
          </Row>
        </Col>
      </Container>
    </div>
  );
}

export default NavBar;