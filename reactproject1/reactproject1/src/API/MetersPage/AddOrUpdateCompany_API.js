import LogOut from "../Auth/LogOut";

export default async function AddOrUpdateCompany({company_name, company_inn, contract_number, first_report_number, first_report_number_date,setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');

        var resp = await fetch(`/api/meters/create_update_company?CompanyInn=${company_inn}&CompanyName=${company_name}&Contract=${contract_number}&MsgNumber1=${first_report_number}&Date=${first_report_number_date}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            }
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