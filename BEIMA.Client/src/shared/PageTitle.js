import React from 'react';
import './shared.css';

function CapitalizeAndAssignValue(text){
    if (text === '') {
        text = "Home";
    }
    return text.replace(/\b\w/g, l => l.toUpperCase());
}

const PageTitle = (props) => {
    const { pageName } = props;
    var name = CapitalizeAndAssignValue(pageName);
    return <h3 className="pageTitle">{name}</h3>;
}

export default PageTitle;