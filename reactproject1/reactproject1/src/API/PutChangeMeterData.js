import LogOut from './Auth/LogOut';

export default async function PutChangeMeterData({ formData, setError, setSuccess, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');

        if (token != null) {
            let response = await fetch("/api/meters/changeMeterData", {
                method: 'POST',
                headers: {
                    'Authorization': 'Bearer ' + token
                },
                body: formData
            });
            if (response.ok == false) {
                if (response.status == 401) {
                    alert('Ошибка аутентификации. Будет выполнен выход...');
                    LogOut(setIsAuthenticated);
                }
                else {
                    setError({
                        isError: true,
                        errorDesc: response.statusText
                    });
                }
            }
            else {
                setSuccess({
                    isSuccess: true,
                    successDesc: "Изменения выполнены успешно"
                });
            };
        }
        else {
            LogOut(setIsAuthenticated);
            setError({
                isError: true,
                errorDesc: 'Ошибка аутентификации'
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