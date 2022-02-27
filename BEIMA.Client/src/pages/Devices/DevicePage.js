import { useOutletContext } from 'react-router-dom';
import { useEffect, useState, useRef } from "react"
import {ItemCard} from "../../shared/ItemCard/ItemCard"
import styles from './DevicePage.module.css'
import { Form, Card, Button, FormControl, Image} from "react-bootstrap";
import { TiDelete } from "react-icons/ti";

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
  


  const mockDeviceCall = async () => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(500)
    const result = {
      deviceId: '43adx',
      deviceTypeId: '54',
      deviceTag: 'Battery',
      manufacturer: 'Tesla',
      modelNum: '1231dx',
      serialNum: '423dss',
      yearManufactured: 2022,
      notes: 'It is very heavy',
      location: {
        buildingId: 'Student Union Building',
        notes: "",
        latitude: '43.60149984813998',
        longitude: '-116.2013672986418'
      },
      fields: {
        customFieldUid1: 'value1',
        customFieldUid2: 'value2',
        customFieldUid3: 'value3'
      }
    }
    return result
  }

  const mockDeviceImageCall = async () => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(500)
    const url = 'https://cs482storage.blob.core.windows.net/documents/EAStest_600x300.jpg?sp=r&st=2022-02-27T01:10:39Z&se=2022-09-10T08:10:39Z&sv=2020-08-04&sr=b&sig=PuJcp%2BDi0wnAZkLZuyt23Bse92vAUaY56Z3v%2F1AneIA%3D'
    return url
  }

  const mockDeviceDocumentsCall = async () => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(500)
    const docs = ['fileA', 'fileB', 'fileC']
    return docs
  }

  const mockDeviceTypeCall = async(docId) => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(500)
    const type = {
      deviceTypeId: 54,
      name: `Test Item Type #32`,
      description: "The FitnessGram PACER Test is a multistage aerobic capacity test that progressively gets more difficult as it continues.",
      notes: "There are no notes",
      fields: {
        customFieldUid1: "Dimensions",
        customFieldUid2: "Weight",
        customFieldUid3: "Color",
      }
    }
    return type
  }

  useEffect(() => {
    const loadData = async() => {
      const [device, image, documents] = await Promise.all([mockDeviceCall(), mockDeviceImageCall(), mockDeviceDocumentsCall()])
      const deviceType = await mockDeviceTypeCall(device.deviceId)
  
      setDevice(device)
      setImage(image)
      setDocuments(documents)
      setDeviceType(deviceType)
      setLoading(false)
    }
   loadData()
  },[])



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

  const FormItem = ({editable, id, label, value, onChange }) => {
    return (
      <Form.Group className="mb-3" controlId={id}>
        <Form.Label><b>{label}</b></Form.Label>
        <Form.Control required type="text" disabled={!editable} size="sm" value={value}  onChange={onChange}/>
      </Form.Group>
    )
  }

  const ImageUpload = ({editable, onImageChange}) => {
    const filetypes = ".png, .tiff, .tiff, .jpg, .jpeg, .png, .gif, .raw"
    return (
      <Form.Group className="mb-3" controlId='imageUpload'>
        <Form.Label><b>Upload Device Image</b></Form.Label>
        <Form.Control type="file" disabled={!editable} multiple={false} onChange={onImageChange} accept={filetypes}/>
      </Form.Group>
    )
  }

  const FileUpload = ({editable, onDocumentchange}) => {
    const ref = useRef();
    
    const docChange = (event) => {
      onDocumentchange(event)
      ref.current.value = ''
    }

    return (
      <Form.Group className="mb-3" controlId='fileUpload'>
        <Form.Label><b>Upload Documents</b></Form.Label>
        <Form.Control type="file" multiple={true} disabled={!editable} ref={ref} onChange={(event) => docChange(event)}/>
      </Form.Group>
    )
  }

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

  const RenderItem = ({device, setDevice, deviceType, image, setImage, documents, setDocuments}) => {
    const [editable, setEditable] = useState(false)
    
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

    const submit = () => {
      const newDevice = {
        tag:tag,
        manufacturer:manufacturer,
        modelNum:modelNum,
        serialNum:serialNum,
        yearManufactured:yearManufactured,
        notes:notes,
        location: {
          buildingId: buildingId,
          notes: locNotes,
          latitude: lat,
          longitude: long
        },
        fields : fields
      }

      const newImage = imageCopy
      const docs = docCopy
      const newDocs = addedDocs
      const delDocs = removedDocs

      // Hit endpoints here
      console.log(newDevice)
      console.log(newImage)
      console.log(docs)
      console.log(newDocs)
      console.log(delDocs)
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
      var files = event.target.files
      if(files && files.length > 0){
        var tempAdded = [...addedDocs]
        var temp = [...files]
        temp.forEach(file => {
          tempAdded.push(file)
        })
        setAddedDocs(tempAdded)
      }
    }

    const deleteDocument = (doc) => {
      var tempDocs = [...docCopy]
      var tempAdded = [...addedDocs]
      var tempDel = [...removedDocs]

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
          {editable ? 
           <div className={styles.buttonRow}>
              <Button onClick={submit}>
                Save
              </Button>
              <Button variant="secondary" onClick={cancel}>
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
              <Image src={imageCopy} fluid/>
            </Card.Body>
          </Card>
        </Form.Group>

        <ImageUpload type="Device Image" editable={editable} onImageChange={onImageChange}/>

        <Form.Group>
          <Form.Label><b>Documents</b></Form.Label>
        </Form.Group>

        <div className={[styles.fields,'mb-3'].join(' ')} id="documents">
          {docCopy.map((doc, i) => <DocumentCard key={i} editable={editable} document={doc} deleteDocument={deleteDocument}/> )}
          {addedDocs.map((file, i) => <DocumentCard key={i} editable={editable} document={file.name} deleteDocument={deleteDocument}/> )}
        </div>

        <FileUpload editable={editable} onDocumentchange={onDocumentChange}/>

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
          {Object.keys(deviceType.fields).map((key, i) => <FormCard key={i} editable={editable} id={key} label={deviceType.fields[key]} value={fields[key]} onChange={onCustomFieldChange}/>)}
        </div>
      </Form>
    )
  }

  return (
    <div className={styles.item} id="devicePageContent">
      <ItemCard 
        title={loading ? 'Loading' : `${device.deviceTag}:${device.deviceId}`}
        RenderItem={<RenderItem device={device} setDevice={setDevice} deviceType={deviceType} image={image} setImage={setImage} documents={documents} setDocuments={setDocuments}/>} 
        loading={loading}
        route="/devices"
      />
    </div>
  )
}

export default DevicePage