import React, { useEffect, useState } from "react";
import "../../../../node_modules/bootstrap/dist/css/bootstrap.min.css"
import ModalWindow from "../ModalWindow/ModalWindow";
import ModalWindow2 from "../ModalWindow2/ModalWindow2";
import NewValueForm from "./newValueForm/NewValueForm";
import style from "./SelectField.module.css"


export default function SelectField({ options,selected, setSelected,add_new_flag=true}) {
    let index = 0;
    const [optionsState, setOptionsState] = useState([{
        value: "По умолчанию",
        desc: "По умолчанию",
        selected: false
    }]);
    const [modalVisible, setModalVisible] = useState(false);

    function AddNewOption(e) {
        if (e.target.value == "addNewOptionValue") {
            setModalVisible(true);
        }
        else {
            setSelected(e.target.value);
        }
    }

    useEffect(() => {
        if (options != null && options.length != 0)
            setOptionsState([...optionsState, ...options]);
    }, [options])

    return (
        <div>
            <select className="form-select" id={"inputGroupSelect" + Date.now()} onChange={AddNewOption}>

                {optionsState.map(element => {
                    let key = Math.random().toString(16).slice(2);
                    if (element.value == selected) {
                        return <option selected key={key} defaultValue={element.value}>{element.desc}</option>
                    }
                    else {
                        return <option key={key} defaultValue={element.value}>{element.desc}</option>
                    }
                }
                )}

                {add_new_flag == true ?
                    <option id={style.addOption} value="addNewOptionValue">
                        Добавить...
                    </option>
                    : null} 
            </select>
            <ModalWindow2 visible={modalVisible}
                setVisible={setModalVisible}
                children={
                    <NewValueForm options={optionsState} setOptionsState={setOptionsState} close={() => setModalVisible(false)} />
                } />
        </div>
    )
}