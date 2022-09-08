import LogOut from '../Auth/LogOut';

export default async function CreateReport({formToFetch, setResponse, setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');
        
        var resp = await fetch(`/api/ReportXML80020/createReport`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            },
            body: formToFetch
        });
        if (resp.ok && resp.status != 204) {
            var responseObj = await resp.json();
            setResponse(responseObj);
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