import React from "react";
import style from "./ErrorPage.module.css"

export default function ErrorPage({errorName,errorDesc}) {
    return (
        <div className={style.errorMain}>
            <div className={style.errorSpace}>
                <span className={style.errorName}>{errorName} error</span>
                <span className={style.errorDesc}>{errorDesc}</span>
            </div>
        </div>
    )
}