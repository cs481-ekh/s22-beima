import { useEffect, useState, useMemo, useRef } from "react"
import styles from  "./DevicesPage.module.css"
import ItemList from "../../shared/ItemList/ItemList";
import { useOutletContext } from 'react-router-dom';
import { Button } from "react-bootstrap";
import GetDeviceList from "../../services/GetDeviceList";
import GetDeviceType from '../../services/GetDeviceType';
import GetBuilding from "../../services/GetBuilding";
import GetBuildingList from '../../services/GetBuildingList.js';
import * as Notifications from '../../shared/Notifications/Notification.js';
import GetDeviceTypeList from '../../services/GetDeviceTypeList.js';
import GetAllDeviceDevicesReport from "../../services/AllDevicesReport";
import * as Constants from '../../Constants.js';
import Select from 'react-select';

/*
  * gets the list of buildings from the database
  * @return the object containing building list
  */
const getBuildingList = async () => {
  const buildingList = await GetBuildingList();  
  if(!(buildingList.status === Constants.HTTP_SUCCESS)){
    Notifications.error("Unable to get building list for dropdown", `Contact support.`);
    return;
  }
  
  let data = buildingList.response.map((item) => { return { label: item.name, value : item.id} });  
  return data;
}

/*
* gets the list of device types from the database
* @return the object containing device type names and IDs
*/
const getDeviceTypes = async () => {
  const deviceTypeData = await GetDeviceTypeList();  
  if(!(deviceTypeData.status === Constants.HTTP_SUCCESS)){
    Notifications.error("Unable to get device type list for dropdown", `Contact support.`);
    return;
  }
  
  let data = deviceTypeData.response.map((item) => { return { label: item.name, value : item.id} });  
  return data;
}

/*
* generates a zip containing a file entry per device type
* filled with all devices associated with it
* @return object containing status of the request
*/
const generateAllDevicesReport = async () => {
  const reportResult = await GetAllDeviceDevicesReport();
  if(reportResult.status !== Constants.HTTP_SUCCESS){
    Notifications.error("Unable to generate devices report", `Contact support.`);
  }
}

const DeviceFilter = ({loading, filterCallback}) => {
  const [deviceTypes, setDeviceTypes] = useState([])
  const [buildings, setBuildings] = useState([])

  const deviceTypeSelect = useRef(null)
  const buildingSelect = useRef(null)
  
  useEffect(() => {
    const fetchdata = async () => {
      let results = await Promise.all([getDeviceTypes(), getBuildingList()])
      setDeviceTypes(results[0])
      setBuildings(results[1])      
    }
    fetchdata();
  }, [])

  const clearFilters = () => {
    deviceTypeSelect.current.clearValue()
    buildingSelect.current.clearValue()
  }

  const mapSelectedFilters = (select) => {
    let values = select.current.getValue()
    let ids = values.map(val => val.value)
    return ids
  }

  const customStyles = useMemo(() => ({
    valueContainer: (provided, state) => ({
        ...provided,
        whiteSpace: "break-spaces",
        overflow: "hidden",
        width: "100%"
    }),
    multiValue: (provided, state) => ({
      ...provided,
      whiteSpace: "break-spaces",
      overflow: "hidden",
      "backgroundColor": "#fafafa",
      border: "1px solid rgba(0,0,0,.125)"
    }),
    multiValueLabel: (styles, { data }) => ({
      ...styles,
      whiteSpace: "break-spaces",
      overflow: "hidden",
    })
  }), []);

  return (
    <div id="filterContainer">
        <h5>Filter Options</h5>
        <div className={styles.row}>
          <div className={styles.container}>
            <label className={styles.filterLabel}>Device Types</label>
            <Select
              inputId="deviceTypeFilter"
              closeMenuOnSelect={false}
              isMulti
              options={deviceTypes}
              styles={customStyles}
              ref={deviceTypeSelect}
            />
          </div>
          <div className={styles.container}>
            <label className={styles.filterLabel}>Buildings</label>
            <Select
              inputId="buildingFilter"
              closeMenuOnSelect={false}
              isMulti
              options={buildings}
              styles={customStyles}
              ref={buildingSelect}
            />
          </div>
          <Button id="submitFilterButton" 
            disabled={loading} 
            onClick={() => filterCallback(mapSelectedFilters(deviceTypeSelect), mapSelectedFilters(buildingSelect))}
          >Apply Filters</Button>
          <Button id="clearFilterButton" onClick={() => {clearFilters(); filterCallback([],[])}}>Clear Filters</Button>
          <Button id="generateReportButton" className={styles.reportButton} onClick={() => generateAllDevicesReport()}>Generate All Devices Report</Button>
        </div>
    </div>
  )
}

const DevicesPage = () => {
  const [devices, setDevices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [setPageName] = useOutletContext();

  const [deviceTypeFilters, setDeviceTypeFilters] = useState([])
  const [buildingFilters, setBuildingFilters] = useState([])

  const DeviceListCall = async (deviceTypeIds, buildingIds) => {
    let data = await GetDeviceList(deviceTypeIds, buildingIds);
    return data.response;
  }

  const filterData = async(deviceTypeIds, buildingIds) => {
    setDeviceTypeFilters(deviceTypeIds)
    setBuildingFilters(buildingIds)
  }

  useEffect(() => {
    setPageName('Devices')
  }, [setPageName])

  useEffect(() => {  
    const loadData = async () => {
      setLoading(true)
      let devices = await DeviceListCall(deviceTypeFilters, buildingFilters);
      devices = await Promise.all(devices.map(async (item) => {
        let type = await GetDeviceType(item.deviceType)
        item['deviceTypeName'] = type.status === 404 ? 'Device Type Not Found' : type.response.name;
        let building = item.buildingId === null ? 'No Assigned Building' : (await GetBuilding(item.buildingId)).response.name;
        item['buildingName'] = building;
        return item;
      }));
      setLoading(false)
      setDevices(devices)
    }
    loadData()
  },[deviceTypeFilters, buildingFilters])

  /**
   * Renders a custom description of the item's fields
   * @param item: json item 
   * @returns html
   */
  const RenderItem = (item) => {
    // add these back to the returned object when we can call to the buildings and device type DBs
    //<div>Device Type: {item.deviceType}</div>
    return (
      <div className={styles.details}>
        <div>Manufacturer: {item.manufacturer}</div> 
        <div>SerialNumber: {item.serialNumber}</div> 
        <div>Notes: {item.notes}</div>
        <div>Last Modified: {item.lastModified}</div> 
        <div>Location: {item.buildingName}</div>
      </div>
    )
  }

  return (
    <div className={styles.pageContent} id="devicesPageContent">
      <ItemList list={devices} RenderItem={RenderItem} loading={loading} isDeviceList={true} filter={<DeviceFilter filterCallback={filterData}/>}/>
    </div>
  )
}

export default DevicesPage