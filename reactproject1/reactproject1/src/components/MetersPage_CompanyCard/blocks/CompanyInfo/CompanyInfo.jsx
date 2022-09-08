import React from "react";
import "../../../../../node_modules/bootstrap/dist/css/bootstrap.min.css"
import styleModule from "./CompanyInfo.module.css"

export default function CompanyInfo({header}) {
    return (
        <div>
            <div id={styleModule.inputGroup} className="input-group flex-nowrap">
                <span className="input-group-text" id="addon-wrapping">Организация-потребитель</span>
                <input id={styleModule.inputId} defaultValue={header.orgGetter} type="text" className="form-control" placeholder="Организация-потребитель" readOnly />
            </div>

            <div id={styleModule.inputGroup} className="input-group flex-nowrap">
                <span className="input-group-text" id="addon-wrapping">ИНН потребителя</span>
                <input id={styleModule.inputId} defaultValue={header.orgGetterInn} type="text" className="form-control" placeholder="ИНН потребителя" readOnly  />
            </div>

            <div id={styleModule.inputGroup} className="input-group flex-nowrap">
                <span className="input-group-text" id="addon-wrapping">Организация-отправитель</span>
                <input id={styleModule.inputId} defaultValue={header.orgSender} type="text" className="form-control" placeholder="Организация-отправитель" readOnly />
            </div>
            <div id={styleModule.inputGroup} className="input-group flex-nowrap">
                <span className="input-group-text" id="addon-wrapping">ИНН отправителя</span>
                <input id={styleModule.inputId} defaultValue={header.orgSenderInn} type="text" className="form-control" placeholder="ИНН отправителя" readOnly />
            </div>
            <div id={styleModule.inputGroup} className="input-group flex-nowrap">
                <span className="input-group-text" id="addon-wrapping">Номер договора</span>
                <input id={styleModule.inputId} defaultValue={header.contract} type="text" className="form-control" placeholder="Номер договора" readOnly/>
            </div>
            <div id={styleModule.inputGroup} className="input-group flex-nowrap">
                <span className="input-group-text" id="addon-wrapping">Первоначальный номер отчёта</span>
                <input id={styleModule.inputId} defaultValue={header.msgNumber} type="text" className="form-control" readOnly />
            </div>
            <div id={styleModule.inputGroup} className="input-group flex-nowrap">
                <span className="input-group-text" id="addon-wrapping">Дата первоначального номера отчёта</span>
                <input id={styleModule.inputId} defaultValue={header.msgNumberDate} type="text" className="form-control" readOnly/>
            </div>

        </div>
    )
}