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
  },[setPageName])
 
  const mockCall = async () => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(1000)
    let data = []
    for(let i = 0; i < 5; i++){
      data.push({
        deviceTypeId: i,
        name: `Test Item Type #${i}`,
        description: "A short informational description",
        numDevices: i+4,
      })
    }
    // Map data into format supported by list
    let mapped = data.map(({deviceTypeId, name, description, numDevices}) => {
      return {
        id: deviceTypeId,
        name: name,
        description: description,
        numDevices: numDevices
      }
    })
    return mapped
  }

  useEffect(() => {
    const loadData = async () => {
      setLoading(true)
      let types = await mockCall()
      setLoading(false)
      setDeviceTypes(types)
    }
   loadData()
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
    <div className={styles.list} id="deviceTypesContent">
      <ItemList list={deviceTypes} RenderItem={RenderItem} loading={loading}/>
    </div>
  )
}

export default DeviceTypesPage