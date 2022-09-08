import React, { useEffect, useState } from "react";
import GetIndicByAddr from "../../../../../API/PowerIndications/GetIndicByAddr";
import GetIndicByAddrEnd from "../../../../../API/PowerIndications/GetIndicByAddrEnd";
import GetIndicByAddrStartEnd from "../../../../../API/PowerIndications/GetIndicByAddrStartEnd";
import GetIndicByAddrStart from "../../../../../API/PowerIndications/GetIndicByAddrStart";


export default async function UpdatePowerTable({ currentPage, startDate, endDate, meters, recordsCountToPage, pagesCount,activeNavName, setPagesCount, setCurrentPage, setPowersTableBody, setIsAuthenticated, setError, setIsLoading }) {


    function setLocalIndicationsCountSpecAddr(indicCount) {
        var currentPageLocal = {
            curPage: currentPage
        };
        checkPages(indicCount.count, currentPageLocal);

        GetIndicByAddr({
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

        GetIndicByAddrEnd({
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

        GetIndicByAddrStart({
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

        GetIndicByAddrStartEnd({
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
    function setIndicationsSpec(indications) {
        let rowNumber = currentPage * recordsCountToPage - recordsCountToPage + 1;
        let rows = [];
        if (indications != null) {
            indications.map((tr) => {
                var row = <tr key={rowNumber}>
                    <td hidden>{tr.id}</td>
                    <th scope="row">{rowNumber}</th>
                    <td>{tr.address ?? 'undefined'}</td>
                    <td>{tr.date ?? 'undefined'}</td>
                    <td>{tr.time ?? 'undefined'}</td>
                    <td>{tr.pplus ?? 'undefined'}</td>
                    <td>{tr.pminus ?? 'undefined'}</td>
                    <td>{tr.qplus ?? 'undefined'}</td>
                    <td>{tr.qminus ?? 'undefined'}</td>
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
        if (activeNavName == 'Показания мощностей') {
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
                    GetIndicByAddrStartEnd({
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
                    GetIndicByAddrStart({
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
                GetIndicByAddrEnd({
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
                GetIndicByAddr({
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