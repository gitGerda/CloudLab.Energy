import React from "react";
import style from './SearchCustomElement1.module.css'

export default function SearchCustomElement1({placeholder,fontSize,onkeyup,defValue}) {
    return (
        <div>
            <input className={style.input} placeholder={placeholder} style={{ fontSize: fontSize }} onKeyUp={onkeyup} defaultValue={defValue}>

            </input>
        </div>
    )
}