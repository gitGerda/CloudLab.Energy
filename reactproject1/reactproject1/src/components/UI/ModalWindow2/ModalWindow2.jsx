import React from "react";
import style from "./ModalWindow2.module.css"

export default function ModalWindow2({ visible, setVisible, children }) {
    let modalWindowVisible = style.mainDiv2;
    if (visible == false) {
        modalWindowVisible = modalWindowVisible + " " + style.close2;
    }
    return (
        <div className={modalWindowVisible} onClick={() => setVisible(false)}>
            <div className={style.modalWindow2} onClick={(e)=>e.stopPropagation()}>
                {children}
            </div>
        </div>
    )
}