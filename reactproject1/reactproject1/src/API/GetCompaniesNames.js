import LogOut from './Auth/LogOut';

export default async function GetCompaniesNames({ setCompanies, setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');
        var resp = await fetch("/api/meters/getCompanies", {
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
        if (resp.ok) {
            var companies = await resp.json();
            if (companies != null) {
                setCompanies(companies);
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