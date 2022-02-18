import logo from './BSU-logo.png';
import { Nav, Navbar, Container, Row, Col} from 'react-bootstrap';
import './shared.css';

const NavBar = () => {
    return (
        <div bg="light" className="sharedNavBar">
            <Container fluid>
                <Col>
                    <Row>
                        <img src={logo} className="bsu-logo" alt="bsu-logo" />  
                    </Row>
                    <Row>
                        <Navbar.Brand className="sharedHeader">BEIMA</Navbar.Brand> 
                    </Row>
                    <Row>
                        <Nav className="ml-auto">
                            <Col>
                                <Row>
                                    <Nav.Link href="devices" className="sharedText">Devices</Nav.Link> 
                                </Row>
                                <Row>
                                    <Nav.Link href="device_types" className="sharedText">Device Types</Nav.Link> 
                                </Row>
                                <Row>
                                    <Nav.Link href="add_device" className="sharedText">Add Device</Nav.Link> 
                                </Row>
                                <Row>
                                    <Nav.Link href="add_device_types" className="sharedText">Add Device Type</Nav.Link>  
                                </Row>
                                <Row>
                                    <Nav.Link href="help" className="sharedText">Help</Nav.Link>   
                                </Row>
                            </Col>
                        </Nav>   
                    </Row>
                </Col>
            </Container>
        </div>
    );
}

export default NavBar;