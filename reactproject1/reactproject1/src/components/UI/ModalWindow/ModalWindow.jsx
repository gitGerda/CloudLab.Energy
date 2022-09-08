import React from "react";
import style from "./ModalWindow.module.css"

export default function ModalWindow({visible,setVisible,children,custom_container_style}) {
    let modalWindowVisible = style.mainDiv;
    if (visible==false)
    {
        modalWindowVisible = modalWindowVisible + " " + style.close;
    }
    return (
        <div className={modalWindowVisible}  onClick={() => setVisible(false)}>
            <div className={style.modalWindow} style={custom_container_style} onClick={(e)=>e.stopPropagation()}>
                {children}
            </div>
        </div>
    )
}