import './App.css';
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
import DevicesPage from './pages/Devices/DevicesPage'
import DevicePage from './pages/Devices/DevicePage'
import DeviceTypesPage from './pages/DeviceTypes/DeviceTypesPage';
import DeviceTypePage from './pages/DeviceTypes/DeviceTypePage';
import NavBar from './shared/NavBar';
import PageTitle from './shared/PageTitle';
import 'bootstrap/dist/css/bootstrap.min.css';

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<AppLayout/>}>
          <Route index element={<HomePage/>}/>
          <Route path="devices" >
            <Route index element={<DevicesPage/>}/>  
            <Route path=":id" element={<DevicePage/>}/>
          </Route>
          <Route path="deviceTypes">
            <Route index element={<DeviceTypesPage/>}/>  
            <Route path=":typeId" element={<DeviceTypePage/>}/>
          </Route>
          
          <Route path="help" element={<HelpPage/>}/>
          <Route path="*" element={<Navigate to="/"/>}/>
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

const AppLayout = () => {
  var location = useLocation();
  var path = location.pathname;
  path = path.replace('/', '');
  return (
    <div className="page">
      
      <NavBar />
      <div className="content">
        <PageTitle pageName={path} />
        <Outlet />
      </div>
    </div>
  )
}

export default App;
