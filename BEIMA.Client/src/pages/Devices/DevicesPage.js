import { useEffect, useState } from "react"
import styles from  "./DevicesPage.module.css"
import ItemList from "../../shared/ItemList/ItemList";
import { useOutletContext } from 'react-router-dom';

const DevicesPage = () => {
  const [devices, setDevices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('Devices')
  },[setPageName])
  
  const mockCall = async () => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(1000)
    var data = []
    for(var i = 0; i < 5; i++){
      data.push({
        _id: i,
        name: `Test Item #${i}`,
        deviceType: "Batteries",
        buildingName: "Student Union Building",
        serialNumber: "234asfdsa",
        manufacturer: "Tesla"
      })
    }
    // Map data into format supported by list
    var mapped = data.map(item => {
      return {
        id: item._id,
        name: item.name,
        deviceType: item.deviceType,
        buildingName: item.buildingName,
        serialNumber: item.serialNumber,
        manufacturer: item.manufacturer
      }
    })
    return mapped
  }

  useEffect(() => {
    const loadData = async () => {
      setLoading(true)
      var devices = await mockCall()
      setLoading(false)
      setDevices(devices)
    }
    loadData()
  },[])

  const RenderItem = (item) => {
    return (
      <div className={styles.details}>
        <div>Location: {item.buildingName}</div>
        <div>Device Type: {item.deviceType}</div> 
        <div>Manufacturer: {item.manufacturer}</div> 
        <div>SerialNumber: {item.serialNumber}</div> 
      </div>
    )
  }

  return (
    <div className={styles.list} id="devicesPageContent">
      <ItemList list={devices} RenderItem={RenderItem} loading={loading}/>
    </div>
  )
}

export default DevicesPage