import { green } from "@mui/material/colors";
import React from "react";
import style from './EllipseBtnWithRightImg.module.css'

export default function EllipseBtnWithRightImg({imgSrc, btnColor, btnTxt, title,txtFontStyle, txtFontWeight,onClick, margin=0}) {
    return (
      <button className={style.mainBtn} style={{ 'background': btnColor, borderColor: btnColor,margin:margin}} onClick={onClick} title={title}>
            <img className={style.img} src={imgSrc} />
        <span className={style.txt} style={{ fontStyle: txtFontStyle, fontWeight: txtFontWeight}}>{btnTxt}</span>    
      </button>  
    );
}