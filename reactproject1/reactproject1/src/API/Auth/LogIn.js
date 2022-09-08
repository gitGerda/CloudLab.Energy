export default async function LogIn(login,setError, setSuccess, setIsAuthenticated,formToFetch) {
    try {
        setError({
            isError: false,
            errorDesc: ""
        });
        setSuccess({
            isSuccess: false,
            successDesc: ""
        });

        let response = await fetch("/api/Auth/logIn", {
            method: 'POST',
            body: formToFetch
        });
        if (response.ok == false) {
            setError({
                isError: true,
                errorDesc: response.statusText
            });
        }
        else {
            await response.json().then((resp) => {
                sessionStorage.setItem(resp.name, resp.token);
                sessionStorage.setItem('isAuth', 'true');
                sessionStorage.setItem('authLogin', login);
            });
            setSuccess({
                isSuccess: true,
                successDesc: ""
            });
            setIsAuthenticated({
                login: login,
                isAuthenticated: true
            });
        };
    }
    catch (Exception) {
        setError({
            isError: true,
            errorDesc: Exception.message
        })
    }
}