import LogOut from "../Auth/LogOut";

export default async function DeleteCommunicPoint({ communic_point_id,setResponse, setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');
        var resp = await fetch(`/api/communicpoints/delete_communic_point?communic_point_id=${communic_point_id}`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            }
        });
        if (resp.ok && resp.status != 204) {
            var result = await resp.json();
            setResponse(result);
            return;
        }
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