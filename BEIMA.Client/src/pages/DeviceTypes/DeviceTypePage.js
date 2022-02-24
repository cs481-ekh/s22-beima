import { useState, useEffect } from "react"
import {ItemCard, ItemFieldList} from "../../shared/ItemCard/ItemCard"
import styles from './DeviceTypePage.module.css'
import { useOutletContext } from 'react-router-dom';
import { Form, Card,FormGroup, FormLabel,Button, FormControl, InputGroup, DropdownButton} from "react-bootstrap";
import { MdDelete } from "react-icons/md";
import { IoAdd } from "react-icons/io5";

const DeviceTypePage = () => {
  const [deviceType, setDeviceType] = useState(null)
  const [loading, setLoading] = useState(true)
  const [setPageName] = useOutletContext();

  useEffect(() => {
    setPageName('View Device Type')
  },[])


  const mockCall = async () => {
    const sleep = ms => new Promise(resolve => setTimeout(resolve, ms))
    await sleep(1000)
    var data = {
      deviceTypeId: 54,
      name: `Test Item Type #32`,
      description: "The FitnessGram PACER Test is a multistage aerobic capacity test that progressively gets more difficult as it continues.",
      notes: "There are no notes",
      fields: {
        Weight: "Int",
        Dimensions: "String",
        Color: "String",
        Manufactorer: "String"
      }
    }
    
    // Map data into format supported by list
    var mapped = {
      id: data.deviceTypeId,
      name: data.name,
      description: data.description,
      notes: data.notes,
      fields: data.fields
    }

    return mapped
  }

  useEffect(async () => {
    setLoading(true)
    var type = await mockCall()
    setDeviceType(type)
    setLoading(false)
  },[])




  const Field = ({field, type, editable}) => {
    return (
      <Card className={styles.field}>
        <Card.Body >
          <div className={[styles.row, 'mb-3'].join(' ')}>
            <Form.Group>
              {editable}
              <Form.Label>Field Name</Form.Label>
              <FormControl required type="text" plaintext={!editable} disabled={!editable} size="sm" placeholder="Field Name" defaultValue={field}/>
            </Form.Group>              
            
            <Form.Group>
              <Form.Label>Allowed Values</Form.Label>
              <Form.Select required size="sm" disabled={!editable} defaultValue={type}>
                <option value="String">Character and Numbers</option>
                <option value="Int">Numbers</option>
              </Form.Select>
            </Form.Group>
          </div>         
          { editable ? 
           <div className={styles.deleteButton}>
              <Button variant="danger" >
                Delete
              </Button>
            </div>
          : null}
        </Card.Body>
      </Card>
    )
  }  


  const RenderItem = ({item}) => {
    const [editable, setEditable] = useState(false)
    const [itemCopy, setItemCopy] = useState({...item})
    const [numFields, setNumFields] = useState(Object.keys(item.fields).length)

    const handleSubmit = (event) => {
      const form = event.currentTarget;
      console.log(form)
    }

    const addField = () => {
      const key = `field${numFields+1}`
      
      let fields = {...itemCopy.fields}
      fields[key] = "String"
      const temp = {...itemCopy, fields}
      setItemCopy(temp)
      setNumFields(Object.keys(temp.fields).length)
    }

    const cancel = () => {
      setItemCopy({...item})
      setNumFields(Object.keys(item.fields).length)
      setEditable(false)
    }

    return (
      <Form className={styles.form}>
        <Form.Group>
           {editable ? 
           <div>
              <Button variant="success" onClick={() => {}}>
                Save
              </Button>
              <Button variant="secondary" onClick={cancel}>
                Cancel
              </Button>
           </div>
          : 
            <Button variant="primary" onClick={() => setEditable(true)}>
              Edit
            </Button>
          }
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label><b>Description</b></Form.Label>
          <Form.Control required as="textarea" rows={3} placeholder="Device Type Description"  disabled={!editable} defaultValue={itemCopy.description}/>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label><b>Notes</b></Form.Label>
          <Form.Control required as="textarea" rows={1} placeholder="Device Type Notes"  disabled={!editable} defaultValue={itemCopy.notes}/>
        </Form.Group>

        <Form.Group  className="mb-3">
          <Form.Label><b>Manditory Fields</b></Form.Label>
          <div className={styles.fields}>
            <Field field="Manufacturer" type="String" editable={false}/>
            <Field field="Model Number" type="String" editable={false}/>
            <Field field="Serial Number" type="String" editable={false}/>
            <Field field="Year Manufactured" type="Int" editable={false}/>
          </div>
        </Form.Group>
        
        <Form.Group  className={[styles.between, "mb-3" ].join(' ')}>
          <Form.Label><b>Additional Fields</b></Form.Label>
          {editable ? 
            <Button variant="primary"  onClick={addField} >
             <IoAdd size={20} color="#fff"/>
            </Button>
          : null}
        </Form.Group>
        <Form.Group>
          <div className={styles.fields}>
            {Object.keys(itemCopy.fields).map((field, i) => <Field key={i} field={field} editable={editable} type={itemCopy.fields[field]}/>)}
          </div>  
        </Form.Group>
      </Form>
    )
  }

  return (
    <div>
      <div className={styles.item}>
        <ItemCard 
          title={loading ? 'Loading' : deviceType.name}
          RenderItem={<RenderItem item={deviceType} />} 
          loading={loading}
          route="/deviceTypes"
        />
      </div>
    </div>
  )
}

export default DeviceTypePage