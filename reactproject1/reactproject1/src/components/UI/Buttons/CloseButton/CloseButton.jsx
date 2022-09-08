import React from "react"
import style from "./CloseButton.module.css"

export default function CloseButton({close}) {
    return (
        <button id={style.closeButton} type="button" className="close" data-dismiss="modal" aria-label="Close" onClick={close}>
            <span aria-hidden="true">&times;</span>
        </button>
    )
}


