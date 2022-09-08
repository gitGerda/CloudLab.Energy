import LogOut from './Auth/LogOut';
export default async function MeterInfoById({ meterID, setMeterInfo, setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');
        var resp = await fetch("/api/meters/getMeterInfo?meterID=" + meterID, {
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
        if (resp.ok) {
            var meter = await resp.json();
            if (meter != null) {
                setMeterInfo(meter);
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
    catch(Exception) {
        setError({
            isError: true,
            errorDesc: Exception.message
        });
    }
}