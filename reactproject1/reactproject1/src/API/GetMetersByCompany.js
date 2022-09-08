import LogOut from './Auth/LogOut';

export default async function GetMetersByCompany({ companyName, setMeters, setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');
        var resp = await fetch("/api/meters/getMetersByCompany/?company="+companyName, {
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
        if (resp.ok) {
            var meters = await resp.json();
            if (meters != null) {
                setMeters(meters);
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