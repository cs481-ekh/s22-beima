import { useOutletContext, useParams, useNavigate } from 'react-router-dom';
import { useEffect, useState, useRef } from "react"
import {ItemCard} from "../../shared/ItemCard/ItemCard"
import styles from './DevicePage.module.css'
import { Form, Card, Button, FormControl, Image} from "react-bootstrap";
import { TiDelete } from "react-icons/ti";
import updateDevice from "../../services/UpdateDevice.js";
import deleteDevice from "../../services/DeleteDevice.js";
import getDevice from "../../services/GetDevice.js";

const DevicePage = () => {
  const [setPageName] = useOutletContext();
  const [loading, setLoading] = useState(true);
  const [device, setDevice] = useState(null);
  const [image, setImage] = useState(null)
  const [documents, setDocuments] = useState([])
  const [deviceType, setDeviceType] = useState(null)

  useEffect(() => {
    setPageName('View Device')
  },[setPageName])
  
  const { id } = useParams();

  const DeviceCall = async () => {
    const result = await getDevice(id);
    return result.response;
  }

  const mockDeviceImageCall = async () => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(500)
    const url = ''
    return url
  }

  const mockDeviceDocumentsCall = async () => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(500)
    const docs = []
    return docs
  }

  const mockDeviceTypeCall = async(docId) => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(500)
    const type = {}
    return type
  }

  useEffect(() => {
    const loadData = async() => {
      const [device, image, documents] = await Promise.all([DeviceCall(), mockDeviceImageCall(), mockDeviceDocumentsCall()])
      const deviceType = await mockDeviceTypeCall(device.deviceId)
  
      setDevice(device)
      setImage(image)
      setDocuments(documents)
      setDeviceType(deviceType)
      setLoading(false)
    }
   loadData()
  })

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
    return (
      <Card>
        <Card.Body >
          <Form.Group className="mb-3" controlId={id}>
            <Form.Label>{label}</Form.Label>
            <FormControl required type="text" disabled={!editable} size="sm" value={value} onChange={onChange}/>
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
        <Form.Control required type="text" disabled={!editable} size="sm" value={value}  onChange={onChange}/>
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
        <Form.Control type="file" multiple={false} onChange={onImageChange} accept={filetypes}/>
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
      ref.current.value = ''
    }

    return (
      <Form.Group className="mb-3" controlId='fileUpload'>
        <Form.Label><b>Upload Documents</b></Form.Label>
        <Form.Control type="file" multiple={true} ref={ref} onChange={(event) => docChange(event)}/>
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
  const DocumentCard = ({editable, document, deleteDocument}) => {
    return (
      <Card>
        <Card.Body className={styles.documentCard}>
          {document}
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
  const RenderItem = ({device, setDevice, deviceType, image, setImage, documents, setDocuments}) => {
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

    const [imageCopy, setImageCopy] = useState(image)

    const [docCopy, setDocCopy] = useState(documents)
    const [addedDocs, setAddedDocs] = useState([])
    const [removedDocs, setRemovedDocs] = useState([])
    const navigate = useNavigate();

    const updateDeviceCall = () => {
      const newDevice = {
        _id: deviceID,
        deviceTypeId: deviceTypeID,
        deviceTag:tag,
        manufacturer:manufacturer,
        modelNum:modelNum,
        serialNum:serialNum,
        yearManufactured:yearManufactured,
        notes:notes,
        fields : fields,
        location: {
          buildingId: buildingId,
          notes: locNotes,
          latitude: lat,
          longitude: long
        }
      }

      const newImage = imageCopy
      const docs = docCopy
      const newDocs = addedDocs
      const delDocs = removedDocs

      // Hit endpoints here
      updateDevice(newDevice);
      console.log(newImage)
      console.log(docs)
      console.log(newDocs)
      console.log(delDocs)
      setEditable(false)
    }

    const deleteDeviceCall = (id) => {
      deleteDevice(id);
      navigate('/devices')
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
      setLat(device.location.latitude)
      setLong(device.location.longitude)
      setLocNotes(device.location.notes)
      setImageCopy(image)
      setDocCopy(documents)
      setAddedDocs([])
      setRemovedDocs([])
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
        setYearManufactured(value)
      } else if (target === 'locationNotes'){
        setLocNotes(value)
      } else if (target === 'deviceLatitude'){
        setLat(value)
      } else if (target === 'deviceLongitude'){
        setLong(value)
      } else if (target === 'deviceBuildingId'){
        setBuildingId(value)
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
        setImageCopy(URL.createObjectURL(event.target.files[0]))
      }
    }

    const onDocumentChange = (event) => {
      let files = event.target.files
      if(files && files.length > 0){
        let tempAdded = [...addedDocs]
        let temp = [...files]
        temp.forEach(file => {
          tempAdded.push(file)
        })
        setAddedDocs(tempAdded)
      }
    }

    const deleteDocument = (doc) => {
      let tempDocs = [...docCopy]
      let tempAdded = [...addedDocs]
      let tempDel = [...removedDocs]

      if(tempDocs.includes(doc)){
        tempDocs = tempDocs.filter(val => val !== doc)
        tempDel.push(doc)
        setDocCopy(tempDocs)
        setRemovedDocs(tempDel)
      } else {
        tempAdded = tempAdded.filter(val => val.name !== doc)
        setAddedDocs(tempAdded)
      }
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
              { imageCopy !== '' ?
              <Image src={imageCopy} fluid/>
              : "No image for device"}
            </Card.Body>
          </Card>
        </Form.Group>

        {editable ? <ImageUpload type="Device Image"  onImageChange={onImageChange}/> : null }
        

        <Form.Group>
          <Form.Label><b>Documents</b></Form.Label>
        </Form.Group>

        <div className={[styles.fields,'mb-3'].join(' ')} id="documents">
          { docCopy.length > 0 ?
          docCopy.map((doc, i) => <DocumentCard key={i} editable={editable} document={doc} deleteDocument={deleteDocument}/> )
          : "No documents for device"}
          {addedDocs.map((file, i) => <DocumentCard key={i} editable={editable} document={file.name} deleteDocument={deleteDocument}/> )}
        </div>

        {editable ? <FileUpload editable={editable} onDocumentchange={onDocumentChange}/> : null }

        <FormItem editable={editable} id="deviceNotes" label="Device Notes" value={notes} onChange={onChange}/>
        
        <Form.Group>
          <Form.Label><b>Location</b></Form.Label>
        </Form.Group>
        
        <div className={[styles.fields,'mb-3'].join(' ')}>
          <FormCard editable={editable} id="deviceBuildingId" label="Building" value={buildingId} onChange={onChange}/>
          <FormCard editable={editable} id="deviceLatitude" label="Latitude" value={lat} onChange={onChange}/>
          <FormCard editable={editable} id="deviceLongitude" label="Longitude" value={long} onChange={onChange}/>
        </div>
        <FormItem editable={editable} id="locationNotes" label="Location Notes" value={locNotes} onChange={onChange}/>

        <Form.Group>
          <Form.Label><b>Fields</b></Form.Label>
        </Form.Group>

        <div className={styles.fields}>
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
        title={loading ? 'Loading' : `${device.deviceTag} - <Device Type Name> - <Building Name>`}
        RenderItem={<RenderItem device={device} setDevice={setDevice} deviceType={deviceType} image={image} setImage={setImage} documents={documents} setDocuments={setDocuments}/>} 
        loading={loading}
        route="/devices"
      />
    </div>
  )
}

export default DevicePage