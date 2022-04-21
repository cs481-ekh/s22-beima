import sdp from '../../shared/sdp-logo-infinity.png';
import { Card } from "react-bootstrap";
import { useOutletContext } from 'react-router-dom';
import { useEffect } from 'react';
import styles from './AboutPage.module.css'

const AboutPage = () => {
  const [setPageName] = useOutletContext();

  
  useEffect(() => {
    setPageName('About')
  },[setPageName])


  return (
    <div className={styles.pageContent}>
      <Card >
        <Card.Body className={styles.container}>
          <div><img src={sdp} className="bsu-logo" alt="Boise State Senior Design Project Computer Science"/></div>

          <div>
              This website was created for a<br/>
              Boise State University<br/>
              Computer Science Senior Design Project by
          </div>
          <div>Ashlyn Adamson</div>
          <div>Keelan Chen</div>
          <div>Tom Hess</div>
          <div>Kenny Miller</div>
          <div>Joseph Moore</div>
          <div>For information about sponsoring a project go to</div>
          <div><a href="https://www.boisestate.edu/coen-cs/community/cs481-senior-design-project/">https://www.boisestate.edu/coen-cs/community/cs481-senior-design-project/</a></div>           
        </Card.Body>
      </Card>
    </div>
  );
}

export default AboutPage