import React,{useEffect} from "react";
import GetEnergyIndicByAddrStartEnd from "../../../../../API/EnergyIndications/GetEnergyIndicByAddrStartEnd";
import GetEnergyIndicByAddrStart from "../../../../../API/EnergyIndications/GetEnergyIndicByAddrStart";
import GetEnergyIndicByAddrEnd from "../../../../../API/EnergyIndications/GetEnergyIndicByAddrEnd";
import GetEnergyIndicByAddr from "../../../../../API/EnergyIndications/GetEnergyIndicByAddr";
import style from "./UpdateEnergyTable.module.css"
import { useToast } from "@chakra-ui/react";
import DeleteEnergyIndications from "../../../../../API/EnergyIndications/DeleteEnergyIndications";


export default async function UpdateEnergyTable({ currentPage, startDate, endDate, meters, recordsCountToPage, pagesCount, activeNavName, setPagesCount, setCurrentPage, setPowersTableBody, setIsAuthenticated, setError, setIsLoading }) {
 
    const toast = useToast();

    function setLocalIndicationsCountSpecAddr(indicCount) {
        var currentPageLocal = {
            curPage: currentPage
        };
        checkPages(indicCount.count, currentPageLocal);

        GetEnergyIndicByAddr({
            pageNumber: currentPageLocal.curPage,
            recordsCountToPage: recordsCountToPage,
            countFlag: "false",
            addresses: meters,
            setIndications: setIndicationsSpec,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
    }
    function setLocalIndicationsCountSpecAddrEnd(indicCount) {
        var currentPageLocal = {
            curPage: currentPage
        };
        checkPages(indicCount.count, currentPageLocal);

        GetEnergyIndicByAddrEnd({
            endDate: endDate,
            pageNumber: currentPageLocal.curPage,
            recordsCountToPage: recordsCountToPage,
            countFlag: 'false',
            addresses: meters,
            setIndications: setIndicationsSpec,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
    }
    function setLocalIndicationsCountSpecAddrStart(indicCount) {
        var currentPageLocal = {
            curPage: currentPage
        };
        checkPages(indicCount.count, currentPageLocal);

        GetEnergyIndicByAddrStart({
            startDate: startDate,
            pageNumber: currentPageLocal.curPage,
            recordsCountToPage: recordsCountToPage,
            countFlag: 'false',
            addresses: meters,
            setIndications: setIndicationsSpec,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
    }
    function setLocalIndicationsCountSpecAddrStartEnd(indicCount) {
        var currentPageLocal = {
            curPage: currentPage
        };
        checkPages(indicCount.count, currentPageLocal);

        GetEnergyIndicByAddrStartEnd({
            startDate: startDate,
            endDate: endDate,
            pageNumber: currentPageLocal.curPage,
            recordsCountToPage: recordsCountToPage,
            countFlag: 'false',
            addresses: meters,
            setIndications: setIndicationsSpec,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
    }
    async function _delete_energy_indic(row_data) {
        console.log(row_data);
        var _result = await DeleteEnergyIndications({
            meter_id: row_data?.meterId ?? -1,
            month: row_data?.month ?? 0,
            year: row_data?.year ?? -1,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
        if (_result == true) {
            toast({
                title: "Успех",
                description: "Данные показаний энергий успешно удалены",
                position: 'bottom-right',
                isClosable: true,
                status: 'success',
                duration: 3000
            });
            setCurrentPage(2);
            setCurrentPage(1);
        }
    }
    function setIndicationsSpec(indications) {
        let rowNumber = currentPage * recordsCountToPage - recordsCountToPage + 1;
        let rows = [];
        if (indications != null) {
            indications.map((tr) => {
                var row = <tr key={rowNumber}>
                    <td hidden>{tr.meterId}</td>
                    <th scope="row">{rowNumber}</th>
                    <td>{tr.address ?? 'undefined'}</td>
                    <td>{tr.year ?? 'undefined'}</td>
                    <td>{tr.month ?? 'undefined'}</td>
                    <td>{tr.startValue ?? 'undefined'}</td>
                    <td>{tr.endValue ?? 'undefined'}</td>
                    <td>{tr.total ?? 'undefined'}</td>
                    <td>
                        <button onClick={()=>_delete_energy_indic(tr)}>
                            <span className={style.delete_btn}>Удалить</span>
                        </button>
                    </td>
                </tr>

                rows.push(row);
                rowNumber = rowNumber + 1;
            });
            setPowersTableBody(rows);
        }
        else {
            setPowersTableBody([]);
        }

        setIsLoading(false);
    }
    function checkPages(recordsCount, currentPageLocal) {
        let count = recordsCount ?? 0;
        var computedPages = Math.ceil(count / recordsCountToPage)
        if (computedPages != pagesCount) {
            setPagesCount(computedPages);
            setCurrentPage(1);
            currentPageLocal.curPage = 1;
        };
    }

    useEffect(() => {
        if (activeNavName == 'Показания энергий') {
            meters = meters.replace(" ", "");
            if (meters == "") {
                setPowersTableBody();
                return;
            }
            setIsLoading(true);
            setError({
                isError: false,
                errorDesc: ""
            });
            if (startDate != "") {
                if (endDate != "") {
                    //StartDateEndDateAddr
                    GetEnergyIndicByAddrStartEnd({
                        startDate: startDate,
                        endDate: endDate,
                        recordsCountToPage: "",
                        pageNumber: "",
                        countFlag: true,
                        addresses: meters,
                        setIndications: setLocalIndicationsCountSpecAddrStartEnd,
                        setError: setError,
                        setIsAuthenticated: setIsAuthenticated
                    })
                }
                else {
                    //StartDateAddr 
                    GetEnergyIndicByAddrStart({
                        startDate: startDate,
                        recordsCountToPage: "",
                        pageNumber: "",
                        countFlag: true,
                        addresses: meters,
                        setIndications: setLocalIndicationsCountSpecAddrStart,
                        setError: setError,
                        setIsAuthenticated: setIsAuthenticated
                    });
                }
            }
            else if (endDate != "") {
                //EndDateAddr
                GetEnergyIndicByAddrEnd({
                    endDate: endDate,
                    recordsCountToPage: "",
                    pageNumber: "",
                    countFlag: true,
                    addresses: meters,
                    setIndications: setLocalIndicationsCountSpecAddrEnd,
                    setError: setError,
                    setIsAuthenticated: setIsAuthenticated
                })
            }
            else {
                //Addr
                GetEnergyIndicByAddr({
                    pageNumber: "",
                    recordsCountToPage: "",
                    countFlag: "true",
                    addresses: meters,
                    setIndications: setLocalIndicationsCountSpecAddr,
                    setError: setError,
                    setIsAuthenticated: setIsAuthenticated
                });
            }
        }
    }
        , [currentPage, startDate, endDate, meters, recordsCountToPage, activeNavName]);
 }