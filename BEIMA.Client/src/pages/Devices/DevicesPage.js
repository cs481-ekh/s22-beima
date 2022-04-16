import { useEffect, useState } from "react"
import styles from  "./DevicesPage.module.css"
import ItemList from "../../shared/ItemList/ItemList";
import { useOutletContext } from 'react-router-dom';
import GetDeviceList from "../../services/GetDeviceList";
import GetDeviceType from '../../services/GetDeviceType';
import GetBuilding from "../../services/GetBuilding";
import GetAllDeviceDevicesReport from "../../services/AllDevicesReport";
import { Card, Button, Row, Col, Form } from 'react-bootstrap';

const DevicesPage = () => {
  const [devices, setDevices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('Devices')
  },[setPageName])
  
  const DeviceListCall = async () => {
    let data = await GetDeviceList();
    return data.response;
  }

  useEffect(() => {
    const loadData = async () => {
      setLoading(true)
      let devices = await DeviceListCall();
      let type;
      let building;
      devices = await Promise.all(devices.map(async (item) => {
        type = await GetDeviceType(item.deviceType)
        item['deviceTypeName'] = type.status === 404 ? 'Device Type Not Found' : type.response.name;
        building = item.buildingId === null ? 'No Assigned Building' : (await GetBuilding(item.buildingId)).response.name;
        item['buildingName'] = building;
        return item;
      }));
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
    // add these back to the returned object when we can call to the buildings and device type DBs
    // <div>Location: {item.buildingName}</div>
    //<div>Device Type: {item.deviceType}</div> 
    return (
      <div className={styles.details}>
        <div>Manufacturer: {item.manufacturer}</div> 
        <div>SerialNumber: {item.serialNumber}</div> 
        <div>Notes: {item.notes}</div>
        <div>Last Modified: {item.lastModified}</div> 
      </div>
    )
  }

  return (
    <div className={styles.list} id="devicesPageContent">
      <ItemList list={devices} RenderItem={RenderItem} loading={loading} isDeviceList={true}/>
      <Button onClick={() => GetAllDeviceDevicesReport()}></Button>
    </div>
  )
}

export default DevicesPage