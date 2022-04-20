import logo from './BSU-logo.png';
import { useState } from "react";
import { getCurrentUser } from '../services/Authentication';
import './shared.css';
import { Link } from "react-router-dom";
import { logout } from '../services/Authentication';
import { MdDevices, MdLogout } from "react-icons/md";
import { IoChevronForward, IoAddCircleOutline } from "react-icons/io5";
import { CgTemplate } from "react-icons/cg"
import { BiBuildings, BiHelpCircle } from "react-icons/bi"
import { BsPinMap } from "react-icons/bs"
import { HiOutlineUserGroup, HiOutlineInformationCircle } from "react-icons/hi"

const NavBar = () => {
  const [currentUser] = useState(getCurrentUser);

  return (
    <div className="sharedNavBar">
      <div className="logo"><img src={logo} className="bsu-logo" alt="Boise State University" /></div>
      <div className="navbarDivider">BEIMA</div>
      <Link to="/devices" className="sharedText">
        <div className="textIcon">
          <MdDevices/>
          <div>Devices</div>
        </div>
        <IoChevronForward/>        
      </Link>

      <Link to="/deviceTypes" className="sharedText">
        <div className="textIcon">
          <CgTemplate/>
          <div>Device Types</div>
        </div>
        <IoChevronForward/>     
      </Link>

      <Link to="/addDevice" className="sharedText">
        <div className="textIcon">
          <IoAddCircleOutline/>
          <div>Add Device</div>
        </div>
        <IoChevronForward/>   
      </Link>

      <Link to="/addDeviceType" className="sharedText">
        <div className="textIcon">
          <IoAddCircleOutline/>
          <div>Add Device Type</div>
        </div>
        <IoChevronForward/>         
      </Link>

      <Link to="/buildings" className="sharedText">
        <div className="textIcon">
          <BiBuildings/>
          <div>Buildings</div>
        </div>
        <IoChevronForward/>  
      </Link>

      <Link to="/map" className="sharedText">
        <div className="textIcon">
          <BsPinMap/>
          <div>Map</div>
        </div>
        <IoChevronForward/>  
      </Link>
      
      {currentUser.Role === 'admin' ?
        <>
          <div className="navbarDivider">Admin</div>
          <Link to="/users" className="sharedText">
            <div className="textIcon">
              <HiOutlineUserGroup/>
              <div>Users</div>
            </div>
            <IoChevronForward/>  
          </Link>
        </>
        
      :<></>}

      <div className="navbarDivider">Resources</div>
      <Link to="help" className="sharedText">
        <div className="textIcon">
          <BiHelpCircle/>
          <div>Help</div>
        </div>
        <IoChevronForward/>  
      </Link>

      <Link to="about" className="sharedText">
        <div className="textIcon">
          <HiOutlineInformationCircle/>
          <div>About</div>
        </div>
        <IoChevronForward/>  
      </Link>

      <div className="sharedText logout" onClick={() => logout()}>
        <div className="textIcon">
          <MdLogout/>
          <div>Logout</div>
        </div>
        <IoChevronForward/>  
      </div>
    </div>
  );
}

export default NavBar;