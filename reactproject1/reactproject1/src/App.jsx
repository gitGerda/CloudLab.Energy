import React, { useEffect, useState } from "react";
import { BrowserRouter, Navigate, Route, Routes, useNavigate } from 'react-router-dom';
import "../src/styles/index.css";
import AnimatedStars from "./components/animatedCycle/AnimatedStars";
import CardsPages from "./components/UI/CardsPages/CardsPages";
import SideBar from './components/UI/SideBar/SideBar';
import { AuthContext } from "./context/context";
import AutoreadingPage from "./pages/Indications/AutoreadingPage/AutoreadingPage";
import DatabasePage from "./pages/Indications/DatabasePage/DatabasePage";
import LoginPage from "./pages/LoginPage/LoginPage";
import MetersPage from "./pages/MetersPage/MetersPage";
import ReportPage from "./pages/ReportPage/ReportPage";
import SettingsPage from "./pages/SettignsPage/SettingsPage";


export default function App() {
    const [AuthenticateInfo, setIsAuthenticated] = useState();

    useEffect(() => {
        if (sessionStorage.getItem('isAuth') == null) {
            sessionStorage.setItem('isAuth', 'false');
            setIsAuthenticated({
                login: "Unknown", isAuthenticated: false
            });
        };
    }, [])

    return (
        <AuthContext.Provider value={{ AuthenticateInfo, setIsAuthenticated }}>
            {sessionStorage.getItem('isAuth') == 'false' ?
                <BrowserRouter>
                    <Routes>
                        <Route index path="login" element={<LoginPage />}></Route>
                        <Route path="*" element={<Navigate to={'/login'}></Navigate>} ></Route>
                    </Routes>
                </BrowserRouter>
                :
                <BrowserRouter>
                    <Routes>
                        <Route path="account" element={<SideBar />}>
                            {/* <Route index path="main" element={<AnimatedStars />}></Route> */}
                            <Route index path="meters" element={<MetersPage></MetersPage>}></Route>
                            <Route path="indications/database" element={<DatabasePage></DatabasePage>}/>
                            <Route path="indications/autoReadingData" element={<AutoreadingPage />}/>
                            <Route path="reports" element={<ReportPage/>}></Route>
                            {/* <Route path="settings" element={<SettingsPage></SettingsPage>}></Route> */}
                        </Route>
                        <Route path="*" element={<Navigate to={'/account/meters'}></Navigate>} ></Route>
                    </Routes>
                </BrowserRouter>}
        </AuthContext.Provider>
    )
}