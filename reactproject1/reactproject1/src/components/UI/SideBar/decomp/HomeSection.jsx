import React from "react";
import { Outlet, Link } from "react-router-dom";
import st from "../../../../styles/SideBar.module.css"

function sideBarToggle() {
    let sidebar = document.querySelector("." + st.sidebar);
    sidebar.classList.toggle(st.close);
}
export default function HomeSection({ currentLinkName,homeContentHeaderStyle}) {
    return (
        <section className={st.homeSection}>
            <div className={st.homeContent} style={homeContentHeaderStyle}>
                <i id={st.menuPic} className="bx bx-menu" onClick={sideBarToggle}/>
                <span className={st.text}> {currentLinkName}</span>
            </div>

            <div className={st.homeSectionContainer}>
                <Outlet />
            </div>
            

        </section>
    )
}