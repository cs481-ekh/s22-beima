import './App.css';
import { useState } from "react"
import {
  BrowserRouter,
  Routes,
  Navigate,
  Route,
  Outlet
} from "react-router-dom";
import { getCurrentUser } from './services/Authentication';
import HelpPage from './pages/Help/HelpPage';
import DevicesPage from './pages/Devices/DevicesPage'
import DevicePage from './pages/Devices/DevicePage'
import DeviceTypesPage from './pages/DeviceTypes/DeviceTypesPage';
import DeviceTypePage from './pages/DeviceTypes/DeviceTypePage';
import AddDevicePage from './pages/Devices/AddDevicePage';
import AddDeviceTypePage from './pages/DeviceTypes/AddDeviceTypePage';
import LoginPage from './pages/Authentication/LoginPage';
import BuildingListPage from './pages/Building/BuildingListPage';
import AddBuildingPage from './pages/Building/AddBuildingPage';
import BuildingPage from './pages/Building/BuildingPage';
import ListUsersPage from './pages/Users/ListUsersPage';
import AddUserPage from './pages/Users/AddUserPage';
import UserPage from './pages/Users/UserPage';
import NavBar from './shared/NavBar';
import PageTitle from './shared/PageTitle';
import 'bootstrap/dist/css/bootstrap.min.css';

const App = () => {
  const [currentUser] = useState(getCurrentUser());

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<AppLayout />}>
          {currentUser.token !== '' ?
          <>
          <Route index element={<Navigate to="/devices" />}/>
          <Route path="addDevice" element={<AddDevicePage/>}/>
          <Route path="addDeviceType" element={<AddDeviceTypePage/>}/>
          <Route path="devices" element={<DevicesPage/>}/>  
          <Route path="devices/:id" element={<DevicePage/>}/>
          <Route path="deviceTypes" element={<DeviceTypesPage/>}/>  
          <Route path="deviceTypes/:typeId" element={<DeviceTypePage/>}/>
          <Route path="buildings/:id" element={<BuildingPage/>}/>
          <Route path="buildings" element={<BuildingListPage/>}/>
          <Route path="buildings/addBuilding" element={<AddBuildingPage/>}/> 
          {currentUser.Role === 'admin' ?
          <>
          <Route path="users" element={<ListUsersPage/>}/>
          <Route path="users/addUser" element={<AddUserPage/>}/>
          <Route path="users/:id" element={<UserPage/>}/>
          </>
          :<></>} 
          <Route path="Help" element={<HelpPage />} />
          <Route path="*" element={<Navigate to="/devices" />} />
          </>
          : <>
          <Route index element={<Navigate to="/login" />}/>
          <Route path="login" element={<LoginPage/>}/>
          <Route path="*" element={<Navigate to="/login" />} />
          </>}
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

const AppLayout = () => {
  const [pageName, setPageName] = useState('')
  const [currentUser] = useState(getCurrentUser());

  return (
    <div className="page">
      {currentUser.token === '' ?
      <></>
      : <NavBar />}
      <div className="content">
        <PageTitle pageName={pageName} />
        <Outlet context={[setPageName]}/>
      </div>
    </div>
  )
}

export default App;
