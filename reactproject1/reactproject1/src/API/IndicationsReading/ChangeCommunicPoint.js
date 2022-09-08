import LogOut from "../Auth/LogOut";

export default async function ChangeCommunicPoint({communic_point_id="",port="",desc="", setError, setIsAuthenticated }) {
    try {
        if (communic_point_id == "" || port== "") {
            throw "Empty Id or Port";
        }

        let token = sessionStorage.getItem('aspNetToken');

        var resp = await fetch(`/api/communicpoints/change_communic_point?communic_point_id=${communic_point_id}&port=${port}&desc=${desc}`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            }
        });
        if (resp.ok && resp.status != 204) {
            return true; 
        }
        else {
            if (resp.status == 401) {
                alert('Ошибка аутентификации. Будет выполнен выход...');
                LogOut(setIsAuthenticated);
            }
            else {
                setError({
                    isError: true,
                    errorDesc: resp.statusText
                });
            }
            return false;
        }
    }
    catch (Exception) {
        setError({
            isError: true,
            errorDesc: Exception.message
        });
        return false;
    }
}