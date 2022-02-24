import { useOutletContext } from 'react-router-dom';
import { useEffect, useState } from "react"
import FieldForm from "../../shared/AddItemCard/AddItemCard";

const AddDevicePage = () => {
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('Add Device')
  },[])

  return (
    <div>
      <FieldForm/>
    </div>
  )
}

export default AddDevicePage