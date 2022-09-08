import React, { useContext, useEffect, useMemo, useState } from "react";
import style from "./ReportPage.module.css"
import "../../../node_modules/bootstrap/dist/css/bootstrap.min.css";
import ReportGenerateWindow from "./ReportPageComponents/ReportGenerateWindow/ReportGenerateWindow";
import HistoryComponent from "./ReportPageComponents/HistoryComponent/HistoryComponent";
import GraphicsComponent from "./ReportPageComponents/GraphicsComponent/GraphicsComponent";
import ReportAlert from "./ReportPageComponents/AlertComponent/ReportAlert";
import { useToast } from "@chakra-ui/react";
import { AuthContext } from "../../context/context";

export default function ReportPage() {

    const { setIsAuthenticated } = useContext(AuthContext);
    const [errorState, setErrorState] = useState({
        isError: false,
        errorDesc: ""
    });
    const [lnkToDownRep, setLnkToDownRep] = useState("");
    const [graphCompTable, setGraphCompTable] = useState([]);
    const [graphCompHeader, setGraphCompHeader] = useState({
        companyName: "-",
        reportDate: "-",
        periodDate: "-",
        reportPath:""
    });
    const [graphCompLoading, setGraphCompLoading] = useState(false);
    const [reportAlertVisible, setReportAlertVisible] = useState(false);
    const [historyNeedUpdateStateGlobal, setHistoryNeedUpdateStateGlobal] = useState(false);
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

    return (
        <div className={style.mainBox}>

            <ReportAlert isVisible={reportAlertVisible} linkToDownload={lnkToDownRep} set_visiible={setReportAlertVisible}/>

            <div className={style.newReportAndHistoryBlock}>
                <div className={style.newReportBlock}>
                    <ReportGenerateWindow
                        setHistoryNeedUpdateState={setHistoryNeedUpdateStateGlobal}
                        setLinkToDownRep={setLnkToDownRep}
                        setReportAlertVisible={setReportAlertVisible}
                        setGraphCompHeader = {setGraphCompHeader}
                        setGraphCompTable={setGraphCompTable}
                        setGraphCompLoading={setGraphCompLoading}
                        setErrorState={setErrorState}
                        setIsAuth={setIsAuthenticated} />
                </div>

                <div className={style.historyBlock}>
                    <HistoryComponent
                        UpdateGlobalState={historyNeedUpdateStateGlobal}
                        setGraphHeader={setGraphCompHeader}
                        setGraphTable={setGraphCompTable}
                        setGraphCompLoading ={setGraphCompLoading}
                        setUpdateGlobalState={setHistoryNeedUpdateStateGlobal}
                        setError={setErrorState}
                        setIsAuthenticated={setIsAuthenticated} />
                </div>
            </div>

            <div className={style.dataVisualizeBlock}>
                <GraphicsComponent 
                    infoBlockDescription={graphCompHeader}
                    infoBlockTable={graphCompTable}
                    isLoading={graphCompLoading}
                    reportLnk={lnkToDownRep}
                />
            </div>
            
        </div>
    )
}