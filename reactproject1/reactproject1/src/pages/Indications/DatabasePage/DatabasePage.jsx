import { background, toast, useToast } from "@chakra-ui/react";
import React, { useContext, useEffect, useMemo, useState } from "react";
import GetCompaniesNames from "../../../API/GetCompaniesNames";
import GetMetersByCompany from "../../../API/GetMetersByCompany";
import Accordion from "../../../components/UI/Accordion/Accordion";
import CardsPages from "../../../components/UI/CardsPages/CardsPages";
import Spinner1 from "../../../components/UI/Spinner1/Spinner1";
import { AuthContext } from "../../../context/context";
import style from "./DatabasePage.module.css"
import UpdatePowerTable from "./DatabasePageComponents/DataBasePageHooks/UpdatePowerTable";
import UpdateEnergyTable from "./DatabasePageComponents/DataBasePageHooks/UpdateEnergyTable";
import FilterForm from "./DatabasePageComponents/FilterForm/FiltersForm";
import TableDataBase from "./DatabasePageComponents/TableDataBase/TableDataBase";
import DeletePowerIndications from "../../../API/PowerIndications/DeletePowerIndications";

export default function DatabasePage() {
    const toast = useToast();
    const [recordsCountToPage, setRecordsCountToPage] = useState(100);
    const [pagesForView, setPagesForView] = useState(10);
    const [currentPage, setCurrentPage] = useState(1);
    const [pagesCount, setPagesCount] = useState(1);
    const [startDate, setStartDate] = useState("");
    const [endDate, setEndDate] = useState("");
    const [meters, setMeters] = useState("");
    const { setIsAuthenticated } = useContext(AuthContext);
    const [error, setError] = useState({
        isError: false,
        errorDesc: ''
    });
    const [companies, setCompanies] = useState();
    const [selectedCompany, setSelectedCompany] = useState();
    const [isLoading, setIsLoading] = useState(true);

    const [powersTableBody, setPowersTableBody] = useState([]);
    const [activeNavName, setActiveNavName] = useState('Показания мощностей');
    const [pages, setPages] = useState([
        {
            navName: "Показания мощностей",
            bodyElement: <TableDataBase headTable={<tr>
                <th scope="col">#</th>
                <th scope="col">Address</th>    
                <th scope="col">Date</th>
                <th scope="col">Time</th>
                <th scope="col">P+</th>
                <th scope="col">P-</th>
                <th scope="col">Q+</th>
                <th scope="col">Q-</th>
            </tr>}
                bodyTable={powersTableBody}
                pagesCount={pagesCount}
                pagesForView={pagesForView}
                curPageNum={currentPage}
                setCurPageNum={setCurrentPage}
                isLoading={isLoading}
            />,
            active: true
        },
        {
            navName: "Показания энергий",
            bodyElement: <TableDataBase headTable={<tr>
                <th scope="col">#</th>
                <th scope="col">Address</th>
                <th scope="col">Year</th>
                <th scope="col">Month</th>
                <th scope="col">StartValue</th>
                <th scope="col">EndValue</th>
                <th scope="col">Total</th>
            </tr>}
                bodyTable={powersTableBody}
                pagesCount={pagesCount}
                pagesForView={pagesForView}
                curPageNum={currentPage}
                setCurPageNum={setCurrentPage}
                isLoading={isLoading}
            ></TableDataBase>,
            active: false
        }
    ]);

    useMemo(() => {
        setPages([
            {
                navName: "Показания мощностей",
                bodyElement: <TableDataBase headTable={<tr>
                    <th scope="col">#</th>
                    <th scope="col">Address</th>
                    <th scope="col">Date</th>
                    <th scope="col">Time</th>
                    <th scope="col">P+</th>
                    <th scope="col">P-</th>
                    <th scope="col">Q+</th>
                    <th scope="col">Q-</th>
                </tr>}
                    bodyTable={powersTableBody}
                    pagesCount={pagesCount}
                    pagesForView={pagesForView}
                    curPageNum={currentPage}
                    setCurPageNum={setCurrentPage}
                    isLoading={isLoading}
                />,
                active: true
            },
            {
                navName: "Показания энергий",
                bodyElement: <TableDataBase headTable={<tr>
                    <th scope="col">#</th>
                    <th scope="col">Address</th>
                    <th scope="col">Year</th>
                    <th scope="col">Month</th>
                    <th scope="col">StartValue</th>
                    <th scope="col">EndValue</th>
                    <th scope="col">Total</th>
                    <th scope="col">Action</th>
                </tr>}
                    bodyTable={powersTableBody}
                    pagesCount={pagesCount}
                    pagesForView={pagesForView}
                    curPageNum={currentPage}
                    setCurPageNum={setCurrentPage}
                    isLoading={isLoading}
                ></TableDataBase>,
                active: false
            }
        ]);
    }, [powersTableBody, pagesCount, pagesForView, currentPage, isLoading,activeNavName])

    useMemo(() => {
        if (isLoading) {
            setPowersTableBody();
        }
    }, [isLoading])

    function setActiveNav(e) {
        var currentPages = [];
        let target = e.target.textContent;
        pages.forEach((value) => {
            if (value.navName == target) {
                value.active = true;
            }
            else {
                value.active = false;
            }
            currentPages.push(value);
        }) 
        setPages(currentPages);
        setActiveNavName(e.target.textContent);
        setPowersTableBody([]);
    }
    function setCompaniesSpec(companiesFromFetch) {
        let tempMS = [];
        companiesFromFetch.forEach(element => {
            tempMS.push({
                value: element?.name,
                desc: element?.name,
                selected: false,
                meters: []
            });
        });
        setCompanies(tempMS);
    }

    function setMetersSpec(metersFromFetch) {
        if (companies != null) {
            var localCompanies = companies;
            var company = localCompanies.find(value => {
                if (value.value == metersFromFetch[0].companyName) {
                    return value;
                };
            });
            company.meters = [];

            metersFromFetch.forEach(element => {
                var meter = {
                    id: element.idMeter,
                    address: element.address
                };
                company.meters.push(meter);
            })
            setCompanies(localCompanies);
        }
    }

    var seqRunner = function (deeds) {
        return deeds.reduce(function (p, deed) {
            return p.then(function () {
                // Выполняем следующую функцию только после того, как отработала
                // предыдущая.
                return deed();
            });
        }, Promise.resolve()); // Инициализируем очередь выполнения.
    };
    useEffect(async () => {
        if (companies == null) {
            await GetCompaniesNames({ setCompanies: setCompaniesSpec, setError: setError, setIsAuthenticated: setIsAuthenticated });
            await setSelectedCompany("По умолчанию");
        }
    }, []);

    useMemo(async () => {
        if (selectedCompany != "По умолчанию") {
            await GetMetersByCompany({
                companyName: selectedCompany,
                setMeters: setMetersSpec,
                setError: setError,
                setIsAuthenticated: setIsAuthenticated
            }).then(() => {
                companies?.find((value2) => {
                    if (value2.value == selectedCompany) {
                        var metersStr = "";
                        value2.meters.forEach((value3) => {
                            metersStr = metersStr + value3.address + " , ";
                        });
                        metersStr = metersStr.slice(0, metersStr.length - 2);
                        setMeters(metersStr);
                    }
                });
            });
        }
        else {
            var compToFetch = [];
            companies.forEach((value) => {
                if (value.meters.length == 0) {
                    var f = async () => {
                        await GetMetersByCompany({
                            companyName: value.value,
                            setMeters: setMetersSpec,
                            setError: setError,
                            setIsAuthenticated: setIsAuthenticated
                        })
                    };
                    compToFetch.push(f);
                };
            });
            seqRunner(compToFetch).then(() => {
                var metersStr = "";
                companies.forEach((value) => {
                    value.meters.forEach((value2) => {
                        metersStr = metersStr + value2.address + " , "
                    });
                });
                metersStr = metersStr.slice(0, metersStr.length - 2);
                setMeters(metersStr);
            });
        }
    }, [selectedCompany])


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
        };
    }, [error.isError])

    UpdatePowerTable({
        currentPage: currentPage,
        startDate: startDate,
        endDate: endDate,
        meters: meters,
        recordsCountToPage: recordsCountToPage,
        pagesCount: pagesCount,
        setPagesCount: setPagesCount,
        setCurrentPage: setCurrentPage,
        setPowersTableBody: setPowersTableBody,
        setIsAuthenticated: setIsAuthenticated,
        setError: setError,
        setIsLoading: setIsLoading,
        activeNavName: activeNavName
    });
    UpdateEnergyTable({
        currentPage: currentPage,
        startDate: startDate,
        endDate: endDate,
        meters: meters,
        recordsCountToPage: recordsCountToPage,
        pagesCount: pagesCount,
        setPagesCount: setPagesCount,
        setCurrentPage: setCurrentPage,
        setPowersTableBody: setPowersTableBody,
        setIsAuthenticated: setIsAuthenticated,
        setError: setError,
        setIsLoading: setIsLoading,
        activeNavName: activeNavName
    });

    function _delete_indications(e) {
        console.log("Meters: " + meters);
        console.log("Start_date: " + startDate);
        console.log("End_date: " + endDate);
        console.log("Current_page: "+activeNavName);

        let _start_date = startDate ? startDate : "01.01.1900";
        let _end_date;
        if (endDate) {
            _end_date = endDate;
        }
        else {
            let _date = new Date();
            _end_date= _date.getFullYear().toString() +"."+ (_date.getMonth()+1).toString() + "."+_date.getDate().toString();
        }

        let addresses = meters.replace(" ", "");
        addresses = addresses.split(',');
        let arr = new Array();
        addresses.map(async (value) => {
            if (activeNavName == "Показания мощностей") {
                let _result = await DeletePowerIndications({
                    address: value,
                    start_date: _start_date,
                    end_date: _end_date,
                    setError: setError,
                    setIsAuthenticated: setIsAuthenticated
                });
                if(_result == true) {
                    toast({
                        title: "Успешно",
                        description: "Данные были успешно удалены",
                        position: 'bottom-right',
                        isClosable: true,
                        status: 'success',
                        duration: 3000
                    });
                }
            }
        });
    }

    return (
        <div className={style.main_container}>
            <div className={style.filtCont}>
                {activeNavName == "Показания мощностей" ?
                    <Accordion headerTextValue={"Фильтры"}
                        childNodes={<FilterForm companies={companies}
                            selectedCompany={selectedCompany}
                            setSelectedCompany={setSelectedCompany}
                            meters={meters}
                            setMeters={setMeters}
                            setStartDate={setStartDate}
                            setEndDate={setEndDate}
                            deleteClick={_delete_indications}
                        ></FilterForm>}
                        style1={{
                            background: "#0C056D",
                            fontSize: "1em",
                            borderRadius: "7px 7px 0 0 ",
                            color: "#fff"
                        }}
                        style2={{ height: "35px" }}
                        styleBody={
                            {
                                background: "#E8F1F5",
                                borderRadius: "0 0 7px 7px",
                                padding: "1rem 0.3rem"
                            }
                        }
                    ></Accordion> : <div></div>}
            </div>

            <CardsPages pages={pages} activeNavName={activeNavName} setActiveNav={setActiveNav}></CardsPages>

        </div>
    )
}