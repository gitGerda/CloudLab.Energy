import LogOut from "../Auth/LogOut";

export default async function GetSheduleSettingsMeters({ shedule_id = null, setMeters, setError, setIsAuthenticated}) {
    try {
        let token = sessionStorage.getItem('aspNetToken');

        let request = shedule_id == null ? '/api/shedules/get_shedule_settings_meters'
            : `/api/shedules/get_shedule_settings_meters?shedule_id=${shedule_id}`;

        var resp = await fetch(request, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            }
        });
        if (resp.ok && resp.status != 204) {
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
                setMeters([]);
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