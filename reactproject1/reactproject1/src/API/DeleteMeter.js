import LogOut from './Auth/LogOut';

export default async function DeleteMeter({ meterID, setError, setSuccess, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');
        let response = await fetch("/api/meters/deleteMeter?meterID="+meterID, {
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
        if (response.ok == false) {
            if (response.status == 401) {
                alert('Ошибка аутентификации. Будет выполнен выход...');
                LogOut(setIsAuthenticated);
            }
            else {
                setError({
                    isError: true,
                    errorDesc: "не возможно удалить данный счётчик, так как возможно он прикреплён к расписанию считывания показаний" 
                });
            }
        }
        else {
            setSuccess({
                isSuccess: true,
                successDesc: "Успешное удаление"
            });
        };
    }
    catch(Exception) {
        setError({
            isError: true,
            errorDesc: Exception.message
        })
    }
}