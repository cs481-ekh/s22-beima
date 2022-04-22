import { Card } from "react-bootstrap";
import { useOutletContext } from 'react-router-dom';
import { useEffect } from 'react';
import styles from './HelpPage.module.css'
import manual from './BEIMA_User_Manual.pdf'

const HelpPage = () => {
  const [setPageName] = useOutletContext();

  
  useEffect(() => {
    setPageName('Help')
  },[setPageName])


  return (
    <Card className={styles.pageContent}>
      <Card.Body>
        <h2>Overview</h2>
        <a target="_blank" rel="noopener noreferrer" href={manual}>User Manual</a>
        <div className={styles.section}>
          The BEIMA App is your way to view and manage equipment all across campus. This page is your guide to helping you use the app to its fullest potential. 
          Below are explanations of each tool at your disposal within the app. See the link above for the whole User Manual.
        </div>
        <h5>Devices</h5>
        <div className={styles.section}>
          The Devices page is where you can see a list of all the devices on campus. On it, you can filter the records as well as click on an individual item to
          see more of its attributes.
        </div>
        <h5>Device Types</h5>
        <div className={styles.section}>
          The Device Types page is where you can see and manage a list of all the different categories of devices. Each category has an associated set of fields that
          will exist on every device associated with it. This allows for a great deal of flexibility as well as ease the process of adding a new device record. When adding
          a new device, the selected device type you use will auto-populate all the required fields that will need to be filled out. 
        </div>
        <h5>Add Device</h5>
        <div className={styles.section}>
          The Add Device page is where you will go when you want to add a new device to the BEIMA application. Simply select a device type and fill out the required fields before
          submitting the final record.
        </div>
        <h5>Add Device Type</h5>
        <div className={styles.section}> 
          The Add Device Type page is where you will go when you want to add a new device type. On the page, you can create a custom template that will define what fields are required
          for devices to be registered under it.
        </div>
      </Card.Body>
    </Card>
  );
}

export default HelpPage