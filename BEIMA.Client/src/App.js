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
import NavBar from './shared/NavBar';
import PageTitle from './shared/PageTitle';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<AppLayout/>}>
          <Route index element={<HomePage/>}/>
          <Route path="Help" element={<HelpPage/>}/>
          <Route path="*" element={<Navigate to="/"/>}/>
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

function AppLayout(){
  var location  = useLocation();
  var path = location.pathname;
  path = path.replace('/','');
  return (
    <div className="page">
      <NavBar/>
      <div className="content">
        <PageTitle pageName={path}/>
        <Outlet/>
      </div>
    </div>
  )
}

export default App;
