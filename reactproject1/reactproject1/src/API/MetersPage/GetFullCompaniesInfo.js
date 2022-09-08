import LogOut from "../Auth/LogOut";

export default async function GetFullCompaniesInfo({setCompaniesInfo, setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');

        var resp = await fetch(`/api/meters/get_full_companies_info`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            }
        });
        if (resp.ok && resp.status != 204) {
            var _info= await resp.json();
            setCompaniesInfo(_info);
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