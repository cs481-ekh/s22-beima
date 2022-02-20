import React from 'react';
import './shared.css';

const PageTitle = (props) => {
    const { pageName } = props;
    var name;
    if (pageName === '') {
        name = "Home";
    } else {
        name = pageName;
    }
    return <h3 className="pageTitle">{name}</h3>;
}

export default PageTitle;