﻿import { useEffect, useState } from "react";
import { Collapse } from "bootstrap"

export default function CollapseElement({ classNames, collapseId, displayName, component }: props) {
    var [toggle, setToggle] = useState(false);

    useEffect(() => {
        var myCollapse = document.getElementById(collapseId)
        var bsCollapse = new Collapse(myCollapse, { toggle: false })
        toggle ? bsCollapse.show() : bsCollapse.hide()
    })

    return (
        <div className="py-2">
            <button className={"btn " + classNames} onClick={() => setToggle(toggle => !toggle)}>
                {displayName}
            </button>
            <div className="collapse" id={collapseId}>
                {component}
            </div>
        </div>
    )
}