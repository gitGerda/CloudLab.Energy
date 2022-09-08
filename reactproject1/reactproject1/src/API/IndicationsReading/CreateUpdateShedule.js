import LogOut from "../Auth/LogOut";

export default async function CreateUpdateShedule({data,setError, setIsAuthenticated }) {
    try {
        let token = sessionStorage.getItem('aspNetToken');

        var resp = await fetch(`/api/shedules/create_update_shedule`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });
    
        if (resp.ok && resp.status != 204) {
            return true;
        }
        else {
            if (resp.status == 401) {
                alert('Ошибка аутентификации. Будет выполнен выход...');
                LogOut(setIsAuthenticated);
                return false;
            }
            else {
                setError({
                    isError: true,
                    errorDesc: resp.statusText
                });
                return false;
            }
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


// BODY SIGNATURE
//
// public class SheduleCreateUpdateModel {
//     public int?shedule_id
//         {
//     get; set;
// }
//         public string shedule_name
// {
//     get; set;
// } = "";
//         public int communic_point_id
// {
//     get; set;
// }
//         public List < int > selected_meters_id = new List < int > ();
//         public string periodicity
// {
//     get; set;
// } = "";
//     }

