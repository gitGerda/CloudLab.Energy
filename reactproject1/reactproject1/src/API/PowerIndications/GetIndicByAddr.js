import LogOut from '../Auth/LogOut';

export default async function GetIndicByAddr({ pageNumber,recordsCountToPage,countFlag,addresses,setIndications ,setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');
        addresses = addresses.replace(" ", "");
        addresses = addresses.split(',');
        let arr = new Array();
        addresses.map((value) => {
            arr.push(Number(value));
        });

        var resp = await fetch(`/api/indications/PowerIndications/`+
            `GetIndicByAddr?pageNumber=${pageNumber}&recordsCountToPage=${recordsCountToPage}&countFlag=${countFlag}`, {
            method:'POST',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type':'application/json'
            },
            body: JSON.stringify(arr)
        });
        if (resp.ok && resp.status != 204) {
            var indications = await resp.json();
            if (indications != null) {
                setIndications(indications);
            }
        }
        else {
            if (resp.status == 401 ) {
                alert('Ошибка аутентификации. Будет выполнен выход...');
                LogOut(setIsAuthenticated);
            }
            else {
                setIndications(null);
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