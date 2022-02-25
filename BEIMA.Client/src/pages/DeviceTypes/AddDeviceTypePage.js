import { useOutletContext } from 'react-router-dom';
import { useEffect, useState } from "react"
import AddDeviceTypeCard from '../../shared/AddDeviceTypeCard/AddDeviceTypeCard';
import styles from './AddDeviceTypePage.module.css'

const defaultDeviceFields = [
  "Building",
  "Longitude",
  "Latitude",
  "Location Notes",
  "Device Type",
  "Device Tag",
  "Manufacturer",
  "Model Number",
  "Serial Number",
  "Year Manufactured",
  "Device Notes"
]

const typeAttributes = {
    "Name": "",
    "Description": "",
    "Device Type Notes": ""
}

const AddDeviceTypePage = () => {
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('Add Device Type')
  },[])

  return (
    <div className={styles.typeform}>
      <AddDeviceTypeCard attributes={typeAttributes} fields={defaultDeviceFields}/>
    </div>
  )
}

export default AddDeviceTypePage