import React from "react";
import st from "../../../../styles/SideBar.module.css"
import { Link } from "react-router-dom";

function navToggle(e) {
    let arrowParent = e.target.parentElement.parentElement;
    arrowParent.classList.toggle(st.showMenu);
}

export default function NavLinkSubMenu({ boxIcon, linkNameStr, subMenuLinks,setCurrentLinkName}) {
    return (
        <li>
            <div className={st.iocnLink}>
                <a>
                    <i className={boxIcon} />
                    <span className={st.linkName}>{linkNameStr}</span>
                </a>

                <i className={String("bx bxs-chevron-down " + st.arrow )} onClick={navToggle}></i>
            </div>
            <ul className={st.subMenu}>
                <li>
                    <a className={st.linkName}>{linkNameStr}</a>
                </li>

                {
                    subMenuLinks.map((valueX) => 
                        <li key={valueX.pathQW}>
                            <Link to={valueX.pathQW} onClick={() => setCurrentLinkName(linkNameStr+" / "+valueX.nameQW)}>{valueX.nameQW}</Link>
                        </li>
                    )
                }
            </ul>
        </li>
    )
}