import { AddIcon, CheckCircleIcon, DownloadIcon, SettingsIcon } from "@chakra-ui/icons";
import {ListIcon, ListItem } from "@chakra-ui/react";
import React from "react";
import "../../../../../../node_modules/bootstrap/dist/css/bootstrap.min.css";
import style from "./HistoryListElement.module.css"

export default function HistoryListElement({ id, header, headerSmall, body, bodySmall,detalizVisible,report_path, onClickSpec }) {
    
    return (
        <div>
            <a href="#GraphicsComponent" className="list-group-item list-group-item-action " aria-current="true" onClick={onClickSpec}>
                <div id="historyListElementId" style={{ display: "none" }}>{id}</div>
                <div className={"d-flex w-100 justify-content-between " + style.disableClick}>
                    <h5 className="mb-1">{header}</h5>
                    <small className={style.disableClick + " " + style.time}>{headerSmall}</small>
                </div>
                
                <p className={"mb-1 "+style.disableClick} style={{ marginBottom: '0' }}>{body}</p>

                <div className={style.indicators + " "+ style.disableClick}>
                    {detalizVisible == true ? <p className={style.detalizationParag + " " + style.disableClick}><CheckCircleIcon /> Детализация</p>
                        : <p></p>}

                    {
                        report_path != "" ?
                            <p className={style.download + " " + style.disableClick}>
                                <DownloadIcon /> XML80020 отчёт
                            </p> : <p></p>
                    }
                </div>
                
                <small className={style.disableClick}>{bodySmall}</small>
                <div id="reportPathId" style={{ display: "none" }}>{report_path}</div>
            </a>
        </div>
    );

}
