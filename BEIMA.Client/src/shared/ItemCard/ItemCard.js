import styles from './ItemCard.module.css'
import { Card, Placeholder } from 'react-bootstrap'
import { IoArrowBack } from "react-icons/io5";

import {Link} from "react-router-dom"

/**
 * Creates a list of placeholders to display
 * @returns html
 */
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

/**
 * Shared card used to contain
 * a back button and title
 * as well as display a <RenderItem/>
 * 
 * @param title: page title
 * @param RenderItem: Custom html object to display
 * @param loading: loading state
 * @param route: where to link to on click
 * @returns html
 */
export const ItemCard = ({title, RenderItem, loading, route}) => {
  return (
    <Card id="itemCard">
      <Card.Body>
        <Card.Title className={styles.cardtitle}>
          <Link to={route} className={styles.back}>
            <IoArrowBack color='#fff' size={20} />
          </Link>
          {title}
        </Card.Title>
        {loading ? <LoadingContent/> : RenderItem}
      </Card.Body>
    </Card>
  )
}
