import LogOut from '../Auth/LogOut';

export default async function DeletePowerIndications({address, start_date, end_date ,setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');

        var resp = await fetch(`/api/indications/PowerIndications/`+
            `delete_indications?meter_address=${address}&start_date=${start_date}&end_date=${end_date}`, {
            method:'delete',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type':'application/json'
            }
        });
        if (resp.ok && resp.status != 204) {
            return true;
        }
        else {
            if (resp.status == 401 ) {
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