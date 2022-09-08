import LogOut from './Auth/LogOut';
export default async function GetMeterTypes({ setMetertypes, setError, setIsAuthenticated })
{
    try {
        let token = sessionStorage.getItem('aspNetToken');
        var resp = await fetch("/api/meters/getMeterTypes", {
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
        if (resp.ok) {
            var meterTypes = await resp.json();
            if (meterTypes != null) {
                setMetertypes(meterTypes);
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