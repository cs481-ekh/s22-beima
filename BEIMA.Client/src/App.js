import './App.css';
import {
  BrowserRouter,
  Routes,
  Navigate,
  Route,
  Outlet
} from "react-router-dom";
import HomePage from './pages/Home/HomePage';
import HelpPage from './pages/Help/HelpPage';
import DevicesPage from './pages/Devices/DevicesPage'
import DeviceTypesPage from './pages/DeviceTypes/DeviceTypesPage';

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<AppLayout/>}>
          <Route index element={<HomePage/>}/>
          <Route path="devices" element={<DevicesPage/>}/>
          <Route path="deviceTypes" element={<DeviceTypesPage/>}/>
          <Route path="help" element={<HelpPage/>}/>
          <Route path="*" element={<Navigate to="/"/>}/>
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

const AppLayout = () => {
  return (
    <div>
      <div>Sidebar and navbar go here</div>
      <Outlet/>
    </div>    
  )
}

export default App;
