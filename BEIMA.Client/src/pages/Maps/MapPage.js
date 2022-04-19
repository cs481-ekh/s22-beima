import React, {useState, useEffect} from 'react';
import styles from './MapPage.module.css'
import GetDeviceList from '../../services/GetDeviceList';
import { useOutletContext } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import ReactMapGL, {Marker} from 'react-map-gl';
import 'mapbox-gl/dist/mapbox-gl.css';

/**
 * React component for creating the map page.
 * @returns The map page content.
 */
const MapPage = () => {
  const [devices, setDevices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [setPageName] = useOutletContext();

  const [viewport, setViewport] = useState({
    latitude: 43.60361004262107,
    longitude: -116.20004380745182,
    zoom: 15
  });

  const navigate = useNavigate();

  const DeviceListCall = async () => {
    let data = await GetDeviceList();
    return data.response;
  }

  useEffect(() => {
    setPageName('Map')
  }, [setPageName]);

  useEffect(() => {  
    const loadData = async () => {
      setLoading(true)
      let devices = await DeviceListCall();
      // Create device markers to place on the map.
      devices = await Promise.all(devices.map(device => 
                                  <Marker key={device.id} latitude={device.latitude} longitude={device.longitude}>
                                    <div className={styles.markerContainer} onClick={() => navigate(`/devices/${device.id}`)}>
                                      <span className={styles.markerText}>{device.deviceTag}</span>
                                    </div>
                                  </Marker>
      ));
      setLoading(false)
      setDevices(devices)
    }
    loadData()
  },[]);

  return (
    <div className={styles.mapDiv}>
      <ReactMapGL
        {...viewport}
        mapboxAccessToken={process.env.REACT_APP_MAPBOX_TOKEN}
        mapStyle={"mapbox://styles/mapbox/streets-v11"}
        onDrag={(viewport) => {setViewport(viewport)}}
        onZoom={(viewport) => {setViewport(viewport)}}
      >{devices}</ReactMapGL>
    </div>
  );
}

export default MapPage;