import styles from './ItemCard.module.css'
import { Card, Placeholder, Button } from 'react-bootstrap'
import { IoArrowBack } from "react-icons/io5";
import {Link} from "react-router-dom"


const LoadingContent = () => {
  return (
    <Placeholder animation="wave">
       <Placeholder xs={12} size="sm" bg="secondary"/>
       <Placeholder xs={12} size="sm" bg="secondary"/>
       <Placeholder xs={12} size="sm" bg="secondary"/>
       <Placeholder xs={12} size="sm" bg="secondary"/>
    </Placeholder>
  )
}

const ItemCard = ({title, item, RenderItem, loading, route}) => {
  return (
    <Card>
      <Card.Body>
        <Card.Title className={styles.cardheader}>
          <Link to={route} className={styles.back}>
            <IoArrowBack color='#fff' size={20} />
          </Link>
          {title}
        </Card.Title>
        {loading ? <LoadingContent/> : RenderItem(item)}
      </Card.Body>
    </Card>
  )
}

export default ItemCard