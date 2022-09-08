import React from "react";
import EllipseBtnWithRightImg from "../../../../../components/UI/Buttons/EllipseBtnWithRightImg/EllipseBtnWithRightImg";
import style from './CommunicPoint.module.css';
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
    useToast,
} from '@chakra-ui/react'
import CommunicPointModalWindow from "./CommunicPointModalWindow/CommunicPointModalWindow";
import DeleteCommunicPoint from "../../../../../API/IndicationsReading/DeleteCommunicPoint";

export default function CommunicPoint({ communicPointsMs,setCommunicPointsNeedUpdate, setError, setIsAuthenticated }) {

    const toast = useToast();

    async function _deleteCommunicPoint(e) {
        var _communic_point_id = e.target.id.replace("delete_", "");
        await DeleteCommunicPoint({
            communic_point_id: _communic_point_id,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated,
            setResponse:_deleteCommunicPoint_ResponseProcessing
        });
    }
    function _deleteCommunicPoint_ResponseProcessing(response) {
        if (response?.status == false) {
            var _shedules_names = "";
            response?.description.forEach(element => {
                _shedules_names += `[${element}] `;
            });
            toast({
                title: "Warning",
                description: `Communication point cannot be deleted, because it use in schedule(s): ${_shedules_names}`,
                position: 'bottom-right',
                isClosable: true,
                status: 'warning',
                duration: 5000
            }); 
        }
        else if (response?.status == true) {
            toast({
                title: "Success",
                description: "Communication point was successfully deleted",
                position: 'bottom-right',
                isClosable: true,
                status: 'info',
                duration: 3000
            }); 
            setCommunicPointsNeedUpdate(true);
        }
    }

    return (
        <div className={style.globalBlock}>
            <div className={style.header}>
                <div className={style.headerName}>
                    Communication points
                </div>
                <div className={style.headerAddBtn}>
                    <CommunicPointModalWindow
                        imgSrc={'/images/plus-green.svg'}
                        btnColor='#BBDCBC'
                        btnTxt='Add point'
                        title='Добавить новую точку связи'
                        txtFontStyle='normal'
                        txtFontWeight='600' 
                        formHeader={"Add New Communicaton Point"}
                        setError={setError}
                        setIsAuthenticated={setIsAuthenticated}
                        setCommunicPointsNeedUpdate={setCommunicPointsNeedUpdate}
                    ></CommunicPointModalWindow>
                </div>
            </div>
            <div className={style.tableGlobal}>
                <TableContainer>
                    <Table className={style.tableBlock} variant='striped' colorScheme='purple'>
                        <Thead>
                            <Tr className={style.tableHead}>
                                <Th className={style.tableThLeft}>Name</Th>
                                <Th>Port</Th>
                                <Th>Desc</Th>
                                <Th className={style.tableThRight}>Action</Th>
                            </Tr>
                        </Thead>
                        <Tbody className={style.tableBody}> 
                            {
                                communicPointsMs.map((value, index) => {
                                    var _last = index == communicPointsMs.length - 1 ? style.lastTr : undefined;
                                    return <Tr className={_last} key={value.id}>
                                        <Td>{value.name}</Td>
                                        <Td>{value.port}</Td>
                                        <Td>{value.description}</Td>
                                        <Td>
                                            <div className={style.actionsTd}>
                                                <CommunicPointModalWindow
                                                    communic_point_id={value.id}
                                                    changeBtnFlag={true}
                                                    title='Изменить настройки точки связи'
                                                    formHeader={"Reconfiguring Communication Point"}
                                                    setError={setError}
                                                    setIsAuthenticated={setIsAuthenticated}
                                                    setCommunicPointsNeedUpdate={setCommunicPointsNeedUpdate} />
                                                <span> | </span>
                                                <button id={"delete_" + value.id}
                                                    className={style.tableActionDelete}
                                                    onClick={_deleteCommunicPoint}>delete</button>
                                            </div>
                                        </Td>
                                    </Tr>
                                })
                            }
                        </Tbody>
                    </Table>
                </TableContainer>
            </div>
        </div>
    );
}