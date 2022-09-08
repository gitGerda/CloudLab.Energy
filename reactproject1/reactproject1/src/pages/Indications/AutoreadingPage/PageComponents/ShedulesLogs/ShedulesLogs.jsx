import React, { useEffect, useMemo, useState } from "react";
import style from './ShedulesLogs.module.css'
// import style from '../CommunicPoints/CommunicPoint.module.css'
import EllipseBtnWithRightImg from "../../../../../components/UI/Buttons/EllipseBtnWithRightImg/EllipseBtnWithRightImg";
import {
    Table,
    Thead,
    Tbody,
    Tfoot,
    Tr,
    Th,
    Td,
    TableCaption,
    TableContainer,
} from '@chakra-ui/react'
import GetShedulesLogs from "../../../../../API/ShedulesLogs/GetShedulesLogs";
import GetShedulesLogsById from "../../../../../API/ShedulesLogs/GetSheduleLogsById";
import { responseInterceptor } from "http-proxy-middleware";
import ResetAllLogs from "../../../../../API/ShedulesLogs/ResetAllLogs";

export default function ShedulesLogs({selectedSheduleToLogs,setSelectedSheduleToLogs, setError, setIsAuthenticated}) {

    const INFO_STATUS = "info";
    const OK_STATUS = "success";
    const WARNING_STATUS = "warning";
    const ERROR_STATUS = "error";
    const okeyImgSrc = '/images/okey.svg';
    const errorImgSrc = '/images/errorStatus.svg';
    const warningImgSrc = '/images/statusWarning.svg';
    const infoImgSrc = '/images/infoStatus.svg';

    const [logs, setLogs] = useState([]);
    const [skipLogsRecordsCount, setSkipLogsRecordsCount] = useState(0);
    const [takeLogsRecordsCount, setTakeLogsRecordsCount] = useState(100);
    const [currentSelectedShedule, setCurrentSelectedShedule] = useState(selectedSheduleToLogs);

    useMemo(async () => {
        if (selectedSheduleToLogs != currentSelectedShedule && selectedSheduleToLogs!=undefined) {
            setSkipLogsRecordsCount(0);
            setCurrentSelectedShedule(selectedSheduleToLogs);
            await GetShedulesLogsById({
                shedule_id: selectedSheduleToLogs?.id,
                skip: 0,
                take: takeLogsRecordsCount,
                setLogsState: _setLogsCustom,
                setError: setError,
                setIsAuthenticated: setIsAuthenticated
            });
        }
        else if (selectedSheduleToLogs != currentSelectedShedule && selectedSheduleToLogs == undefined) {
            setSkipLogsRecordsCount(0);
            setCurrentSelectedShedule(selectedSheduleToLogs);
            await GetShedulesLogs({
                skip: 0,
                take: takeLogsRecordsCount,
                setLogsState: _setLogsCustom,
                setError: setError,
                setIsAuthenticated: setIsAuthenticated
            });
        }
    }, [selectedSheduleToLogs]);

    useEffect(async () => {
        await GetShedulesLogs({
            skip: skipLogsRecordsCount,
            take: takeLogsRecordsCount,
            setLogsState: _setLogsCustom,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
    }, []);
    function _setLogsCustom(response) {
        console.log(response);
        response.forEach(element => {
            switch (element?.status) {
                case OK_STATUS: { element.status = okeyImgSrc; break; }
                case WARNING_STATUS:{ element.status = warningImgSrc; break;}
                case ERROR_STATUS: { element.status = errorImgSrc; break; }
                default: break;
            };
        });
        response = response.filter((value) => {
            if (value?.status != INFO_STATUS)
                return value;
        });

        if (selectedSheduleToLogs != currentSelectedShedule) {
            setLogs([...response]);
        }
        else {
            setLogs([...logs, ...response]);
        }
    }
    async function _onScroll(e) {
        var _tableGlobal = document.getElementById(style.tableGlobal);
        console.log(`scroll offsetHeight+scrollTop = ${_tableGlobal.offsetHeight+_tableGlobal.scrollTop}`);
        console.log(`scroll height ${_tableGlobal.scrollHeight}`);
        if (Math.ceil(_tableGlobal.offsetHeight + _tableGlobal.scrollTop) >= _tableGlobal.scrollHeight-1) {
            console.log('Scroll');
            var _skip = skipLogsRecordsCount+ takeLogsRecordsCount;

            if (selectedSheduleToLogs == undefined) {
                await GetShedulesLogs({
                    skip: _skip,
                    take: takeLogsRecordsCount,
                    setLogsState: _setLogsCustom,
                    setError: setError,
                    setIsAuthenticated: setIsAuthenticated
                });
            }
            else {
                await GetShedulesLogsById({
                    shedule_id: selectedSheduleToLogs?.id,
                    skip: _skip,
                    take: takeLogsRecordsCount,
                    setLogsState: _setLogsCustom,
                    setError: setError,
                    setIsAuthenticated: setIsAuthenticated
                });
            }
            setSkipLogsRecordsCount(_skip);
        }
    }

    async function _resetLogs(e) {
        await ResetAllLogs({
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
        setLogs([]);
    }

    function _sheduleFilterOnClose(e) {
        setSelectedSheduleToLogs(undefined);
    }

    return (
        <div>
            <div className={style.globalBlock}>
                <div className={style.header}>
                    <div className={style.headerName}>
                        Shedules Logs
                        {
                            selectedSheduleToLogs != undefined ?
                                <button className={style.sheduleLogFilterCloseBtn}
                                    onClick={_sheduleFilterOnClose}
                                    title="Click to reset filter"
                                >
                                    {selectedSheduleToLogs?.name.length > 24 ? selectedSheduleToLogs?.name.substr(0,20)+"...":selectedSheduleToLogs?.name}
                                    <img className={style.sheduleFilterCloseImg} src="/images/sheduleBlankImg/close.svg"></img>
                                </button>
                                : <span />
                        }
                    </div>
                    <div className={style.headerAddBtn}>
                        <EllipseBtnWithRightImg
                            imgSrc={'/images/sheduleBlankImg/reset.svg'}
                            btnColor='#8C90E3'
                            btnTxt='Reset'
                            title='Reset all logs'
                            txtFontStyle='normal'
                            txtFontWeight='600'
                            onClick={_resetLogs}
                        />

                    </div>
                </div>
                <div id={style.tableGlobal} className={style.tableGlobal} onScroll={_onScroll}>
                    
                    <TableContainer>
                        <Table className={style.tableBlock} variant='striped' colorScheme='orange'>
                            <Thead >
                                <Tr className={style.tableHead}>
                                    <Th className={style.tableThLeft}>Status</Th>
                                    <Th>Date/Time</Th>
                                    <Th>Shedule</Th>
                                    <Th className={style.tableThRight}>Desc</Th>
                                </Tr>
                            </Thead>
                            <Tbody className={style.tableBody}>
                                {
                                    logs.map(element => <Tr key={element?.id}>
                                        <Td className={style.statusTd}>
                                            <img className={style.statusImg} src={element?.status}></img>
                                        </Td>
                                        <Td >{element?.dateTime}</Td>
                                        <Td >{element?.shedule_name}</Td>
                                        <Td >{element?.description}</Td>
                                    </Tr>)
                                }
                            </Tbody>
                        </Table>
                    </TableContainer>
                    {
                        logs.length == 0 ? <div className={style.contToPicture}><img className={style.emptyLogsPicture} src="/images/fly.svg"></img></div> : <span />
                    }
                </div>
            </div>
        </div>
    );
}