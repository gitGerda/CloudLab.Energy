import React, { useEffect, useMemo, useState,useContext } from "react";
import style from "./MeterInfo.module.css"
import "../../../../node_modules/bootstrap/dist/css/bootstrap.min.css"
import SelectField from "../../../components/UI/SelectField/SelectField";
import CloseButton from "../../../components/UI/Buttons/CloseButton/CloseButton";
import Spinner1 from "../../../components/UI/Spinner1/Spinner1";
import MeterInfoById from "../../../API/MeterInfoByID";
import GetCompaniesNames from "../../../API/GetCompaniesNames";
import GetCompaniesSenders from "../../../API/GetCompaniesSenders";
import GetMeterTypes from "../../../API/GetMeterTypes";
import GetMeterInterfaces from "../../../API/GetMeterInterfaces";
import PutChangeMeterData from "../../../API/PutChangeMeterData";
import CreateMeter from "../../../API/MetersPage/CreateMeter";
import { useToast } from "@chakra-ui/react";
import DeleteMeter from "../../../API/DeleteMeter";
import { AuthContext } from "../../../context/context";

export default function MeterInfo({ meterID, setVisible, setNeedUpdate }) {
    const { setIsAuthenticated } = useContext(AuthContext);
    const toast = useToast();
    const [meterObjState, setMeterObjState] = useState({
        idMeter: "",
        type: "",
        address: "",
        sim: "",
        inn: "",
        companyName: "",
        inncenter: "",
        centerName: "",
        measuringpointName: "",
        measuringchannelA: "",
        measuringchannelR: "",
        xml80020code: "",
        transformationRatio: "",
        interface: ""

    });
    const [isLoading, setIsLoading] = useState(false);
    const [errorState, setErrorState] = useState({
        isError: false,
        errorDesc: ""
    });
    const [successState, setSuccessState] = useState({
        isSuccess: false,
        successDesc:""
    });
    const [header, setHeader] = useState("Добавление нового счётчика");
    const [companies, setCompanies] = useState([{
        value: "",
        desc: "",
        key:'',
        selected: false
    }]);
    const [selectedCompany, setSelectedCompany] = useState("");
    const [_selected_company_inn, _set_selected_company_inn] = useState('');
    const [companiesSenders, setCompaniesSenders] = useState([{
        value: "",
        desc: "",
        selected: false
    }])
    const [selectedCompaniesSenders, setSelectedCompaniesSenders] = useState("")
    const [meterTypes, setMeterTypes] = useState([]);
    const [selectedMeterTypes, setSelectedMeterTypes] = useState("");
    const [meterInterfaces, setMeterInterfaces] = useState([]);
    const [selectedMeterInterfaces, setSelectedMeterInterfaces] = useState("");

    function setCompaniesSpec(companiesFromFetch) {
        try {
            let tempMS = [];
            companiesFromFetch.forEach(element => {
                let flag = false;
                if (element?.name == meterObjState.companyName) {
                    flag = true;
                    setSelectedCompany(element?.name);
                }
                tempMS.push({
                    value: element?.name,
                    desc: element?.name,
                    key:element?.inn,
                    selected: flag
                });
            });
            setCompanies(tempMS);
        }
        catch {
            setErrorState({ isError: true, errorDesc: "" });
        }
    }
    function setCompaniesSendersSpec(companiesFromFetch) {
        try {
            let tempMS = [];
            companiesFromFetch.forEach(element => {
                let flag = false;
                if (element == meterObjState.centerName) {
                    flag = true;
                    setSelectedCompaniesSenders(element);
                }
                tempMS.push({
                    value: element,
                    desc: element,
                    selected: flag
                });
            });
            setCompaniesSenders(tempMS);
        }
        catch {
            setErrorState({ isError: true, errorDesc: "" });
        }
    }
    function setMeterTypeSpec(meterTypes) {
        try {
            let tempMS = [];
            meterTypes.forEach(element => {
                let flag = false;
                if (element == meterObjState.type) {
                    flag = true;
                    setSelectedMeterTypes(element);
                }
                tempMS.push({
                    value: element,
                    desc: element,
                    selected: flag
                });
            });
            setMeterTypes(tempMS);
        }
        catch {
            setErrorState({ isError: true, errorDesc: "" });
        }
    }
    function setMeterInterfacesSpec(meterInterfaces) {
        try {
            let tempMS = [];
            meterInterfaces.forEach(element => {
                let flag = false;
                if (element == meterObjState.interface) {
                    flag = true;
                    setSelectedMeterInterfaces(element);
                }
                tempMS.push({
                    value: element,
                    desc: element,
                    selected: flag
                });
            });
            setMeterInterfaces(tempMS);
        }
        catch {
            setErrorState({ isError: true, errorDesc: "" });
        }
    }

    useEffect(async () => {
        setIsLoading(true);
        setErrorState({ isError: false, errorDesc:"" });
        setSuccessState({
            isSuccess: false,
            successDesc: ""
        });

        if (meterID != false) {
            await MeterInfoById({ meterID: meterID, setMeterInfo: setMeterObjState, setError: setErrorState, setIsAuthenticated: setIsAuthenticated });
        }
        else {
            setMeterObjState({
                idMeter: "",
                type: "",
                address: "",
                sim: "",
                inn: "",
                companyName: "",
                inncenter: "",
                centerName: "",
                measuringpointName: "",
                measuringchannelA: "",
                measuringchannelR: "",
                xml80020code: "",
                transformationRatio: "",
                interface: ""
            });
        }
    }, [meterID]);

    useMemo(async () => {
        setIsLoading(true);
        if (meterID != false) {
            setHeader(meterObjState.type + " (Адрес: " + meterObjState.address + ")");
        }
        else {
            setHeader('Новый счётчик');
        }
        await GetCompaniesNames({ setCompanies: setCompaniesSpec, setError: setErrorState, setIsAuthenticated: setIsAuthenticated });
        await GetCompaniesSenders({ setCompaniesSenders: setCompaniesSendersSpec, setError: setErrorState, setIsAuthenticated: setIsAuthenticated });
        await GetMeterTypes({ setMetertypes: setMeterTypeSpec, setError: setErrorState, setIsAuthenticated: setIsAuthenticated });
        await GetMeterInterfaces({ setMeterInterfaces: setMeterInterfacesSpec, setError: setErrorState, setIsAuthenticated: setIsAuthenticated });
        setIsLoading(false);

    }, [meterObjState])

    useMemo(() => {
        if (errorState.isError==true) {
            toast({
                title: 'Ошибка!',
                position: 'bottom-right',
                description: "Возникли ошибки: " + errorState.errorDesc,
                status: 'error',
                duration: 3000,
                isClosable: true,
            });
        };
    }, [errorState.isError]);

    useMemo(() => {
        if (selectedCompany == 'По умолчанию') {
            _set_selected_company_inn('');
        }
        else {
            companies.map((el) => {
                if (el.value == selectedCompany) {
                    _set_selected_company_inn(el.key);
                    return;
                }
            })
        }
    },[selectedCompany])

    useMemo(() => {
        if (successState.isSuccess==true) {
            toast({
                title: 'Выполнено!',
                position: 'bottom-right',
                description: successState.successDesc,
                status: 'success',
                duration: 3000,
                isClosable: true,
            });
        };
    }, [successState.isSuccess])

    async function sendData(e) {
        e.preventDefault();
        setIsLoading(true);
        var form = e.target.parentElement.parentElement;
        var formData = new FormData(form);
        formData.append("CompanyName", selectedCompany);
        formData.append("CenterName", selectedCompaniesSenders);
        formData.append("Type", selectedMeterTypes);
        formData.append("Interface", selectedMeterInterfaces);
        if (meterID != false) {
            await PutChangeMeterData({ formData: formData, setError: setErrorState, setSuccess: setSuccessState, setIsAuthenticated: setIsAuthenticated });
        }
        else {
            formData.set("idMeter", 0);
            formData.set("Address", Number(formData.get("Address")));
            formData.forEach((value, key) => {
                console.log(`[${key}] - ${value}`);
            });
            var _result = await CreateMeter({
                meter_form_data: formData,
                setError: setErrorState,
                setIsAuthenticated: setIsAuthenticated
            });
            if (_result == true) {
                toast({
                    title: 'Успех',
                    position: 'bottom-right',
                    description: 'Счётчик успешно создан',
                    status: 'success',
                    duration: 3000,
                    isClosable: true,
                });
            }
        }
        setIsLoading(false);
        setVisible(false);
        setNeedUpdate(true);
    }
    async function deleteMeter(e) {
        setIsLoading(true);

        if (meterID != false) {
            await DeleteMeter({ meterID: meterID, setError: setErrorState, setSuccess: setSuccessState, setIsAuthenticated: setIsAuthenticated });
        }

        setIsLoading(false);
        setVisible(false);
        setNeedUpdate(true);
    }

    return (
        <div>
            {isLoading ? <Spinner1 isLoading={isLoading} color="#6639A6" />
                :
                <div id={style.mainCard} className="card">
                    <div id={style.header} className="card-header">
                        <div id={style.desc}>
                            {header}
                            {errorState.isError ? <span id={style.errorMsg}> (При загрузке данных произошла ошибка)</span>
                                : ""}
                        </div>
                        <CloseButton close={() => setVisible(false)} />
                    </div>
                    <form method="put" action="api/meters/changeMeterData">
                        <div id={style.body} className="card-body">

                            <input id={style.nonVisisbleInput} defaultValue={meterObjState.idMeter} name="idMeter"></input>
                            <div id={style.input} className="input-group">
                                <span className="input-group-text">Организация-потребитель</span>
                                <SelectField add_new_flag={false} options={companies} setSelected={setSelectedCompany} selected={selectedCompany} />
                                <span className="input-group-text">ИНН</span>
                                <input readOnly type="text" className="form-control"
                                    value={_selected_company_inn}
                                    name="Inn" />
                            </div>
                            <div id={style.input} className="input-group">
                                <span className="input-group-text">Организация-отправитель</span>
                                <SelectField options={companiesSenders} setSelected={setSelectedCompaniesSenders} selected={selectedCompaniesSenders} />
                                <span className="input-group-text">ИНН</span>
                                <input type="text" className="form-control" defaultValue={meterObjState.inncenter} name="Inncenter" />
                            </div>
                            <div id={style.input} className="input-group">
                                <span className="input-group-text">Тип</span>
                                <SelectField options={meterTypes} setSelected={setSelectedMeterTypes} selected={selectedMeterTypes} />
                                <span className="input-group-text">Адрес</span>
                                <input type="text" className="form-control" defaultValue={meterObjState.address} name="Address" />
                            </div>
                            <div id={style.input} className="input-group">
                                <span className="input-group-text">Интерфейс</span>
                                <SelectField options={meterInterfaces} setSelected={setSelectedMeterInterfaces} selected={selectedMeterInterfaces} />
                                <span className="input-group-text">SIM</span>
                                <input type="text" className="form-control" defaultValue={meterObjState.sim} name="Sim" />
                            </div>
                            <div id={style.input} className="input-group flex-nowrap">
                                <span className="input-group-text" id="addon-wrapping">Коэффициент трансформации</span>
                                <input type="text" className="form-control" aria-describedby="addon-wrapping"
                                    defaultValue={meterObjState.transformationRatio} name="TransformationRatio" />
                            </div>
                            <div id={style.input} className="input-group flex-nowrap">
                                <span className="input-group-text" id="addon-wrapping">MeasuringPointName</span>
                                <input type="text" className="form-control" aria-describedby="addon-wrapping"
                                    defaultValue={meterObjState.measuringpointName} name="MeasuringpointName" />
                            </div>
                            <div id={style.input} className="input-group flex-nowrap">
                                <span className="input-group-text" id="addon-wrapping">MeasuringChannelA</span>
                                <input type="text" className="form-control" aria-describedby="addon-wrapping"
                                    defaultValue={meterObjState.measuringchannelA} name="MeasuringchannelA" />
                            </div>
                            <div id={style.input} className="input-group flex-nowrap">
                                <span className="input-group-text" id="addon-wrapping">MeasuringChannelR</span>
                                <input type="text" className="form-control" aria-describedby="addon-wrapping"
                                    defaultValue={meterObjState.measuringchannelR} name="MeasuringchannelR"
                                />
                            </div>
                            <div id={style.input} className="input-group flex-nowrap">
                                <span className="input-group-text" id="addon-wrapping">XML80020code</span>
                                <input type="text" className="form-control" aria-describedby="addon-wrapping"
                                    defaultValue={meterObjState.xml80020code} name="Xml80020code"
                                />
                            </div>

                        </div>
                        <div id={style.footer} className="card-footer text-muted">
                            <button id={style.button} type="submit" className="btn btn-success" onClick={sendData}>Сохранить</button>
                            <button id={style.button} type="button" className="btn btn-danger" onClick={deleteMeter}>Удалить</button>
                        </div>
                    </form>
                </div>}
        </div>
    )
}