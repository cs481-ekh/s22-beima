import React from 'react';
import './shared.css';

const PageTitle = (props) => {
    const { pageName } = props;
    return <h3 className="pageTitle">{pageName}</h3>;
}

export default PageTitle;