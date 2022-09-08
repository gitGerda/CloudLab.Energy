import { useContext } from "react";
import { AuthContext } from "../../context/context";

export default function LogOut(setIsAuthenticated) {
    
    //const { setIsAuthenticated} = useContext(AuthContext);

    sessionStorage.setItem('isAuth', 'false');
    sessionStorage.removeItem('aspNetToken');
    sessionStorage.removeItem('authLogin');
    sessionStorage.removeItem('currentLinkName');
    setIsAuthenticated({ login: "Unknown", isAuthenticated: false });
}