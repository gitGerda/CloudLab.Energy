import LogOut from "../Auth/LogOut";

export default async function ChangeSheduleStatus({shedule_id,status,setError,setIsAuthenticated}) {
    try {
        let token = sessionStorage.getItem('aspNetToken');
        var resp = await fetch(`/api/shedules/change_shedule_status?shedule_id=${shedule_id}&status=${status}`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            }
        });
        if (resp.ok && resp.status != 204) {
            return;
        };
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
    catch (Exception) {
        setError({
            isError: true,
            errorDesc: Exception.message
        });
    }
}