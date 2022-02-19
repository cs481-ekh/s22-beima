import logo from './BSU-logo.png';
import { Row, Col} from 'react-bootstrap';
import './shared.css';

const NavBar = () => {
    return (
        <div className="sharedNavBar">
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
                            <a href="Add Device" className="sharedText">Add Device {"\xa0\xa0"}+</a>
                        </Row> 
                        <Row className="pageLink">
                            <a href="Add Device Types" className="sharedText">Add Device Type {"\xa0\xa0\xa0\xa0"}+</a>
                        </Row>
                        <Row className="pageLink">
                            <a href="Help" className="sharedText">Help {"\xa0\xa0"}?</a>
                        </Row>      
                    </Col> 
                </Row>
            </Col>
        </div>
    );
}

export default NavBar;