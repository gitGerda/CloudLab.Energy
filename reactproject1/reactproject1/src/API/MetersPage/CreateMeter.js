import LogOut from "../Auth/LogOut";

export default async function CreateMeter({meter_form_data,setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');

        var resp = await fetch(`/api/meters/create_meter`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + token,
            },
            body:meter_form_data
        });
        if (resp.ok && resp.status != 204) {
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