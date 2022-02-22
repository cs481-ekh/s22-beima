import { Card, Placeholder, ListGroup, ListGroupItem } from "react-bootstrap"
import { MdEditNote } from "react-icons/md";
import { IoChevronDownSharp, IoChevronForward } from "react-icons/io5";
import { useState } from "react"
import { useNavigate } from "react-router-dom";
import styles from './ItemList.module.css'

const LoadingItem = () => {
  return (
    <div className="loadingItem">
      <Placeholder xs={12} size="sm" bg="secondary"/>
    </div>    
  )
}

const LoadingItemList = () => {
  var rows = []
  for(var i = 0; i < 6; i++){
    rows.push(<LoadingItem key={i}/>)
  }
  return (
    <Placeholder as={ListGroup} animation="wave">
        {rows}
    </Placeholder>
  )
}


const Item = ({item, RenderItem}) => {
  const [visable, setVisibility] = useState(false);
  let navigate = useNavigate();

  return (
    <div className={styles.item}>
      <div className={styles.row}> 
        <div className={styles.itemName}>{item.name}</div>
        <MdEditNote color='#f44336' className={styles.hover} size={30} onClick={() => navigate(`${item.id}`)}/>
      </div>

      <div className={styles.itemContent}>
        <div className={[styles.row, styles.hover, styles.smallFont].join(' ')} onClick={() => setVisibility(!visable)}>
          {!visable ? "Show Details" : "Hide Details"}
          {visable ? <IoChevronDownSharp size={15}/> : <IoChevronForward size={15}/>}
        </div>
        <div className={visable ? null: styles.hidden}>
          {RenderItem(item)}   
        </div>
      </div>
    </div>
  )
}

const List = ({list, RenderItem}) => {
  return (
    <div>
      {list.map((item, i) => 
        <Item item={item} RenderItem={RenderItem} key={i}/>
      )}
    </div>
  )
}

/**
 * Generic List with loading animation
 * shared between DeviceTypes and Devices pages
 * Uses a custom render item defined in each
 * page for the details component
 * @param title: title of the list
 * @param list: list of data to display
 * @param renderItem : details render object
 * @param loading: loading status
 */
const ItemList = ({title, list, RenderItem, loading}) => {
  return (
    <Card className="card">
      <Card.Body>
        <Card.Title>{title}</Card.Title>
        {loading ? <LoadingItemList/>: <List list={list} RenderItem={RenderItem}/>}
      </Card.Body>
    </Card>
  )
}

export default ItemList