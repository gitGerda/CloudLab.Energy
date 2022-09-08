import { useToast } from "@chakra-ui/react";
import { getValue } from "@testing-library/user-event/dist/utils";
import React, { useEffect, useState, useMemo } from "react";
import "../../../../../node_modules/bootstrap/dist/css/bootstrap.min.css";
import GetCompaniesNames from "../../../../API/GetCompaniesNames";
import CreateReport from "../../../../API/Report/CreateReport";
import style from "./ReportGenerateWindow.module.css"


export default function ReportGenerateWindow({ setReportAlertVisible, setHistoryNeedUpdateState,setLinkToDownRep,setGraphCompLoading,setGraphCompTable, setGraphCompHeader,setIsAuth }) {

    const [companies, setCompanies] = useState([]);
    const [energyYears, setEnergyYears] = useState([]);
    const [errorState, setErrorState] = useState({
        isError: false,
        errorDesc: ""
    });
    const toast = useToast();

    useMemo(() => {
        if (errorState.isError == true) {
            toast({
                title: "Произошла ошибка",
                description: errorState.errorDesc,
                position: 'bottom-right',
                isClosable: true,
                status: 'error',
                duration: 3000
            });
            setErrorState({
                isError: false,
                errorDesc: ""
            });
        }
    }, [errorState.isError]);

    function submitSpec(e) {
        e.preventDefault();
        var formToFetch = new FormData(e.target);

        var formToFetchJsonObj = {};
        formToFetch.forEach(function (value, key) {
            formToFetchJsonObj[key] = value;
        });
        var formToFetchJson = JSON.stringify(formToFetchJsonObj);

        let companyName = formToFetch.get("companyName");

        let reportDate = new Date(formToFetch.get("reportDate"));
        reportDate = reportDate.getDate() + "-" + (reportDate.getMonth()+1) + "-" + reportDate.getFullYear();

        let startDate = new Date(formToFetch.get("startDate"));
        startDate = startDate.getDate() + "-" + (startDate.getMonth() + 1) + "-" + startDate.getFullYear();

        let endDate = new Date(formToFetch.get("endDate"));
        endDate = endDate.getDate() + "-" + (endDate.getMonth()+1) + "-" + endDate.getFullYear();
        

        if (companyName == 'default' || reportDate == '' || startDate == '' || endDate == '') {
            setErrorState({
                isError: true,
                errorDesc: "Невозможно сформировать отчёт, так как обнаружены не заполненные поля"
            });
        }
        else {
            CreateReport({
                formToFetch: formToFetchJson,
                setResponse: createReportResponse,
                setError: setErrorState,
                setIsAuthenticated: setIsAuth
            });
            if (formToFetch.get('energyFlag') !== null) {
                setGraphCompHeader({
                    companyName: companyName,
                    reportDate: reportDate,
                    periodDate: startDate + " / " + endDate
                });
                setReportAlertVisible(false);
                setGraphCompLoading(true);
            } else {
                setGraphCompHeader({
                    companyName: "-",
                    reportDate: "-",
                    periodDate: "-"
                });
            };
        };
    }

    function createReportResponse(response) {
        try {
            var tableToGraph = [];
            response?.energyDetalization?.map((value) => {
                var rowToTableGraph = {
                    detalizationType: value.detalizationType,
                    id: value.id,
                    reportId: value.reportId,
                    tblColCheck: value.tblColCheck,
                    tblColDateOfRead: value.tblColDateOfRead,
                    tblColEnergySum: value.tblColEnergySum,
                    tblColMeterAddress: value.tblColMeterAddress,
                    tblColMeterType: value.tblColMeterType,
                    tblColPeriodEnd: value.tblColPeriodEnd,
                    tblColPeriodStart: value.tblColPeriodStart,
                    tblColPowerSum: value.tblColPowerSum,
                    tblColTransformRatio: value.tblColTransformRatio,
                };
                tableToGraph.push(rowToTableGraph);
            });
            setGraphCompTable(tableToGraph);
            setLinkToDownRep(response.reportPath);
            setGraphCompLoading(false);
            // setReportAlertVisible(true);
        }
        catch {
            setErrorState({
                isError: true,
                errorDesc:"Не удалось построить детализацию отчёта"
            });
        };
        setHistoryNeedUpdateState(true); 
    }

    function addEnergySwitch(e) {
        var switchText = document.getElementById("switchText");
        if (e.target.checked == true) {
            switchText.classList.remove(style.nonVisible);
        }
        else {
            switchText.classList.add(style.nonVisible);
        }
    }
    function setCompaniesSpec(companiesFromFetch) {
        var localCompanies = [];
        companiesFromFetch.map((value) => {
            localCompanies.push(value);
        });
        setCompanies(localCompanies);
    }

    useEffect(async () => {
        await GetCompaniesNames({
            setCompanies: setCompaniesSpec,
            setError: setErrorState,
            setIsAuthenticated: setIsAuth
        });

        let currentYear = new Date();
        currentYear = Number(currentYear.getFullYear());
        setEnergyYears([currentYear, currentYear - 1, currentYear - 2, currentYear - 3]);

    }, [])

    return (
        <div>
            <div className={style.headerOfWindow}>
                <img src="/images/generateReport.svg"></img>
                <span className={style.headerOfWindowText}>Новый отчёт</span>
            </div>

            <form onSubmit={submitSpec}>
                <div className="input-group mb-3">
                    <label className="input-group-text" htmlFor="inputGroupSelect01">Организация:</label>
                    <select className="form-select" id="inputGroupSelect01" name="companyName">
                        <option value="default">Выбрать</option>
                        {
                            companies.map(value =>
                                <option key={value?.key} value={value?.name}>{value?.name}</option>
                            )
                        }
                    </select>
                </div>

                <div className="input-group mb-3">
                    <span className="input-group-text" id="basic-addon1">Дата формирования отчёта:</span>
                    <input type="datetime-local" className="form-control" name="reportDate"/>
                </div>

                <div className="input-group mb-3">
                    <span className="input-group-text" id="basic-addon1">Начало периода:</span>
                    <input type="date" className="form-control" name="startDate"/>
                </div>

                <div className="input-group mb-3">
                    <span className="input-group-text" id="basic-addon1">Конец периода:</span>
                    <input type="date" className="form-control" name="endDate"/>
                </div>

                <div className="form-check form-switch">
                    <input className="form-check-input" type="checkbox" id="flexSwitchCheckChecked"
                        defaultChecked onChange={addEnergySwitch} name="energyFlag"/>
                    <label className="form-check-label" htmlFor="flexSwitchCheckChecked">Добавить отчёт о потребленной энергии (кВт*ч)</label>
                    <div id="switchText" className={style.txt}>Примечание: отчёт о потреблённой энергии расчитывается за месяц на начало периода</div>
                </div>
                <div className="d-grid gap-2">
                    <button className={"btn btn-primary " + style.generateReportBtn} type="submit">Сформировать отчёт</button>
                </div>
            </form>
        </div>
    );
}