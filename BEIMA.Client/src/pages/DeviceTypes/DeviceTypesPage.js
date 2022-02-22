import { useEffect, useState } from "react"
import styles from './DeviceTypesPage.module.css'
import ItemList from "../../shared/ItemList/ItemList";
import { useOutletContext } from 'react-router-dom';


const DeviceTypesPage = () => {
  const [deviceTypes, setDeviceTypes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('Device Types')
  },[])
 
 
  
 
  const mockCall = async () => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(1000)
    var data = []
    for(var i = 0; i < 5; i++){
      data.push({
        deviceTypeId: i,
        name: `Test Item Type #${i}`,
        description: "A short informational description",
        numDevices: i+4,
      })
    }
    // Map data into format supported by list
    var mapped = data.map(({deviceTypeId, name, description, numDevices}) => {
      return {
        id: deviceTypeId,
        name: name,
        description: description,
        numDevices: numDevices
      }
    })
    return mapped
  }

  useEffect(async () => {
    setLoading(true)
    var types = await mockCall()
    setLoading(false)
    setDeviceTypes(types)
  },[])

  const RenderItem = (item) => {
    return (
      <div className={styles.details}>
        <div>{item.description}</div>
        <div>Number of Devices: {item.numDevices}</div>
      </div>
    )
  }

  return (
    <div >
      <div className={styles.list}>
        <ItemList title="Device Types" list={deviceTypes} RenderItem={RenderItem} loading={loading}/>
      </div>
    </div>
  )
}

export default DeviceTypesPage