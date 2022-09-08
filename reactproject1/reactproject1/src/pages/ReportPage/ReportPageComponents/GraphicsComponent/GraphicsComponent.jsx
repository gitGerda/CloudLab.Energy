import React from "react";
import style from "./GraphicsComponent.module.css";
import "../../../../../node_modules/bootstrap/dist/css/bootstrap.min.css";
import { Heading, List, ListIcon, ListItem, Spinner } from "@chakra-ui/react";
import { CalendarIcon, CheckCircleIcon, RepeatClockIcon, SettingsIcon } from "@chakra-ui/icons";

export default function GraphicsComponent({ infoBlockDescription, infoBlockTable,reportLnk, isLoading }) {
    return (
        <div>
            <div className={style.headerOfWindow}>
                <img className={style.headerImage} src="/images/report2.svg"></img>
                <span className={style.headerOfWindowText}>Детализация</span>
                    {infoBlockDescription?.reportPath != "" && isLoading == false ? <div className={style.download_cont}><a href={infoBlockDescription?.reportPath??reportLnk} className={style.download}>Скачать XML80020</a></div>:<p></p>}
            </div>

            <div className={style.infoBlockDescription}>
                <List spacing={3}>
                    <ListItem>
                        <ListIcon as={SettingsIcon} color='green.500' ></ListIcon>
                        <span className={style.infoBlockDescriptionNames}>Наименование организации: </span>
                        <span>{infoBlockDescription.companyName}</span>
                    </ListItem>
                    <ListItem>
                        <ListIcon as={CalendarIcon} color='green.500' ></ListIcon>
                        <span className={style.infoBlockDescriptionNames}>Дата формирования: </span>
                        <span>{infoBlockDescription.reportDate}</span>
                    </ListItem>
                    <ListItem>
                        <ListIcon as={RepeatClockIcon} color='green.500' ></ListIcon>
                        <span className={style.infoBlockDescriptionNames}>Период: </span>
                        <span>{infoBlockDescription.periodDate}</span>
                    </ListItem>
                </List>
            </div>

            <div className={style.infoBlockTable}>
                {isLoading === true ? <div className={style.spinner}><Spinner thickness='4px'
                    speed='0.65s'
                    emptyColor='gray.200'
                    color='blue.500' size='xl'></Spinner></div> :

                    <table className="table" id="meters_table">
                        <thead>
                            <tr>
                                <th scope="col">#</th>
                                <th scope="col">Тип</th>
                                <th scope="col">Адрес</th>
                                <th scope="col">Коэффициент<br />трансформации</th>
                                <th scope="col">Начало периода <br />(кВт*ч)</th>
                                <th scope="col">Конец периода <br />(кВт*ч)</th>
                                <th scope="col">Итого энергия<br />  (кВт*ч)</th>
                                <th scope="col">Итого мощность<br />  (кВт)</th>
                                <th scope="col">Дата чтения<br />показаний</th>
                                <th scope="col">Сверка итоговых показаний</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                infoBlockTable.length > 0 ? infoBlockTable.map((value, index) => {
                                    return <tr className={value.detalizationType} key={index}>
                                        <th scope="row">{index + 1}</th>
                                        <td>{value.tblColMeterType}</td>
                                        <td>{value.tblColMeterAddress}</td>
                                        <td>{value.tblColTransformRatio}</td>
                                        <td>{value.tblColPeriodStart}</td>
                                        <td>{value.tblColPeriodEnd}</td>
                                        <td>{value.tblColEnergySum}</td>
                                        <td>{value.tblColPowerSum}</td>
                                        <td>{value.tblColDateOfRead}</td>
                                        <td>{value.tblColCheck}</td>
                                    </tr>
                                })
                                    : <tr></tr>
                            }
                        </tbody>
                    </table>}
                <span id="GraphicsComponent" className={style.tableTxt}>**Примечание: при сверке значений используются итоговые показания мощности и энергии за указанный период</span>

            </div>

        </div>
    )
}