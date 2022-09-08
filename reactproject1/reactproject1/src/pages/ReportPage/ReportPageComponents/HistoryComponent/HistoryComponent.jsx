import { Spinner } from "@chakra-ui/react";
import React, {useEffect, useMemo, useState } from "react";
import "../../../../../node_modules/bootstrap/dist/css/bootstrap.min.css";
import GetReportDetalization from "../../../../API/Report/GetReportDetalization";
import GetReportsHistory from "../../../../API/Report/GetReportsHistory";
import Spinner1 from "../../../../components/UI/Spinner1/Spinner1";
import style from "./HistoryComponent.module.css"
import HistoryListElement from "./HistoryListElement/HistoryListElement";

export default function HistoryComponent({UpdateGlobalState,setUpdateGlobalState,setGraphCompLoading,setGraphHeader,setGraphTable, setError, setIsAuthenticated}) {

    const [historyState, setHistoryState] = useState([]);
    const [historyNeedUpdateState, setHistoryNeedUpdateState] = useState(true);
    const [skipDatabaseRecords, setSkipDatabaseRecords] = useState(0);
    const [takeDatabaseRecords, setTakeDatabaseRecords] = useState(5);
    const [databaseRecordsEnd, setDatabaseRecordsEnd] = useState(false);

    useEffect(() => {
        GetHistory();
    },[]);

    useEffect(() => {
        if (UpdateGlobalState === true) {
            setHistoryState([]);
            setSkipDatabaseRecords(0);
            setHistoryNeedUpdateState(true);
            setUpdateGlobalState(false);
        }
    }, [UpdateGlobalState]);
    
    useMemo(() => {
        if (historyNeedUpdateState === true) {
            GetHistory();
        };
    }, [historyNeedUpdateState])

    function GetHistory() {
        if (databaseRecordsEnd === false) {
            GetReportsHistory({
                skipRecords: skipDatabaseRecords,
                takeRecords: takeDatabaseRecords,
                setResponse: setHistoryStateSpec,
                setError: setError,
                setIsAuthenticated: setIsAuthenticated
            });
        }
    };

    function setHistoryStateSpec(response) {
        if (response.length > 0) {
            try {
                let historyStateAdd = [...historyState];
                response.map((value) => {
                    var historyElementToAdd = {};

                    historyElementToAdd.reportId = value.historyElement.id;
                    historyElementToAdd.companyName = value.historyElement.companyName;

                    let reportDateL = new Date(value.historyElement.reportsDate);
                    historyElementToAdd.reportDate = reportDateL.getDate() + "." + (reportDateL.getMonth() + 1) + '.' + reportDateL.getFullYear();
                    historyElementToAdd.reportDateL = reportDateL;

                    let periodEnd = new Date(value.historyElement.periodEndDate);
                    periodEnd = periodEnd.getDate() + "." + (periodEnd.getMonth()+1) + "." + periodEnd.getFullYear();
                    
                    let periodStart = new Date(value.historyElement.periodStartDate);
                    periodStart = periodStart.getDate() + "." + (periodStart.getMonth() + 1) + "." + periodStart.getFullYear();
                    
                    historyElementToAdd.reportPeriod = periodStart + " - " + periodEnd;

                    let reportDate = Date.parse(value.historyElement.reportsDate);
                    let currentDate = Date.now();
                    // historyElementToAdd.companyNameSmall = Math.floor((currentDate - reportDate) / 86400000);
                    historyElementToAdd.companyNameSmall = value?.historyElement?.recordCreateDate ?? "";
                    var _createDate = new Date(historyElementToAdd.companyNameSmall);
                    historyElementToAdd.companyNameSmall =_createDate.getHours()+":"+_createDate.getMinutes()+":"+_createDate.getSeconds()+" / "+ _createDate.getDate() + "." + (_createDate.getMonth() + 1) + "." + _createDate.getFullYear();

                    historyElementToAdd.detalizationFlag = value.detalizationFlag;

                    historyElementToAdd.reportPath = value.historyElement?.reportPath??"";

                    historyStateAdd.push(historyElementToAdd);
                });

                // historyStateAdd.sort((a, b) => {
                //     return b.reportDateL - a.reportDateL;
                // })
                
                setHistoryState(historyStateAdd);
                setHistoryNeedUpdateState(false);
                setSkipDatabaseRecords(skipDatabaseRecords + takeDatabaseRecords);
            }
            catch (Exception) {
                setError({
                    isError: true,
                    errorDesc: Exception.message
                });
            }
        }
        else {
            setDatabaseRecordsEnd(true);
        }
    }

    function historyElementClick(e) {
        console.log(e);
        e.target.parentElement.parentElement.childNodes.forEach(element => {
            element.firstChild.classList.remove('active');
            element.firstChild.style.backgroundColor = '';
            element.firstChild.style.borderColor = '';
        });
        e.target.classList.add('active');
        e.target.style.backgroundColor = '#222533';
        e.target.style.borderColor = '#222533';

        let reportId = e.target.firstChild.innerText;
        
        let companyName = e.target?.children[1]?.children[0].innerText ??"";
        let reportDate = e.target?.children[2].innerText.replace("Дата отчёта: ", "")??"";
        let period = e.target?.children[4].innerText.replace("Период отчёта: ", "")??"";
        let reportPath = e.target?.children[5].innerText??"";

        GetReportDetalization({
            reportId: reportId,
            setResponse: ReportDetalizationSpec,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
        setGraphHeader({
            companyName: companyName,
            reportDate: reportDate,
            periodDate: period,
            reportPath: reportPath
        });
        setGraphCompLoading(true);
    }

    function ReportDetalizationSpec(response) {
        try {
            var tableToGraph = [];
            if (response.length > 0) {
                response.map((value) => {
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
            };
            setGraphTable(tableToGraph);
        }
        catch (Exception) {
            setError({
                isError: true,
                errorDesc: Exception.message
            });
        };
        setGraphCompLoading(false);
    }

    function onScrollSpec(e) {
        var historyList = document.getElementById(style.historyList);
        if (Math.ceil(historyList.offsetHeight + historyList.scrollTop) >= historyList.scrollHeight-10) {
            console.log('Scroll');
            if (!databaseRecordsEnd) {
                setHistoryNeedUpdateState(true);
            }
        }
    }

    return (
        <div>
            <div className={style.headerOfWindow}>
                <img src="/images/history.svg"></img>
                <span className={style.headerOfWindowText}>История</span>
            </div>
 
            <div id={style.historyList} className={"list-group " + style.linksList} onScroll={onScrollSpec}>
                
                {historyState.length > 0 ? historyState.map((value) => {
                    return <HistoryListElement
                        key={value.reportId}
                        detalizVisible={value.detalizationFlag}
                        id={value.reportId}
                        header={value.companyName}
                        headerSmall={value.companyNameSmall}
                        body={"Дата отчёта: " + value.reportDate}
                        bodySmall={"Период отчёта: " + value.reportPeriod}
                        onClickSpec={historyElementClick}
                        report_path={value.reportPath}
                    ></HistoryListElement>
                })
                    :
                    <div></div>
                }

            </div>
        </div>
    );
}