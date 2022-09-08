import React from "react";
import st from "../../../../styles/SideBar.module.css"

export default function LogoDetails({logo}) {
    return (
        <div className={st.logoDetails}>
            <i className="bx bx-cloud-snow"></i>
            <span className={st.logoName}>{logo}</span>
        </div>
    )
}