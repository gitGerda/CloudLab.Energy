import React from "react";
import "../../../../node_modules/bootstrap/dist/css/bootstrap.min.css"
import "../../../../node_modules/bootstrap/js/dist/collapse"
import styleModule from "./Accordion.module.css"

function collapseButton(e)
{
    let accordionItemClass = e.target.parentElement.parentElement.parentElement.className;
    document.getElementsByClassName(accordionItemClass)[0].childNodes[1].classList.toggle("show");
}

export default function Accordion({ headerTextValue, childNodes,style1,style2,styleBody }) {
    const currentDateTime = (new Date()).getTime().toString();
    return (
        <div id={styleModule.accordion} className="accordion-flush">
            <div className={String("accordion-item " + styleModule.myAccordionItem + " "+ currentDateTime)} >
                <h2 className={String("accordion-header " + styleModule.myAccordionHeader)} id="headingOne" style={style2}>
                    <button id={styleModule.accordionButton} className="accordion-button" onClick={collapseButton} style={style1} >
                        <div id={styleModule.headerText}>{headerTextValue}</div>
                    </button>
                </h2>
                <div id="collapseOne" className="accordion-collapse collapse " aria-labelledby="headingOne" data-bs-parent="#accordionExample">
                    <div className="accordion-body" style={styleBody}>
                        {childNodes}
                    </div>
                </div>
            </div>
        </div>
    )
}