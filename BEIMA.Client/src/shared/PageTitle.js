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
    return (
        <div className="toolbar">
            <h3 className="toolbarTitle">{name}</h3>
        </div>
    )
    
    
    
}

export default PageTitle;