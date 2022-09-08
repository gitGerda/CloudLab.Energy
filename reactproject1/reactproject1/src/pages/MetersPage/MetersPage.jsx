import React, { useMemo,useContext, useEffect, useState } from "react";
import style from "./MetersPage.module.css"
import "../../../node_modules/bootstrap/dist/css/bootstrap.min.css"
import CompanyMetersCard from "../../components/MetersPage_CompanyCard/CompanyMetersCard";
import Spinner1 from "../../components/UI/Spinner1/Spinner1";
import ErrorPage from "../ErrorPage/ErrorPage";
import Table1 from "../../components/UI/Tables/Table1/Table1";
import MeterInfo from "./MeterInfo/MeterInfo";
import ModalWindow from "../../components/UI/ModalWindow/ModalWindow";
import { Button, useToast } from "@chakra-ui/react";
import { AddIcon,SunIcon,LinkIcon } from "@chakra-ui/icons";
import { AuthContext } from "../../context/context";
import LogOut from "../../API/Auth/LogOut";
import CompanyInfo from "./CompanyInfo/CompanyInfo";


export default function MetersPage() {
    const { setIsAuthenticated } = useContext(AuthContext);
    const toast = useToast();
    const [isLoading, setIsLoading] = useState();
    const [CompanyCards, setCompanyCards] = useState();
    const [modalVisible, setModalVisible] = useState(false);
    const [meterID, setMeterId] = useState(false);
    const [needUpdate, setNeedUpdate] = useState(false);
    const [isError, setIsError] = useState(false);
    const [company_modal_visible, set_company_modal_visible] = useState(false);
    const [error, setError] = useState({
        isError: false,
        errorDesc: ''
    });
    useMemo(() => {
        if (error.isError == true) {
            toast({
                title: "Произошла ошибка",
                description: error.errorDesc,
                position: 'bottom',
                isClosable: true,
                status: 'error',
                duration: 3000
            });
            setError({
                isError: false,
                errorDesc: ''
            });
        };
    }, [error.isError]);
    useEffect(async () => {
        setIsLoading(true);
        if (needUpdate) {
            setNeedUpdate(false);
            setMeterId(false);
        }
        try {
            let token = sessionStorage.getItem('aspNetToken');
            var resp = await fetch("/api/meters/getAllMetersInfo", {
                headers: {
                    'Authorization': 'Bearer ' + token
                }
            });

            if (resp.ok) {
                var companiesINFO = await resp.json();
                var companyCardsLocal = [];
                companiesINFO.forEach(element => {
                    let metersCount = 0;
                    var tables = [];
                    var headerInfo = {
                        orgGetter: element.companyName,
                        orgGetterInn: element.companyInn,
                        orgSender: element.centerName,
                        orgSenderInn: element.centerInn,
                        contract: element.contractNumber,
                        msgNumber: element.xml80020MsgNumber,
                        msgNumberDate: element.xml80020MsgNumberDate
                    };

                    element.meters.forEach(value => {
                        var tableHead = <tr key={value.sim}>
                            <th key={0} scope="col">№</th>
                            <th key={1} scope="col">Тип</th>
                            <th key={2} scope="col">Адрес</th>
                            <th key={3} scope="col">Интерфейс</th>
                            <th key={4} scope="col">Дата последнего измерения</th>
                            <th key={5} scope="col">Время последнего измерения</th>
                            <th key={6} scope="col">#</th>
                        </tr>;
                        var tableDesc = "SIM: " + value.sim;
                        var tableBody = [];
                        let i = 1;
                        value.metersINFO.forEach(meter => {
                            // if (meter.lastIndicationTime != null) {
                            //     meter.lastIndicationTime = meter.lastIndicationTime.substring(11, 19);
                            // }
                            if (meter.lastIndicationDate != null) {
                                // meter.lastIndicationDate = meter.lastIndicationDate.substring(0, 10);
                                let _datetime = new Date(meter.lastIndicationDate);
                                meter.lastIndicationDate = _datetime.getDate() + "." + (_datetime.getMonth() + 1) + "." + _datetime.getFullYear();
                            }
                            tableBody.push(
                                <tr key={value.sim + i.toString()}>
                                    <th key={0 + i.toString()} scope="row">{i}</th>
                                    <td key={1 + i.toString()} >{meter.type}</td>
                                    <td key={2 + i.toString()} >{meter.address}</td>
                                    <td key={3 + i.toString()} >{meter.interface}</td>
                                    <td key={4 + i.toString()} >{meter.lastIndicationDate}</td>
                                    <td key={5 + i.toString()} >{meter.lastIndicationTime}</td>
                                    <td key={6 + i.toString()} >
                                        <button id='table_btn' type="button" className={"btn btn-success "+style.table_btn}
                                            onClick={() => {
                                                setMeterId(meter.id_meter);
                                                setModalVisible(true);
                                            }}>Открыть</button>
                                    </td>
                                </tr>
                            )
                            i++;
                            metersCount++;
                        })
                        tables.push(<Table1 key={value.sim} headTable={tableHead} bodyTable={tableBody} descTable={tableDesc}></Table1>)
                    });
                    companyCardsLocal.push(<CompanyMetersCard key={element.companyInn} headerInfo={headerInfo} tables={tables} metersCount={metersCount} />);
                });
                setCompanyCards(companyCardsLocal);
            }
            else {
                if (resp.status == 401) {
                    alert('Ошибка аутентификации. Будет выполнен выход...');
                    LogOut(setIsAuthenticated);
                }
                else {
                    setCompanyCards(<ErrorPage errorName={resp.status} errorDesc={"Не удалось получить данные с сервера"} />);
                    setIsError(true);
                }
            }
        }
        catch(Exception) {
            setCompanyCards(<ErrorPage errorName={"error"} errorDesc={Exception.message} />);
            setIsError(true);
        }

        setIsLoading(false);
    }, [needUpdate])

    function AddNewMeterButtonClick(e) {
        setMeterId(false);
        setModalVisible(true);
    }

    function _CompanyModalWindowOpen() {
        set_company_modal_visible(true);
    }

    return (
        <div className={style.metersPage}>
            <div className={style.control_btns_container}>
                <div className={style.control_btns}>
                    <Button rightIcon={<SunIcon/>}
                        colorScheme='yellow' variant='solid' borderRadius={20}
                        isFullWidth='true'
                        onClick={_CompanyModalWindowOpen}>Организации</Button>
                    {/* <Button rightIcon={<LinkIcon/>}
                        colorScheme='blue' variant='solid' borderRadius={20}
                        isFullWidth='true'
                        onClick={AddNewMeterButtonClick}>Интерфейсы связи</Button> */}
                    <Button rightIcon={<AddIcon />}
                        colorScheme='green' variant='solid' borderRadius={20}
                        isFullWidth='true'
                        onClick={AddNewMeterButtonClick}>Новый счётчик</Button>
                </div>
            </div>
            {isLoading ? <Spinner1 isLoading={isLoading}></Spinner1>
                : <div className={style.company_cards}>{CompanyCards}</div>
            }
            {/* company modal window */}
            <ModalWindow visible={company_modal_visible} setVisible={set_company_modal_visible} children={<CompanyInfo
                visible={company_modal_visible}
                change_visible={set_company_modal_visible}
                setError={setError}
                setIsAuthenticated={setIsAuthenticated}
            ></CompanyInfo>} custom_container_style={{ width: '50%', maxHeight: '70vh' }} />
            {/* communic interfaces modal window */}
            {/* meter modal window  */}
            <ModalWindow visible={modalVisible} setVisible={setModalVisible}
                children={<MeterInfo setVisible={setModalVisible} meterID={meterID} setNeedUpdate={setNeedUpdate} />} />
        </div>
    )
}