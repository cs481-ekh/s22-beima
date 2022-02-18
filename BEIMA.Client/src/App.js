import logo from './logo.svg';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import PageTitle from './shared/PageTitle';
import NavBar from './shared/NavBar';
import { Row, Col, Container } from 'react-bootstrap';

function App() {
  return (
    <Row className="App">
      <Col >
        <NavBar/>
      </Col>
      <Col>
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" />
          <PageTitle pageName="Home"/>
          <p>
            Edit <code>src/App.js</code> and save to reload.
          </p>
          <a
            className="App-link"
            href="https://reactjs.org"
            target="_blank"
            rel="noopener noreferrer"
          >
            Learn React
          </a>
        </header>
      </Col>
    </Row>
  );
}

export default App;
