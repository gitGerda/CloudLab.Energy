import { React, useContext, useEffect, useState } from "react";
import st from "../../../../styles/SideBar.module.css"
import { AuthContext} from "../../../../context/context";
import LogOut from "../../../../API/Auth/LogOut";
import { useToast } from "@chakra-ui/react";

export default function ProfileDetails() {
    const [userName, setUserName] = useState("");
    const { setIsAuthenticated } = useContext(AuthContext);
    const toast = useToast();

    useEffect(() => {
        let name = sessionStorage.getItem('authLogin');
        setUserName(name);
    });

    function logOutLocal() {
        toast({
            title: 'Выполняется выход...',
            position: 'bottom',
            isClosable: 'true',
            duration: 1000
        });
        //LogOut(setIsAuthenticated);
        //<LogOut></LogOut>
        LogOut(setIsAuthenticated);
    }

    return (
        <div className={st.profileDetails}>
            <div className={st.profileContent}>
                <img src="/images/profile.svg" alt="profile" />
            </div>

            <div className={st.nameJob}>
                <div className={st.profileName}>{userName}</div>
                <div className={st.job}>{""}</div>
            </div>
            <i id={st.logOutButton} className='bx bx-log-out' onClick={logOutLocal}></i>
        </div>
    )
}