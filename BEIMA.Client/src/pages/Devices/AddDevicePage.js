import { useOutletContext } from 'react-router-dom';
import { useEffect, useState } from "react"
import {  } from 'react-bootstrap';
import AddItemCard from "../../shared/AddItemCard/AddItemCard";
import styles from './AddDevicePage.module.css';

const AddDevicePage = () => {
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('Add Device')
  },[])

  return (
    <div className={styles.fieldform}>
      <AddItemCard />
    </div>
  )
}

export default AddDevicePage