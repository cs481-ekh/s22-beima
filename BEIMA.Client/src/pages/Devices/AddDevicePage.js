import { useOutletContext } from 'react-router-dom';
import { useEffect, useState } from "react"
import {  } from 'react-bootstrap';
import AddDeviceCard from "../../shared/AddDeviceCard/AddDeviceCard";
import styles from './AddDevicePage.module.css';

const defaultDeviceFields = {
  "Building": "",
  "Longitude": "",
  "Latitude": "",
  "Device Type": "",
  "Device Tag": "",
  "Manufacturer": "",
  "Model Number": "",
  "Serial Number": "",
  "Year Manufactured": "",
  "Notes": ""
}

const AddDevicePage = () => {
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('Add Device')
  },[])

  return (
    <div className={styles.fieldform}>
      <AddDeviceCard fields={defaultDeviceFields}/>
    </div>
  )
}

export default AddDevicePage