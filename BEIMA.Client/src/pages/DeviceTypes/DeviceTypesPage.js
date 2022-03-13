import { useEffect, useState } from "react"
import styles from './DeviceTypesPage.module.css'
import ItemList from "../../shared/ItemList/ItemList";
import { useOutletContext } from 'react-router-dom';
import GetDeviceTypeList from '../../services/GetDeviceTypeList.js';


const DeviceTypesPage = () => {
  const [deviceTypes, setDeviceTypes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('Device Types')
  },[setPageName])
 
  const DevicesCall = async () => {
    return (await GetDeviceTypeList()).response;
  }

  useEffect(() => {
    const loadData = async () => {
      setLoading(true)
      let types = await DevicesCall()
      setLoading(false)
      setDeviceTypes(types)
    }
   loadData()
  },[])

  /**
   * Renders a custom description of the item's fields
   * 
   * @param item 
   * @returns html
   */
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
      <ItemList list={deviceTypes} RenderItem={RenderItem} loading={loading} isDeviceList={false}/>
    </div>
  )
}

export default DeviceTypesPage