import { Button, Spinner, useToast } from "@chakra-ui/react";
import React, { useEffect, useState, useMemo, useContext } from "react";
import LogIn from "../../API/Auth/LogIn";
import { AuthContext } from "../../context/context";
import style from "./LoginPage.module.css"



export default function LoginPage() {
    const toast = useToast();
    const { AuthenticateInfo, setIsAuthenticated } = useContext(AuthContext);
    const [loginActive, setLoginActive] = useState(style.isActive);
    const [signUpActive, setSignUpActive] = useState("");
    const [errorState, setErrorState] = useState({
        isError: false,
        errorDesc: ""
    });
    const [successState, setSuccessState] = useState({
        isSuccess: false,
        successDesc: ""
    });

    function loginBtnClick(e) {
        e.preventDefault();
        var form = e.target.form;
        var authData = new FormData(form);
        let continueFlag = false;
        authData.forEach((value, key) => {
            if (value == "") {
                toast({
                    title: "Предупреждение",
                    description: 'Заполните поле ' + key,
                    position: 'bottom',
                    isClosable: true,
                    status: 'warning',
                    duration: 3000
                });
                continueFlag = true;
            };
        })
        if (continueFlag == true) {
            return;
        };
        toast({
            title: "Попытка входа...",
            position: 'bottom',
            isClosable: true,
            status: 'info',
            duration: 1000
        });
        LogIn(authData.get('login'),setErrorState, setSuccessState, setIsAuthenticated,authData);
    }

    useEffect(() => {
        if (errorState.isError == true) {
            toast({
                title: "Ошибка входа",
                description: 'Проверьте логин и пароль',
                position: 'bottom',
                isClosable: true,
                status: 'error',
                duration: 3000
            });
        }
    }, [errorState.isError]);

    useEffect(() => {
        if (successState.isSuccess == true) {
            toast({
                title: "Успешно",
                position: 'bottom',
                isClosable: true,
                status: 'info',
                duration: 3000
            });
        }
    }, [successState.isSuccess])


    return (
        <div className={style.loginPageMain}>
            <section className={style.formsSection}>
                <div className={style.specDiv}>
                    <div className={style.loginTitle}>
                        <i className="bx bx-cloud-snow"></i>
                        <h1 className={style.sectionTitle}>CloudLab.Energy</h1>
                    </div>

                    <div className={style.forms}>
                        <div className={style.formWrapper + " " + loginActive}>
                            <button type="button"
                                className={style.switcher + " " + style.switcherLogin}
                                onClick={(e) => {
                                    if (loginActive == "") {
                                        setLoginActive(style.isActive);
                                        setSignUpActive("");
                                    }
                                }}>Вход<span className={style.underline}></span></button>
                            <form id="loginForm" className={style.form + " " + style.formLogin}>
                                <fieldset form="loginForm">
                                    <legend>Please, enter your email and password for login.</legend>
                                    <div className={style.inputBlock}>
                                        <label htmlFor="login-email">Логин</label>
                                        <input id="login-email" type='text' name="login" defaultValue={""} required />
                                    </div>
                                    <div className={style.inputBlock}>
                                        <label htmlFor="login-password">Пароль</label>
                                        <input id="login-password" type="password" name="pwd" defaultValue={""} required />
                                    </div>
                                </fieldset>
                                <button type="submit" className={style.btnLogin} onClick={loginBtnClick}>Войти</button>
                               
                            </form>
                        </div>
                        <div className={style.formWrapper + " " + signUpActive}>
                            <button type="button" className={style.switcher + " " + style.switcherSignup}
                                onClick={() => {
                                    if (signUpActive == "") {
                                        setLoginActive("");
                                        setSignUpActive(style.isActive)
                                    }
                                }}
                            >
                                Регистрация
                                <span className={style.underline}></span>
                            </button>
                            <form className={style.form + " " + style.formSignup}>
                                <fieldset>
                                    <legend>Please, enter your email, password and password confirmation for sign up.</legend>
                                    <div className={style.singUpMess}>Для регистрации в CloudLab<br />обратитесь
                                        к администрации сервиса</div>
                                    <div className={style.inputBlock}>
                                        <label htmlFor="signup-email">Логин</label>
                                        <input id="signup-email" required readOnly />
                                    </div>
                                    <div className={style.inputBlock}>
                                        <label htmlFor="signup-password">Пароль</label>
                                        <input id="signup-password" type="password" required readOnly />
                                    </div>
                                    <div className={style.inputBlock}>
                                        <label htmlFor="signup-password-confirm">Подтверждение пароля</label>
                                        <input id="signup-password-confirm" type="password" required readOnly />
                                    </div>
                                </fieldset>
                                <button type="submit" className={style.btnSignup} disabled={true}>Продолжить</button>
                            </form>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    )
}