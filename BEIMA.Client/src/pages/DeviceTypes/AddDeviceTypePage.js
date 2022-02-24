import { useOutletContext } from 'react-router-dom';
import { useEffect, useState } from "react"
import AddDeviceTypeCard from '../../shared/AddDeviceTypeCard/AddDeviceTypeCard';
import styles from './AddDeviceTypePage.module.css'

const AddDeviceTypePage = () => {
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('Add Device Type')
  },[])

  return (
    <div className={styles.typeform}>
      <AddDeviceTypeCard/>
    </div>
  )
}

export default AddDeviceTypePage