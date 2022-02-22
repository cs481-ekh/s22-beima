import { useState, useEffect } from "react"
import ItemCard from "../../shared/ItemCard/ItemCard"
import styles from './DeviceTypePage.module.css'


const DeviceTypePage = () => {
  const [deviceType, setDeviceType] = useState(null)
  const [loading, setLoading] = useState(true)

  const mockCall = async () => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(1000)
    var data = {
      deviceTypeId: 54,
      name: `Test Item Type #32`,
      numDevices: 4,
      description: "The FitnessGram PACER Test is a multistage aerobic capacity test that progressively gets more difficult as it continues."
    }
    
    // Map data into format supported by list
    var mapped = {
      id: data.deviceTypeId,
      name: data.name,
      description: data.description,
      numDevices: data.numDevices
    }

    return mapped
  }

  useEffect(async () => {
    setLoading(true)
    var type = await mockCall()
    setDeviceType(type)
    setLoading(false)
  },[])

  const RenderItem = (item) => {
    return (
      <div>
        {/* <div>Number of Devices: {item.numDevices}</div> */}
        <div>{item.description}</div>
      </div>
    )
  }

  return (
    <div>
      <div className={styles.item}>
        <ItemCard 
          title={loading ? 'Loading' : deviceType.name} 
          item={deviceType}
          RenderItem={RenderItem} 
          loading={loading}
          route="/deviceTypes"
        />
      </div>
    </div>
  )
}

export default DeviceTypePage