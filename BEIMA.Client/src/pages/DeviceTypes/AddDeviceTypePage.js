import { useOutletContext } from 'react-router-dom';
import { useEffect, useState } from "react"
import AddDeviceTypeCard from '../../shared/AddDeviceTypeCard/AddDeviceTypeCard';

const AddDeviceTypePage = () => {
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('Add Device Type')
  },[])

  return (
    <div>
      <AddDeviceTypeCard/>
    </div>
  )
}

export default AddDeviceTypePage