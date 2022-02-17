import React from 'react';

const PageTitle = (props) => {
    const { pageName } = props;
    return <h3>{pageName}</h3>;
}

export default PageTitle;