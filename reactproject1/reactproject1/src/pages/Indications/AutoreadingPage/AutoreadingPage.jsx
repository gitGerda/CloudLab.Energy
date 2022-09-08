import { Spinner, useToast } from "@chakra-ui/react";
import { toHaveFormValues } from "@testing-library/jest-dom/dist/matchers";
import React, { useState,useEffect, useContext, useMemo } from "react";
import GetCommunicPoints from "../../../API/IndicationsReading/GetCommunicPoints";
import GetShedulesInfo from "../../../API/IndicationsReading/GetShedulesInfo";
import { AuthContext } from "../../../context/context";
import style from "./AutoreadingPage.module.css";
import CommunicPoint from "./PageComponents/CommunicPoints/CommunicPoints";
import ShedulesComponent from "./PageComponents/Shedules/ShedulesComponent";
import ShedulesLogs from "./PageComponents/ShedulesLogs/ShedulesLogs";

export default function AutoreadingPage() {

    const { setIsAuthenticated } = useContext(AuthContext);
    const [error, setError] = useState({
        isError: false,
        errorDesc: ''
    });
    const toast = useToast();
    const [shedulesIsLoading, setShedulesIsLoading] = useState(true);  
    const [shedulesNeedUpdate, setShedulesNeedUpdate] = useState(false);
    const [sheduleFormsMs, setSheduleFormMs] = useState([]);
    const [communicPointsIsLoading, setCommunicPointsIsLoading] = useState(true);
    const [communicPointsMs, setCommunicPointsMs] = useState([]);
    const [communicPointsNeedUpdate, setCommunicPointsNeedUpdate] = useState(false);
    const [selectedSheduleToLogs, setSelectedSheduleToLogs] = useState(undefined);

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
    useMemo(async () => {
        if (shedulesNeedUpdate == true) {
            setShedulesIsLoading(true);
            await GetShedulesInfo({
                setShedules: _setShedulesFormCustom,
                setError: setError,
                setIsAuthenticated: setIsAuthenticated
            });
        }
    },[shedulesNeedUpdate])
    useMemo(async () => {
        if (communicPointsNeedUpdate == true) {
            setCommunicPointsIsLoading(true);
            await GetCommunicPoints({
                setCommunicPoints: _setCommunicPointsMsCustom,
                setError: setError,
                setIsAuthenticated: setIsAuthenticated
            });
        }
    },[communicPointsNeedUpdate])

    useEffect(() => {
        //getting shedules 
        GetShedulesInfo({
            setShedules: _setShedulesFormCustom,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
        //getting communicPoints
        GetCommunicPoints({
            setCommunicPoints: _setCommunicPointsMsCustom,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
    }, [])

    function _setCommunicPointsMsCustom(communic_points) {
        setCommunicPointsMs(communic_points);
        setCommunicPointsIsLoading(false);
        setCommunicPointsNeedUpdate(false);
    }

    function _setShedulesFormCustom(shedules) {
        let shedulesMs = [];
        shedules.map((value) => {
            shedulesMs.push(
                {
                    shedule_id:value.shedule_id,
                    sheduleName: value.name,
                    status: Number(value.status),
                    creatingDate: value.creating_date,
                    shedule: value.shedule,
                    communicPoint: value.communicPointName,
                    countRemModems: value.countRemoteModems,
                    countRemMeters: value.countRemoteMeters,
                    lastReadingValue: value.lastReadingDate ?? "No data available"
                }
            )
        });
        setSheduleFormMs(shedulesMs);
        setShedulesIsLoading(false);
        setShedulesNeedUpdate(false);
    }

    return (
        <div className={style.autoreadingMain}>

            <div className={style.shedulesBlock}>
                {shedulesIsLoading ? <div className={style.spinnerContainer}><Spinner className={style.spinner}></Spinner></div>
                    : <ShedulesComponent sheduleForms={sheduleFormsMs}
                        setShedulesNeedUpdate={setShedulesNeedUpdate}
                        setSheduleForms={_setShedulesFormCustom}
                        setError={setError}
                        setIsAuthenticated={setIsAuthenticated}
                        setShedulesIsLoading={setShedulesIsLoading}
                        setSelectedShedule={setSelectedSheduleToLogs}
                    />}                  
            </div>

            <div className={style.commPointsAndLogsBlock}>
                
                <div className={style.communicPointsBlock}>
                    {communicPointsIsLoading ? <div className={style.spinnerContainer}><Spinner className={style.spinner}></Spinner></div>
                        : <CommunicPoint
                            communicPointsMs={communicPointsMs}
                            setCommunicPointsNeedUpdate={setCommunicPointsNeedUpdate}
                            setError={setError}
                            setIsAuthenticated={setIsAuthenticated}
                        />}  
                </div>

                <div className={style.shedulesLogsBlock}>
                    <ShedulesLogs
                        selectedSheduleToLogs={selectedSheduleToLogs}
                        setSelectedSheduleToLogs={setSelectedSheduleToLogs}
                        setError={setError}
                        setIsAuthenticated={setIsAuthenticated}
                    />
                </div>
            </div>

        </div>
    )
};