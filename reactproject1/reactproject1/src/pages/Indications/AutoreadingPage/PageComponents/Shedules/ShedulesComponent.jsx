import React, { useEffect, useState } from "react";
import GetShedulesInfo from "../../../../../API/IndicationsReading/GetShedulesInfo";
import EllipseBtnWithRightImg from "../../../../../components/UI/Buttons/EllipseBtnWithRightImg/EllipseBtnWithRightImg";
import SearchCustomElement1 from "../../../../../components/UI/Search/1/SearchCustomElement1";
import SheduleModalWindow from "./ModalWindow/ShedulesModalWindow";
import SheduleForm from "./SheduleForm/SheduleForm";
import style from './ShedulesComponent.module.css'


export default function ShedulesComponent({ sheduleForms,setSelectedShedule,setSheduleForms,setShedulesIsLoading, setShedulesNeedUpdate, setError, setIsAuthenticated}) {

    const search_session_item = 'shedules_search_str';
    const [search_value, set_search_value] = useState("");

    function _search_keyup(e){
        console.log(e);
        if (e.code == "Enter") {
            sessionStorage.setItem(search_session_item, e.target.value);
            setShedulesIsLoading(true);
            GetShedulesInfo({
                search_str: e.target.value,
                setShedules: setSheduleForms,
                setError: setError,
                setIsAuthenticated: setIsAuthenticated
            });
        }
    }
    useEffect(() => {
        var search_str = sessionStorage.getItem(search_session_item);
        if (search_str!== null) {
            set_search_value(search_str);
        }
    }, [])
    return (
        <div className={style.globalBlock}>
            <div className={style.header}>
                <div className={style.componentName}>
                    Shedules
                </div>
                <div className={style.headerBtnBlock}>
                    <SheduleModalWindow imgSrc={'/images/plus-green.svg'}
                        btnColor='#BBDCBC'
                        btnTxt='Add new shedule'
                        title='Добавить новое расписание'
                        txtFontStyle='normal'
                        txtFontWeight='600'
                        setError={setError}
                        setIsAuthenticated={setIsAuthenticated}
                        key='-1'
                        setNeedUpdate={setShedulesNeedUpdate}
                    ></SheduleModalWindow>
                </div>
            </div>

            <div className={style.searchMain}>
                <SearchCustomElement1 placeholder={"Search: enter shedule name...[Press 'Enter' to get shedules]"}
                    fontSize="1.1em"
                    onkeyup={_search_keyup}
                    defValue={search_value}
                    />
            </div>

            <div className={style.shedules}>
                {
                    sheduleForms.map((value) => {
                        return <SheduleForm key={value.shedule_id} sheduleName={value.sheduleName}
                            shedule_id={value.shedule_id}
                            status={value.status}
                            creatingDate={value.creatingDate}
                            communicPoint={value.communicPoint}
                            countRemMeters={value.countRemMeters}
                            countRemModems={value.countRemModems}
                            lastReadingValue={value.lastReadingValue}
                            shedule={value.shedule}
                            setShedulesNeedUpdate={setShedulesNeedUpdate}
                            setError={setError}
                            setIsAuthenticated={setIsAuthenticated}
                            setSelectedSheduleToLogs={setSelectedShedule}
                        />
                    })
                }
            </div>
        </div>
    )
}