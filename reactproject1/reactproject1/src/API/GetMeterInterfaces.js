import LogOut from './Auth/LogOut';

export default async function GetMeterInterfaces({ setMeterInterfaces, setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');
        var resp = await fetch("/api/meters/getMeterInterfaces", {
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
        if (resp.ok) {
            var meterInterfaces = await resp.json();
            if (meterInterfaces != null) {
                setMeterInterfaces(meterInterfaces);
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
            };
        }
    }
    catch (Exception) {
        setError({
            isError: true,
            errorDesc: Exception.message
        });
    }
}