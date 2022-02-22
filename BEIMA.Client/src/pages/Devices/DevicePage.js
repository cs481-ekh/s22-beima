import { useOutletContext } from 'react-router-dom';
import { useEffect, useState } from "react"

const DevicePage = () => {
  const [device, setDevice] = useState([]);
  const [loading, setLoading] = useState(true);
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('Device')
  },[])

  return (
    <div>test</div>
  )
}

export default DevicePage