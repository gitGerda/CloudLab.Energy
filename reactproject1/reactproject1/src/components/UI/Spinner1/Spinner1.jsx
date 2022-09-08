import React from "react";
import style from "./Spinner1.module.css"

export default function Spinner1({isLoading, color, position,top,left})
{
    let spinnerPrefix="";
    if (!isLoading) {
        spinnerPrefix = style.closeSpinner;
    }

    return (
        <div id={style.container} className={String("spinner "+spinnerPrefix)} style={{position:{position},top:{top},left:{left}}}>
            <svg viewBox="0 0 100 100">
                <defs>
                    <filter id="shadow">
                        <feDropShadow dx="0" dy="0" stdDeviation="1.5"
                            floodColor="#fc6767" />
                    </filter>
                </defs>
                <circle id={style.spinner} cx="50" cy="50" r="45" style={{ stroke: color}}/>
            </svg>
        </div>
    )
}