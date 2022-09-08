import LogOut from "../Auth/LogOut";

export default async function GetShedulesLogs({skip, take,setLogsState, setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');

        var resp = await fetch(`/api/shedules/get_logs?skip=${skip}&take=${take}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            }
        });
        if (resp.ok && resp.status != 204) {
            var _logs = await resp.json();
            setLogsState(_logs);
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