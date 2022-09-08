import { Input, Stack, useToast } from "@chakra-ui/react";
import React, { useEffect, useMemo, useState } from "react";
import AddOrUpdateCompany from "../../../API/MetersPage/AddOrUpdateCompany_API";
import DeleteCompany from "../../../API/MetersPage/DeleteCompany";
import GetFullCompaniesInfo from "../../../API/MetersPage/GetFullCompaniesInfo";
import style from './CompanyInfo.module.css'

export default function CompanyInfo({ visible, change_visible, setError, setIsAuthenticated }) {
    const toast = useToast();
    const [_company_name, _set_company_name] = useState('');
    const [_company_inn, _set_company_inn] = useState('');
    const [_company_contract, _set_company_contract] = useState('');
    const [_company_report_number, _set_company_report_number] = useState('');
    const[_company_report_number_date, _set_company_report_number_date] = useState('');
    const [_full_companies_info, _set_full_companies_info] = useState([]);
    const [_selected_company_inn, _set_selected_company_inn] = useState('');
    const[_new_company_flag, _set_new_company_flag] = useState(false);

    useMemo(() => {
        if (visible == false) {
            _put_to_zero();
        }
    },[visible])

    useEffect(() => {
        GetFullCompaniesInfo({
            setCompaniesInfo: _set_full_companies_info,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        }); 
    },[]);

    function _put_to_zero() {
        _set_selected_company_inn('');
        _set_company_name('');
        _set_company_inn('');
        _set_company_contract('');
        _set_company_report_number('');
        _set_company_report_number_date('');
        _set_selected_company_inn('');
    }
    function _company_click(e) {
        _full_companies_info.map((value) => {
            if (value?.companyInn == e.target.id) {
                _set_selected_company_inn(e.target.id);
                _set_company_name(value?.companyName);
                _set_company_inn(value?.companyInn);
                _set_company_contract(value?.contract);
                _set_company_report_number(value?.msgNumber1);
                _set_company_report_number_date(value?.date);

                if (value?.companyInn == '')
                    _set_new_company_flag(true);
                else
                    _set_new_company_flag(false);
            };
        });
    }
    async function _save_and_close_on_click(e) {
        if (_company_inn == '' || _company_name=='' || _company_report_number=='' || _company_report_number_date==''|| _company_contract=='') {
            toast({
                title: "Предупреждение",
                description: "Обнаружены не заполненные поля",
                position: 'bottom',
                isClosable: true,
                status: 'warning',
                duration: 3000
            });
        }
        else {
            let _result = await AddOrUpdateCompany({
                company_name: _company_name,
                company_inn: _company_inn,
                contract_number: _company_contract,
                first_report_number: _company_report_number,
                first_report_number_date: _company_report_number_date,
                setError: setError,
                setIsAuthenticated: setIsAuthenticated
            });
            if (_result == true) {
                if (_new_company_flag == true) {
                    toast({
                        title: "Успех",
                        description: "Новая организация успешно добавлена",
                        position: 'bottom',
                        isClosable: true,
                        status: 'success',
                        duration: 3000
                    });
                }
                else {
                    toast({
                        title: "Успех",
                        description: "Данные об организации успешно обновлены",
                        position: 'bottom',
                        isClosable: true,
                        status: 'success',
                        duration: 3000
                    });
                }
                GetFullCompaniesInfo({
                    setCompaniesInfo: _set_full_companies_info,
                    setError: setError,
                    setIsAuthenticated: setIsAuthenticated
                });
            };
            _close_window();
        }
    }
    function _add_new_company(e) {
        let _company = {
            id: '',
            companyName: '@ Новая организация',
            companyInn: '',
            contract: '',
            msgNumber1: '',
            date: ''
        };
        _set_full_companies_info([..._full_companies_info, _company]);
    }
    async function _remove_company() {
        let _result = await DeleteCompany({
            company_inn: _selected_company_inn,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
        if (_result == true) {
            var _filtred_full_companies = _full_companies_info.filter((value) => {
                return value?.companyInn != _selected_company_inn;
            });
            _set_full_companies_info(_filtred_full_companies);
            _put_to_zero();

            toast({
                title: "Успех",
                description: "Организация успешно удалена",
                position: 'bottom',
                isClosable: true,
                status: 'success',
                duration: 3000
            });
        }
        else {
            toast({
                title: "Предупреждение",
                description: "Вы не можете удалить данную организацию, так как с ней сопоставлены счётчики",
                position: 'bottom',
                isClosable: true,
                status: 'warning',
                duration: 3000
            });

        }
    }

    function _close_window() {
        change_visible(false);
    }
    function _on_name_change(e) {
        _on_change(_company_name, _set_company_name, e);
    }
    function _on_inn_change(e) {
        if (_new_company_flag==false) {
            toast({
                title: "Предупреждение",
                description: 'Вы не можете изменить поле ИНН',
                position: 'bottom',
                isClosable: true,
                status: 'warning',
                duration: 3000
            });
        }
        else {
            _on_change(_company_inn, _set_company_inn, e);
        }
    }
    function _on_contract_change(e) {
        _on_change(_company_contract, _set_company_contract, e);
    }
    function _on_report_number_change(e) {
        _on_change(_company_report_number, _set_company_report_number, e);
    }
    function _on_report_date_change(e) {
        _on_change(_company_report_number_date, _set_company_report_number_date, e);
    }
    function _on_change(value,change_func, event) {
        switch (event.nativeEvent.inputType) {
            case 'deleteContentBackward': {
                let _value = value.substr(0, value.length - 1);
                change_func(_value);
                break;
            }
            case 'insertText': {
                let _value = value + event.nativeEvent.data;
                change_func(_value);
                break;
            }
        }
    }
    
    return (
        <div className={style.company_info_container}>
            {/* HEADER  */}
            <div className={style.header}>
                <div className={style.header_name}>
                    <img src='/images/company_modal/house.svg'></img>
                    <span>Организации</span>
                </div>
                <div className={style.header_close}>
                    <button title="Закрыть" onClick={_close_window}>
                        <img src='/images/sheduleBlankImg/close.svg'></img>
                    </button>
                </div>
            </div>
            {/* BODY */}
            <div className={style.body}>
                <div className={style.sidebar}>
                    <button className={style.add_new_company_btn} onClick={_add_new_company}>
                        <img src='/images/company_modal/plus.svg'></img>
                        <span>Новая организация</span>
                    </button>

                    <div className={style.companies}>
                        {
                            _full_companies_info.map((value) => {
                                return <button key={value?.companyInn}
                                   onClick={_company_click}>
                                    {_selected_company_inn == value?.companyInn ?
                                        <img src='/images/company_modal/arrow.svg'></img> : <span></span>}
                                    <span id={value?.companyInn}>{value?.companyName}</span>
                                </button>
                            })
                        }
                    </div>

                </div>
                <div className={style.control_part}>
                    <div>
                        <div>
                            <label htmlFor='name'>1. Наименование организации</label>
                            <Input id='name' name='name' variant='outline' type='text' placeholder="Введите наименование организации" value={_company_name} onChange={_on_name_change}></Input>

                            <label htmlFor='inn'>2. ИНН</label>
                            <Input id='inn' name='inn' variant='outline' type='text' placeholder="Введите ИНН" value={_company_inn} onChange={_on_inn_change}></Input>

                            <label htmlFor='contract'>3. Номер договора</label>
                            <Input id='contract' name='contract' variant='outline' type='text' placeholder="Введите номер договора" value={_company_contract} onChange={_on_contract_change}></Input>

                            <label htmlFor='report_number'>4. Первоначальный номер отчёта</label>
                            <Input id='report_number' name='report_number'  variant='outline' type='text' placeholder="Введите первональный номер отчёта" value={_company_report_number}onChange={_on_report_number_change}></Input>

                            <label htmlFor='report_number_date'>5. Дата первоначального отчёта</label>
                            <Input id='report_number_date' name='report_number_date' variant='outline' type='text' placeholder="Введите дату первонального отчёта" value={_company_report_number_date} onChange={_on_report_date_change}></Input>
                        </div>
                        <div className={style.control_part_btns}>
                            <button onClick={_save_and_close_on_click}>
                                <img src='/images/company_modal/okey.svg'></img>
                                <span>Сохранить</span>
                            </button>
                            <button onClick={_remove_company}>
                                <span>Удалить</span>
                            </button>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    )
}