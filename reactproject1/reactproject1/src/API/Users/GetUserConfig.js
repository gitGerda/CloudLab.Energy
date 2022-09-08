import LogOut from "../Auth/LogOut";

export default async function GetUserConfig({ login, configPropName, setConfig, setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');
        var resp = await fetch(`/api/users/GetUserConfig?login=${login}&propName=${configPropName}`, {
            method:"GET",
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
        if (resp.ok) {
            var userConf = await resp.json();
            if (userConf != null) {
                setConfig(userConf);
            }
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
        }
    }
    catch (Exception) {
        setError({
            isError: true,
            errorDesc: Exception.message
        });
    }
}