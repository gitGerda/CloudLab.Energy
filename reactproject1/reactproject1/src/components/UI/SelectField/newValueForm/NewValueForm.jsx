import { off } from "bootstrap/js/dist/dom/event-handler";
import React, { useEffect, useState } from "react";
import "../../../../../node_modules/bootstrap/dist/css/bootstrap.min.css"
import CloseButton from "../../Buttons/CloseButton/CloseButton";
import style from "./NewValueForm.module.css"

export default function NewValueForm({ options, setOptionsState, close }) {
    
    const [valueToAdd, setValueToAdd] = useState({
        value: "",
        desc:""
    });

    function Add() {
        if (valueToAdd != "") {
            var f = [...options, valueToAdd];
            setOptionsState(f);
            setValueToAdd({
                value: "",
                desc: ""
            })
        }
        close();
    }

    return (
        <div id={style.card} className="card">
            <div id={style.cardHeader} className="card-header">
                <div id={style.desc}>Добавление нового значения</div>
                <CloseButton close={close}/>
            </div>
            <div className="card-body">
                <div className="input-group mb-3">
                    <span className="input-group-text" id="basic-addon1">Введите новое значение</span>
                    <input type="text" className="form-control" aria-label="Username"
                        aria-describedby="basic-addon1" value={valueToAdd.value}
                        onChange={(e) => setValueToAdd({
                            value: e.target.value,
                            desc: e.target.value
                        })} />
                </div>
                <button type="button" className="btn btn-success" onClick={Add}>Добавить</button>
            </div>
        </div>
    )
}