import { useOutletContext } from 'react-router-dom';
import { useEffect, useState } from "react"

const AddDeviceTypePage = () => {
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('Add Device Type')
  },[])

  return (
    <div>Add Device Type Page</div>
  )
}

export default AddDeviceTypePage