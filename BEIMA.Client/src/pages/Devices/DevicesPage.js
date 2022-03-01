import { useEffect, useState } from "react"
import styles from  "./DevicesPage.module.css"
import ItemList from "../../shared/ItemList/ItemList";
import { useOutletContext } from 'react-router-dom';
import GetDeviceList from "../../service/GetDeviceList";

const DevicesPage = () => {
  const [devices, setDevices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('Devices')
  },[setPageName])
  
  const dbCall = async () => {
    let data = await GetDeviceList();
    console.log(data);

    // Map data into format supported by list
    let mapped = data.map(item => {
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
      let devices = await dbCall()
      setLoading(false)
      setDevices(devices)
    }
    loadData()
  },[])

  /**
   * Renders a custom description of the item's fields
   * @param item: json item 
   * @returns html
   */
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