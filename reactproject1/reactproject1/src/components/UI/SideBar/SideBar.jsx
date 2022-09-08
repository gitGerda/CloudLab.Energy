import React, { useContext, useEffect, useMemo, useState } from "react";
import "boxicons/css/boxicons.min.css"
import st from "../../../styles/SideBar.module.css"
import NavLinkBlank from "./decomp/NavLinkBlank";
import NavLinkSubMenu from "./decomp/NavLinkSubMenu";
import ProfileDetails from "./decomp/ProfileDetails";
import LogoDetails from "./decomp/LogoDetails";
import HomeSection from "./decomp/HomeSection";



export default function SideBar() {
    const [currentLinkName, setCurrentLinkName] = useState("");
    const subMenuIndications = [
        { pathQW: "indications/autoReadingData", nameQW: "Опрос по расписанию" },
        { pathQW: "indications/database", nameQW: "База данных" },
    ];
    const subMenuReports = [
        { pathQW: "reports/xml80020", nameQW: "Формат XML80020" },
        { pathQW: "reports/energy", nameQW: "Энергия" }
    ]
    const [homeContentHeaderStyle, setHomeContentHeaderStyle] = useState({
        marginBottom:"10px"
    })

    useEffect(() => {
        var currentLink = sessionStorage.getItem('currentLinkName');
        if (currentLink != null) {
            setCurrentLinkName(currentLink);
        }
        else {
            setCurrentLinkName("Счётчики");
        }
    },[])

    useMemo(() => {
        if (currentLinkName == "Отчёты" || currentLinkName == "Показания / Опрос по расписанию"
            || currentLinkName == "Показания / База данных" 
            ) {
            setHomeContentHeaderStyle({
                //background: "#7D6187"
                background:"#c4c4c4"
            })
        }
        else if (currentLinkName == "Счётчики") {
            setHomeContentHeaderStyle({
                background: '#C8A796',
            })
        }
        else {
            setHomeContentHeaderStyle();
        }
    
    }, [currentLinkName])
    
    function setCurrentLinkNameSpec(name) {
        sessionStorage.setItem('currentLinkName', name);
        setCurrentLinkName(name);
    }
   
    return (
        <div>
            <div className={String(st.sidebar)}>
                <LogoDetails logo="CloudLab.Energy"/>
                <ul className={st.navLinks}>
                    
                    {/* <NavLinkBlank boxIcon={"bx bx-home"} linkPath="main" linkNameStr="Главная" setCurrentLinkName={setCurrentLinkNameSpec} /> */}
                    <NavLinkBlank boxIcon={"bx bx-sitemap"} linkPath="meters" linkNameStr="Счётчики" setCurrentLinkName={setCurrentLinkNameSpec} />
                    <NavLinkSubMenu boxIcon={"bx bx-broadcast"} linkNameStr="Показания" subMenuLinks={subMenuIndications} setCurrentLinkName={setCurrentLinkNameSpec} />
                    <NavLinkBlank boxIcon={"bx bx-sitemap"} linkPath="reports" linkNameStr="Отчёты" setCurrentLinkName={setCurrentLinkNameSpec} />
                    {/* <NavLinkBlank boxIcon={"bx bx-cog"} linkPath="settings" linkNameStr="Настройки" setCurrentLinkName={setCurrentLinkNameSpec} />     */}
                </ul>
                <ProfileDetails/>
            </div>
            <HomeSection currentLinkName={currentLinkName} homeContentHeaderStyle={homeContentHeaderStyle}/>
        </div>

    )
}