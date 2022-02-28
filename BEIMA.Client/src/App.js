import './App.css';
import { useState } from "react"
import {
  BrowserRouter,
  Routes,
  Navigate,
  Route,
  Outlet,
} from "react-router-dom";
import HomePage from './pages/Home/HomePage';
import HelpPage from './pages/Help/HelpPage';
import DevicesPage from './pages/Devices/DevicesPage'
import DevicePage from './pages/Devices/DevicePage'
import DeviceTypesPage from './pages/DeviceTypes/DeviceTypesPage';
import DeviceTypePage from './pages/DeviceTypes/DeviceTypePage';
import AddDevicePage from './pages/Devices/AddDevicePage';
import AddDeviceTypePage from './pages/DeviceTypes/AddDeviceTypePage';
import NavBar from './shared/NavBar';
import PageTitle from './shared/PageTitle';
import 'bootstrap/dist/css/bootstrap.min.css';

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<AppLayout />}>
          <Route index element={<HomePage />} />
          <Route path="addDevice" element={<AddDevicePage/>}/>
          <Route path="addDeviceType" element={<AddDeviceTypePage/>}/>
          <Route path="devices" element={<DevicesPage/>}/>  
          <Route path="devices/:id" element={<DevicePage/>}/>
          <Route path="deviceTypes" element={<DeviceTypesPage/>}/>  
          <Route path="deviceTypes/:typeId" element={<DeviceTypePage/>}/>
          <Route path="Help" element={<HelpPage />} />
          <Route path="*" element={<Navigate to="/" />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

const AppLayout = () => {
  const [pageName, setPageName] = useState('')
  return (
    <div className="page">
      <NavBar />
      <div className="content">
        <PageTitle pageName={pageName} />
        <Outlet context={[setPageName]}/>
      </div>
    </div>
  )
}

export default App;
