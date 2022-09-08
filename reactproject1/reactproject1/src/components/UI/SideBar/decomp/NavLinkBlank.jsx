import React from "react";
import st from "../../../../styles/SideBar.module.css"
import { Link } from "react-router-dom";

export default function NavLinkBlank({boxIcon,linkPath,linkNameStr,setCurrentLinkName}) {
    return (
        <li>
            <Link to={linkPath} className="link" onClick={() => setCurrentLinkName(linkNameStr)}>
                <i className={boxIcon} />
                <span className={st.linkName} > {linkNameStr}</span>
            </Link>
            <ul className={String(st.subMenu + " " + st.blank)}>
                <li>
                    <Link to={linkPath} className={st.linkName} onClick={() => setCurrentLinkName(linkNameStr)}>
                        {linkNameStr}
                    </Link>
                </li>
            </ul>
        </li>
    )
}