import { Card, Placeholder, ListGroup } from "react-bootstrap"
import { MdMoreHoriz } from "react-icons/md";
import { IoChevronDownSharp, IoChevronForward } from "react-icons/io5";
import { useState } from "react"
import { useNavigate } from "react-router-dom";
import styles from './ItemList.module.css'

/**
 * Renders a custom Placeholder
 * @returns PlaceHolder
 */
const LoadingItem = () => {
  return (
    <div className="loadingItem">
      <Placeholder xs={12} size="sm" bg="secondary"/>
    </div>    
  )
}

/**
 * Creates a list of placeholders
 * @returns html
 */
const LoadingItemList = () => {
  let rows = []
  for(let i = 0; i < 6; i++){
    rows.push(<LoadingItem key={i}/>)
  }
  return (
    <Placeholder as={ListGroup} animation="wave">
        {rows}
    </Placeholder>
  )
}

/**
 * Renderes a name with a link
 * as well as the passed in RenderItem
 * using item for the data.
 * @param item
 * @param RenderItem
 * @returns html
 */
const Item = ({item, RenderItem}) => {
  const [visable, setVisibility] = useState(false);
  let navigate = useNavigate();

  return (
    <div className={styles.item}>
      <div className={styles.row}> 
        <div className={styles.itemName}>{item.deviceTag} - {"<Device Type Name>"} - {"<Building Name>"}</div>
        <MdMoreHoriz color='#f44336' className={styles.hover} size={30} onClick={() => navigate(`${item.id}`)}/>
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

/**
 * Loops through list
 * and renders an <Item/> for
 * every object in the list
 * @param list : json
 * @parm RenderItem : html model that will render one item in list
 * @returns html
 */
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
const ItemList = ({list, RenderItem, loading}) => {
  return (
    <Card className={styles.card} id="itemList">
      <Card.Body>
        {loading ? <LoadingItemList/>: <List list={list} RenderItem={RenderItem}/>}
      </Card.Body>
    </Card>
  )
}

export default ItemList