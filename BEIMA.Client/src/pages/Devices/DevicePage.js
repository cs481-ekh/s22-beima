import { useOutletContext, useParams, useNavigate } from 'react-router-dom';
import { useEffect, useState, useRef } from "react"
import {ItemCard} from "../../shared/ItemCard/ItemCard"
import * as Notifications from '../../shared/Notifications/Notification.js'
import styles from './DevicePage.module.css'
import { Form, Card, Button, FormControl, Image} from "react-bootstrap";
import { TiDelete } from "react-icons/ti";
import updateDevice from "../../services/UpdateDevice.js";
import deleteDevice from "../../services/DeleteDevice.js";
import getDevice from "../../services/GetDevice.js";
import GetDeviceType from '../../services/GetDeviceType';
import GetBuilding from '../../services/GetBuilding.js';
import { IoChevronBack, IoChevronForward } from "react-icons/io5";
import * as Constants from '../../Constants';
import FilledDropDown from '../../shared/DropDown/FilledDropDown.js';
import GetBuildingList from '../../services/GetBuildingList';

const noBuildingObj = { name : 'No Assigned Building' };

const DevicePage = () => {
  const [setPageName] = useOutletContext();
  const [loading, setLoading] = useState(true);
  const [device, setDevice] = useState(null);
  const [image, setImage] = useState(null)
  const [documents, setDocuments] = useState([])
  const [deviceType, setDeviceType] = useState(null)
  const [building, setBuilding] = useState(null)
  const [availableBuildings, setAvailableBuildings] = useState(null);
  const [deviceChanged, setDeviceChanged] = useState(false);

  useEffect(() => {
    setPageName('View Device')
  },[setPageName])

  const { id } = useParams();
  
  useEffect(() => {
    const loadData = async() => {
      let deviceCall = await getDevice(id);
      const device = deviceCall.response;
      const deviceType = (await GetDeviceType(device.deviceTypeId)).response;
      let building;
      if(device.location.buildingId !== null){
        building = (await GetBuilding(device.location.buildingId)).response;
      } else {
        building = noBuildingObj;
      }
      let buildingsCall = await GetBuildingList();
      const buildings = buildingsCall.response;

      setDevice(device)
      setImage(device.photo)
      setDocuments(device.files)
      setDeviceType(deviceType)
      setBuilding(building)
      setAvailableBuildings(buildings);
      setLoading(false)
      setDeviceChanged(false)
    }
   loadData();
  },[id, deviceChanged]) 

  /**
   * Renders an card styled input that lets a user change a field's input
   * 
   * @param editable: can this input be used
   * @param id: id that should be set on the input
   * @param label: label of the input
   * @param value: value of the input
   * @param onChange: function to update value of the field in higher level <RenderItem>
   * @returns 
   */
  const FormCard = ({editable, id, label, value, onChange }) => {
    const [expanded, setExpanded] = useState(false);
    const textareaRef = useRef(null);

    useEffect(() => {
      if(textareaRef != null && textareaRef.current != null){
        textareaRef.current.style.height = "0px";
        const scrollHeight = textareaRef.current.scrollHeight;
        textareaRef.current.style.height = scrollHeight + "px";
      }
    }, [value, expanded]);

    return (
      <Card className={expanded ? styles.expandedCard : ''}>
        <Card.Body >
          <Form.Group className="mb-3" controlId={id}>
            <div className={styles.row}>
              <Form.Label>{label}</Form.Label>
              <div>{expanded ? <IoChevronBack className={styles.hover} size={15} onClick={() => setExpanded(false)}/> : <IoChevronForward className={styles.hover} size={15} onClick={() => setExpanded(true)}/>}</div>
            </div>
            {expanded ? 
              <FormControl 
                required
                ref={textareaRef}
                as="textarea"
                className={styles.expandedInput} 
                disabled={!editable} size="sm" 
                value={value ?? ""} 
                onChange={onChange} 
                maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH}
              />
              :
              <FormControl 
                required 
                type="text"
                className={styles.unexpandedInput} 
                disabled={!editable} size="sm" 
                value={value ?? ""} 
                onChange={onChange} 
                maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH}
              />
            }            
          </Form.Group>                
        </Card.Body>
      </Card>
    )
  }

  /**
   * Renders a card with a dropdown field input. 
   * 
   * @param editable: can this input be used
   * @param id: id that should be set on the input
   * @param label: label of the input
   * @param dropDownText: text of the currently selected item
   * @param items: json containing objects with "id" and "name" key-value pairs
   * @param onChange: function to update value of the field in higher level <RenderItem>
   * @param buttonStyle: a CSS style to apply to the FilledDropDown
   * @returns a FormCard that has a FilledDropDown in it
   */
  const FormCardDropdown = ({ editable, id, label, dropDownText, items, onChange, buttonStyle }) => {
    return (
      <Card>
        <Card.Body >
          <Form.Group className="mb-3" controlId={id}>
            <Form.Label>{label}</Form.Label>
            <FilledDropDown editable={editable} dropDownText={dropDownText} items={items} selectFunction={onChange} buttonStyle={buttonStyle} dropDownId={"typeDropDown"} />
          </Form.Group>
        </Card.Body>
      </Card>
    )
  }

  /**
   * Renders an input that lets a user change a field's input
   * 
   * @param editable: can this input be used
   * @param id: id that should be set on the input
   * @param label: label of the input
   * @param value: value of the input
   * @param onChange: function to update value of the field in higher level <RenderItem>
   * @returns 
   */
  const FormItem = ({editable, id, label, value, onChange }) => {
    return (
      <Form.Group className="mb-3" controlId={id}>
        <Form.Label><b>{label}</b></Form.Label>
        <Form.Control required type="text" disabled={!editable} size="sm" value={value}  onChange={onChange} maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH}/>
      </Form.Group>
    )
  }

   /**
   * Creates an input that allows a user to upload an image
   * 
   * @param onImageChange: function used to update image in higher level <RenderItem>
   * @returns html
   */
  const ImageUpload = ({onImageChange}) => {
    const filetypes = ".png, .tiff, .tiff, .jpg, .jpeg, .png, .gif, .raw"
    return (
      <Form.Group className="mb-3" controlId='imageUpload'>
        <Form.Label><b>Upload Device Image</b></Form.Label>
        <Form.Control type="file" multiple={false} onChange={onImageChange} accept={filetypes} maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH}/>
      </Form.Group>
    )
  }

  /**
   * Creates an input that allows a user to upload files
   * 
   * @param onDocumentChange: function used to update documents in higher level <RenderItem>
   * @returns html
   */
  const FileUpload = ({onDocumentchange}) => {
    const ref = useRef();
    
    const docChange = (event) => {
      onDocumentchange(event)
    }

    return (
      <Form.Group className="mb-3" controlId='fileUpload'>
        <Form.Label><b>Upload Documents</b></Form.Label>
        <Form.Control type="file" multiple={true} ref={ref} onChange={(event) => docChange(event)} maxLength={Constants.MAX_INPUT_CHARACTER_LENGTH}/>
      </Form.Group>
    )
  }

  /**
   * Renders a document Tile
   * 
   * @param editable: can this input be used
   * @param document: document json
   * @param deleteDocument: function to delete document in higher level RenderItem
   * @returns html
   */
  const DocumentCard = ({editable, document, fileUrl, deleteDocument}) => {
    return (
      <Card>
        <Card.Body className={styles.documentCard}>
          <a href={fileUrl}>{document}</a>
          { editable ? 
            <TiDelete color="red" className={styles.deleteDocBtn} size={20} onClick={() => deleteDocument(document)}/>
          : null}            
        </Card.Body>
      </Card>
    )
  }

  /**
   * Renders a custom form that enables a user
   * to update a device's fields, image, and documents
   * @param device: device json
   * @param setDevice: function to set device in higher level Device Page
   * @param deviceType: device type json object
   * @param image: image url
   * @param setImage: function to set image in higher level Device Page
   * @param documents: list of document names
   * @param setDocuments: function to set document names in higher level Device Page
   * @returns 
   */
  const RenderItem = ({device, deviceType, building, image, documents}) => {
    const [editable, setEditable] = useState(false)
    
    const [deviceID] = useState(device._id)
    const [deviceTypeID] = useState(device.deviceTypeId)
    const [tag, setTag] = useState(device.deviceTag)
    const [manufacturer, setManufacturer] = useState(device.manufacturer)
    const [modelNum, setModelNum] = useState(device.modelNum)
    const [serialNum, setSerialNum] = useState(device.serialNum)
    const [yearManufactured, setYearManufactured] = useState(device.yearManufactured)
    const [notes, setNotes] = useState(device.notes)
    
    const [fields, setFields] = useState({...device.fields})

    const [buildingId, setBuildingId] = useState(device.location.buildingId)
    const [lat, setLat] = useState(device.location.latitude)
    const [long, setLong] = useState(device.location.longitude)
    const [locNotes, setLocNotes] = useState(device.location.notes)

    const [newImage, setNewImage] = useState(null)
    const [imagePreviewUrl, setImagePreviewUrl] = useState(image.fileUrl)
    const [imageCopy] = useState(image)

    const [docCopy, setDocCopy] = useState(documents)
    const [addedDocs, setAddedDocs] = useState([])
    const [removedDocs, setRemovedDocs] = useState([])
    const navigate = useNavigate();

    const [selectedBuilding, setSelectedBuilding] = useState(device.location.buildingId !== "" || device.location.buildingId !== null ? { name : building.name, id: building.id } : noBuildingObj);
    const [buildingDropDownStyle, setBuildingDropDownStyle] = useState(device.location.buildingId !== "" && device.location.buildingId !== null ? styles.dropDownSelected : styles.button);

    const updateDeviceCall = async () => {
      let warnings = [];
      let isConfirmed = true;
      
      const newDevice = {
        _id:deviceID,
        deviceTypeId:deviceTypeID,
        deviceTag:tag,
        manufacturer:manufacturer,
        modelNum:modelNum,
        serialNum:serialNum,
        yearManufactured:yearManufactured,
        notes:notes,
        fields:fields,
        location:{
          buildingId: buildingId,
          notes: locNotes,
          latitude: lat,
          longitude: long
        },
        deletedFiles: removedDocs
      }
      
       Object.entries(newDevice).forEach(entry => {
        const [key, value] = entry;
        if(value === ""){
          let capitalize = key.split(/(?=[A-Z])/);
          
          for(let i = 0; i < capitalize.length; i++) {
            capitalize[i] = (capitalize[i][0].toUpperCase() + capitalize[i].slice(1)).replace('Num', 'Number');
          }
          
          warnings.push(`"${(capitalize).join(" ")}" field is empty<br/>`);
        }
      });
      
      if( warnings.length > 0 ){
        isConfirmed = (await Notifications.multiWarning('Warning', warnings)).isConfirmed;
      }
      
      if ( isConfirmed ) {
        // Hit endpoints here
        let updateResult = await updateDevice(newDevice, newImage, addedDocs);
        if(updateResult.status === Constants.HTTP_SUCCESS){
          Notifications.success("Update Device Successful", `Device ${tag} updated successfully.`)
          setEditable(false)
          setLoading(true);
          setDeviceChanged(true);
        } else {
          Notifications.error("Unable to Update Device", `Update of Device ${tag} failed.`);
        }
      }
      

    }

    const deleteDeviceCall = async (id) => {
      let deleteNotif = await Notifications.warning("Warning: Device Deletion", `Are you sure you want to delete device ${tag}?`);
      if(deleteNotif.isConfirmed){
        let deleteResult = await deleteDevice(id);
        if(deleteResult.status === 200){
          Notifications.success("Device Deletion Successful", `Device ${tag} successfully deleted.`);
          navigate('/devices');
        } else {
          Notifications.error("Unable to Delete Device", `Deletion of Device ${tag} failed.`);
        }
      }
    }

    const cancel = () => {      
      setTag(device.deviceTag)
      setManufacturer(device.manufacturer)
      setModelNum(device.modelNum)
      setSerialNum(device.serialNum)
      setYearManufactured(device.yearManufactured)
      setNotes(device.notes)
      setFields({...device.fields})
      setBuildingId(device.location.buildingId)
      changeSelectedBuilding(device.location.buildingId);
      setBuildingDropDownStyle(device.location.buildingId !== "" && device.location.buildingId !== null ? styles.dropDownSelected : styles.button)
      setLat(device.location.latitude)
      setLong(device.location.longitude)
      setLocNotes(device.location.notes)
      setImagePreviewUrl(image.fileUrl)
      setAddedDocs([])
      setRemovedDocs([])
      setDocCopy(documents)
      setEditable(false)
    }

    const onChange = (event) => {
      const target = event.target.id
      const value = event.target.value
      if(target === 'deviceNotes'){
        setNotes(value)
      } else if (target === 'deviceTag'){
        setTag(value)
      } else if (target === 'deviceManufacturer'){
        setManufacturer(value)
      } else if (target === 'deviceModelNumber'){
        setModelNum(value)
      } else if (target === 'deviceSerialNumber'){
        setSerialNum(value)
      } else if (target === 'deviceYearManufactured'){
        //year manufactured validation
        if(value.match(/[^\d]/)) {
          return;
        }
        setYearManufactured(value)
      } else if (target === 'locationNotes'){
        setLocNotes(value)
      } else if (target === 'deviceLatitude'){
        setLat(value)
      } else if (target === 'deviceLongitude'){
        setLong(value)
      }
    }

    const onCustomFieldChange = (event) => {
      const target = event.target.id
      const value = event.target.value
      let temp = {...fields}
      temp[target] = value
      setFields(temp)
    }

    const onImageChange = (event) => {
      if(event.target.files && event.target.files[0]){
        setImagePreviewUrl(URL.createObjectURL(event.target.files[0]))
        setNewImage(event.target.files[0])
      }
    }

    const deleteImage = () => {
      let delDocs = removedDocs;
      delDocs.push(imageCopy.fileUid);

      setRemovedDocs(delDocs);
      setNewImage(null);
      setImagePreviewUrl('')
    }

    const onDocumentChange = (event) => {
      if(event.target.files){
        setAddedDocs(event.target.files)
      }
    }

    const deleteDocument = (doc) => {
      let tempDocs = [...docCopy]
      let tempNewDocs = [...addedDocs]
      let delDocs = removedDocs;

      tempDocs.map((tempDoc) => {
        if(tempDoc.fileName.includes(doc)){
          tempDocs = tempDocs.filter(val => val.fileName !== doc)
          delDocs.push(tempDoc.fileUid)
        }
        return null;
      });
      setDocCopy(tempDocs)

      tempNewDocs.map((tempDoc) => {
        if(tempDoc.fileName.includes(doc)){
          tempNewDocs = tempNewDocs.filter(val => val.fileName !== doc)
        }
        return null;
      });
      setAddedDocs(tempNewDocs)

      setRemovedDocs(delDocs)
    }

    /**
    * sets the state for the selected building from the dropdown
    * @param buildingId: the buildingId 
    */
    function changeSelectedBuilding(buildingId) {
      let building = availableBuildings.find(building => {
        return building.id === buildingId;
      })
      if (buildingId === "" || buildingId === "Select Building" || buildingId === null) {
        building = noBuildingObj;
        setBuildingDropDownStyle(styles.button);
      } else {
        setBuildingDropDownStyle(styles.dropDownSelected)
      }
      setSelectedBuilding(building);
      setBuildingId(building.id);
    }

    return (
      <Form className={styles.form}>
        <Form.Group className="mb-3">
          <Button variant="danger" id="deletebtn" onClick={() => deleteDeviceCall(id)} className={styles.deleteButton}>
              Delete Device
          </Button>
          {editable ? 
          <div className={styles.buttonRow}>
              <Button id="savebtn" onClick={updateDeviceCall}>
                Save
              </Button>
              <Button variant="secondary" id="cancelbtn" onClick={cancel}>
                Cancel
              </Button>
          </div>
          : 
            <Button variant="primary" id="editbtn" onClick={() => setEditable(true)}>
              Edit
            </Button>
          }
        </Form.Group>

        <Form.Group>
          <Form.Label><b>Device Image</b></Form.Label>
        </Form.Group>

        <Form.Group className={[styles.image, "mb-3"].join(' ')} id="imageDisplay">
          <Card>
            <Card.Body>
              { editable && (imagePreviewUrl !== '' && imagePreviewUrl !== null) ? 
                <TiDelete color="red" className={styles.deleteDocBtn} size={20} onClick={() => deleteImage()}/>
              : null}  
              { imagePreviewUrl !== '' && imagePreviewUrl !== null?
              <a target="_blank" rel="noopener noreferrer" href={imagePreviewUrl}><Image src={imagePreviewUrl} fluid/></a>
              : "No image for device"}
            </Card.Body>
          </Card>
        </Form.Group>

        {editable ? <ImageUpload type="Device Image"  onImageChange={onImageChange}/> : null }
        

        <Form.Group>
          <Form.Label><b>Documents</b></Form.Label>
        </Form.Group>

        <div className={[styles.fields,'mb-3'].join(' ')} id="documents">
          {docCopy.length === 0 && addedDocs.length === 0 ?
          "No documents for device"
          : <></>}
          {docCopy.length > 0 ?
          docCopy.map((doc) => <DocumentCard key={doc.fileName} editable={editable} document={doc.fileName} fileUrl={doc.fileUrl} deleteDocument={deleteDocument}/> )
          : <></>}
          {addedDocs.length > 0 ?
          Array.from(addedDocs).map((file, i) => <DocumentCard key={i} editable={editable} document={file.name} deleteDocument={deleteDocument}/> )
          : <></>}
        </div>

        {editable ? <FileUpload editable={editable} onDocumentchange={onDocumentChange}/> : null }

        <FormItem editable={editable} id="deviceNotes" label="Device Notes" value={notes} onChange={onChange}/>
        
        <Form.Group>
          <Form.Label><b>Location</b></Form.Label>
        </Form.Group>
        
        <div className={[styles.fields,'mb-3'].join(' ')}>
          <FormCardDropdown editable={editable} id="deviceBuilding" label="Building" dropDownText={selectedBuilding.name} items={availableBuildings} onChange={changeSelectedBuilding} buttonStyle={buildingDropDownStyle}></FormCardDropdown>
          <FormCard editable={editable} id="deviceLatitude" label="Latitude" value={lat} onChange={onChange} />
          <FormCard editable={editable} id="deviceLongitude" label="Longitude" value={long} onChange={onChange}/>
        </div>
        <FormItem editable={editable} id="locationNotes" label="Location Notes" value={locNotes} onChange={onChange}/>

        <Form.Group>
          <Form.Label><b>Fields</b></Form.Label>
        </Form.Group>

        <div className={styles.fields} id="fields">
          <FormCard editable={editable} id="deviceTag" label="Tag" value={tag} onChange={onChange}/>
          <FormCard editable={editable} id="deviceModelNumber" label="Model Number" value={modelNum} onChange={onChange}/>
          <FormCard editable={editable} id="deviceSerialNumber" label="Serial Number" value={serialNum} onChange={onChange}/>
          <FormCard editable={editable} id="deviceManufacturer" label="Manufacturer" value={manufacturer} onChange={onChange}/>
          <FormCard editable={editable} id="deviceYearManufactured" label="Year Manufactured" value={yearManufactured} onChange={onChange}/>
          { Object.keys(deviceType).length > 0 ?
          Object.keys(deviceType.fields).map((key, i) => <FormCard key={i} editable={editable} id={key} label={deviceType.fields[key]} value={fields[key]} onChange={onCustomFieldChange}/>)
          : null}
        </div>
      </Form>
    )
  }

  return (
    <div className={styles.item} id="devicePageContent">
      <ItemCard 
        title={loading ? 'Loading' : `${device.deviceTag} - ${deviceType.name} - ${building.name}`}
        RenderItem={<RenderItem device={device} deviceType={deviceType} building={building} image={image} documents={documents}/>} 
        loading={loading}
        route="/devices"
      />
    </div>
  )
}

export default DevicePage