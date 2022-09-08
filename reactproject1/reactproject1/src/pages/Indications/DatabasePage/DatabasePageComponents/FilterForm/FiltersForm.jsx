import { DeleteIcon, RepeatIcon } from "@chakra-ui/icons";
import { Button, useToast } from "@chakra-ui/react";
import React, { useMemo, useState } from "react";
import "../../../../../../node_modules/bootstrap/dist/css/bootstrap.min.css"
import SelectField from "../../../../../components/UI/SelectField/SelectField";
import style from "./FilterForm.module.css"

export default function FilterForm({ companies, selectedCompany, meters, setSelectedCompany, setMeters, setStartDate,setEndDate,deleteClick }) {
    const toast = useToast();
    const [infoMesFlag, setInputMesFlag] = useState(false);

    function inputFocus() {
        if (infoMesFlag == false) {
            toast({
                title: "Информация",
                description: 'Заполните поле в формате: [Число],[Число],[Число]...',
                position: 'bottom',
                isClosable: true,
                status: 'info',
                duration: 5000
            });
            setInputMesFlag(true);
        }
    }

    function inputKeyUp(e) {
        console.log(e);
        if (e.code == "Backspace") {
            console.log("back");
            let _value = e.target.value;
            setMeters(_value.substr(0, _value.length - 1));
            return;
        }
        if ((e.keyCode >= 48 && e.keyCode <= 57)
            || (e.keyCode == 188 && e.key == ',')|| e.keyCode == 32|| e.keyCode == 8) {
            setMeters(e.target.value+e.key)
        }
        else {
            e.target.value = meters;
            toast({
                title: "Предупреждение",
                description: 'Обнаружены недопустимые символы',
                position: 'bottom',
                isClosable: true,
                status: 'warning',
                duration: 3000
            });
        }
    }

    function startDateBlur(e) {
        setStartDate(e.target.value);
    }
    function endDateBlur(e) {
        setEndDate(e.target.value);
    }

    return (
        <div className={style.mainDiv}>
            <div id={style.inputGroup} className="input-group mb-3">
                <span className="input-group-text" id="basic-addon1">Начало периода</span>
                <input className="form-control" type="date" onBlur={startDateBlur}></input>
                <span className="input-group-text" id="basic-addon1">Конец периода</span>
                <input className="form-control" type="date" onBlur={endDateBlur}></input>
            </div>

            <div id={style.inputGroup} className="input-group mb-3">
                <span className="input-group-text" id="basic-addon1">Организация</span>
                <SelectField add_new_flag={false} options={companies} selected={selectedCompany} setSelected={setSelectedCompany}></SelectField>
                <span className="input-group-text" id="basic-addon1">Адреса счётчиков</span>
                <input
                    type="text"
                    className="form-control"
                    value={meters}
                    readOnly={false}
                    onFocus={inputFocus}
                    onKeyUp={inputKeyUp}/>
            </div>

            <div className={style.btn_container}>
                <Button className={style.btn}
                    rightIcon={<DeleteIcon></DeleteIcon>}
                    colorScheme='red'
                    variant='solid'
                    onClick={deleteClick}
                >Удалить записи за выбранный период</Button>
            </div>
        </div>
    )
}