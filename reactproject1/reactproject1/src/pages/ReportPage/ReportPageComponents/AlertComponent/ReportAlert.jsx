import React, { useMemo, useState } from "react";
import style from "./ReportAlert.module.css";
import { ArrowDownIcon } from "@chakra-ui/icons";
import { Button } from "@chakra-ui/react";

export default function ReportAlert({ isVisible, linkToDownload, set_visiible }) {
    const [visible, setVisible] = useState();

    useMemo(() => {
        if (isVisible == true) {
            setVisible();
        }
        else {
            setVisible({
                display: "none"
            });
        }
    },[isVisible])

    function close_alert() {
        set_visiible(false);
    }

    return (
        <div className={style.reportAlert} style={visible}>
            <img className={style.reportAlertCorrectImage} src="/images/correct.svg"></img>
            <span className={style.reportAlertText}>Отчёт успешно сформирован!</span>
            <Button className={style.reportAlertDownLoadBtn}
                size="xs"
                colorScheme={"facebook"}><a href={linkToDownload}>Скачать</a></Button>
            
            <div className={style.closeBtnContainer} onClick={close_alert}>
                <img src="/images/closeButton.svg" className={style.reportAlertClose}></img>
            </div>
        </div>
    )
}