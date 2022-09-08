import LogOut from "../Auth/LogOut";

export default async function DeleteShedule({ shedule_id, setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');
        var resp = await fetch(`/api/shedules/delete_shedule?shedule_id=${shedule_id}`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            }
        });
        if (resp.ok && resp.status != 204) {
            return true;
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