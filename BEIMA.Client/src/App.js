import './App.css';
import { useEffect, useState } from "react"
import {
  BrowserRouter,
  Routes,
  Navigate,
  Route,
  Outlet,
  useLocation
} from "react-router-dom";
import HomePage from './pages/Home/HomePage';
import HelpPage from './pages/Help/HelpPage';
import AddDevicePage from './pages/Devices/AddDevicePage';
import AddDeviceTypePage from './pages/DeviceTypes/AddDeviceTypePage';
import NavBar from './shared/NavBar';
import PageTitle from './shared/PageTitle';
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<AppLayout />}>
          <Route index element={<HomePage />} />

          <Route path="addDevice" element={<AddDevicePage/>}/>
          <Route path="addDeviceType" element={<AddDeviceTypePage/>}/>

          <Route path="Help" element={<HelpPage />} />
          <Route path="*" element={<Navigate to="/" />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

function AppLayout() {
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
