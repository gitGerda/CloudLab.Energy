import React from "react";
import "../../../node_modules/bootstrap/dist/css/bootstrap.min.css"
import "../../../node_modules/bootstrap/js/dist/collapse"
import Accordion from "../UI/Accordion/Accordion";
import Table1 from "../UI/Tables/Table1/Table1";
import CompanyInfo from "./blocks/CompanyInfo/CompanyInfo";
import styleModule from "./CompanyMetersCard.module.css"

export default function CompanyMetersCard({headerInfo,tables,metersCount}) {
    return (
        <div id={styleModule.card} className="card text-center">
            <div id={styleModule.cardHeader} className="card-header">
                <Accordion childNodes={<CompanyInfo header={headerInfo} />} headerTextValue={headerInfo.orgGetter}/>
            </div>
            <div className="card-body">
                {tables}
            </div>
            <div id={styleModule.cardFooter}  className="card-footer ">
               Общее количество счётчиков: {metersCount}
            </div>
        </div>
    )
}