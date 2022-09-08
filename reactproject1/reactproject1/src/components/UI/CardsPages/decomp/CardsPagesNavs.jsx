import { background } from "@chakra-ui/react";
import React, { useState } from "react";
import "../../../../../node_modules/bootstrap/dist/css/bootstrap.min.css"
import style from "./CardsPagesNavs.module.css"

export default function CardsPagesNavs({ navClassName, navName, setNavActive}) {

    return (
        <li className="nav-item">
            <button className={navClassName +" "+ style.btn} onClick={setNavActive}>{navName}</button>
        </li>
    )
}

/* 
    navClassName
         nav-link
         nav-link active
*/