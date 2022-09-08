import { toast, useDisclosure, useToast } from "@chakra-ui/react";
import React from "react";
import ChangeSheduleStatus from "../../../../../../API/IndicationsReading/ChangeSheduleStatus";
import DeleteShedule from "../../../../../../API/IndicationsReading/DeleteShedule";
import AlertDialogYesNo from "../../../../../../components/UI/Alert/AlertDialogYesNo/AlertDialogYesNo";
import EllipseBtnWithRightImg from "../../../../../../components/UI/Buttons/EllipseBtnWithRightImg/EllipseBtnWithRightImg";
import EllipseBtnWithRightImgWithModalYesNo from "../../../../../../components/UI/Buttons/EllipseBtnWithRightImg/EllipseBtnWithRightImgWithModalYesNo/EllipseBtnWithRightImgWithModalYesNo";
import SheduleModalWindow from "../ModalWindow/ShedulesModalWindow";
import style from './SheduleForm.module.css'

export default function SheduleForm({ shedule_id = 0, sheduleName = 'default', status = 1, creatingDate = '01.01.2000 00:00:00', shedule = 'default', communicPoint = 'default', countRemModems = 0, countRemMeters = 0, lastReadingValue = '01.01.2000 00:00:00', setShedulesNeedUpdate, setError,setIsAuthenticated,setSelectedSheduleToLogs}) {
    
    const statusOpenImgSrc = '/images/sheduleBlankImg/antenna.svg';
    const statusCloseImgSrc = '/images/sheduleBlankImg/wifioff_111094.svg';
    const closeBtnImgSrc = '/images/sheduleBlankImg/close.svg';
    const bodyActSettingsImgSrc = '/images/sheduleBlankImg/settings.svg'
    const bodyActLogImgSrc = '/images/sheduleBlankImg/logs.svg';
    const bodyActChangStatImgSrc = '/images/sheduleBlankImg/pencil.svg';
    const toast = useToast();

    const { isOpen, onOpen, onClose } = useDisclosure();

    async function _changeSheduleStatus() {
        let statusToChange = status == 1 ? 'false' : 'true';
        await ChangeSheduleStatus({ shedule_id: shedule_id, status: statusToChange, setError: setError });
        setShedulesNeedUpdate(true);
    }
    function _setSelectedToLogsCustom() {
        var _selected = {
            id: shedule_id,
            name: sheduleName
        };
        setSelectedSheduleToLogs(_selected);
    }

    async function _delete_shedule() {
        var result = await DeleteShedule({ shedule_id: shedule_id, setError: setError, setIsAuthenticated: setIsAuthenticated });
        onClose();
        if (result == true) {
            toast({
                title: "Success",
                description: "The schedule was successfully deleted",
                position: 'bottom',
                isClosable: true,
                status: 'success',
                duration: 3000
            });
        };
        setShedulesNeedUpdate(true);
    }
    return (
        <div className={style.mainForm}>
            
            <div className={style.header}>
                <div className={style.headerName}>
                    <span className={style.description}>Shedule name:</span>
                    <span className={style.emptySpace}>.......</span>
                    <span className={style.sheduleName} title={sheduleName}>{`${sheduleName.substring(0,11)}${sheduleName.length>15? "...":""}`}</span>
                </div>
                <div className={style.headerStatus}>
                    <span className={style.statusDescription}>Status:</span>
                    <span className={style.emptySpace}>.....</span>

                    {status == 0 ? <span className={style.status + ' ' + style.statusClosed}>Closed</span>    
                        : <span className={style.status + ' ' + style.statusOpen}>Open</span>}

                    {status == 0 ? <img className={style.statusImg} src={statusCloseImgSrc}></img>
                        : <img className={style.statusImg} src={statusOpenImgSrc}></img> }                   
                </div>
                <div className={style.headerCloseBtn} onClick={onOpen}>
                    <img className={style.closeBtn} src={closeBtnImgSrc} title='Удалить расписание'></img>
                    <AlertDialogYesNo isOpenDisclosure={isOpen}
                        onCloseDisclosure={onClose}
                        onYesClick={_delete_shedule}
                        header={<div style={{ fontFamily: 'Consolas' }}>Do you really want to delete this shedule?</div>}
                        body={<div>
                            <span style={{ fontFamily: 'Consolas' }}>The schedule will be completely removed from the settings</span>
                        </div>}
                    />
                </div>
            </div>

            <div className={style.body}>
                <div className={style.bodyInfo}>
                    <div>
                        <span>Creating date:</span>
                        <span className={style.emptySpace}>..</span>
                        <span className={style.bodyInfoRecord}>{creatingDate}</span>
                    </div>
                    <div>
                        <span>Shedule:</span>
                        <span className={style.emptySpace}>..</span>
                        <span className={style.bodyInfoRecord}>{shedule}</span>
                    </div>
                    <div>
                        <span>Communication point:</span>
                        <span className={style.emptySpace}>..</span>
                        <span className={style.bodyInfoRecord}>{communicPoint}</span>
                    </div>
                    <div>
                        <span>Count of remote modems:</span>
                        <span className={style.emptySpace}>..</span>
                        <span className={style.bodyInfoRecord}>{countRemModems}</span>
                    </div>
                    <div>
                        <span>Count of remote meters:</span>
                        <span className={style.emptySpace}>..</span>
                        <span className={style.bodyInfoRecord}>{countRemMeters}</span>
                    </div>
                </div>

                <div className={style.bodyActions}>                   
                    <div className={style.bodyActionsMargins}>
                        <SheduleModalWindow
                            btnColor={'#8BB788'}
                            title='Изменить настройки расписания'
                            btnTxt='Settings'
                            imgSrc={bodyActSettingsImgSrc} 
                            setError={setError}
                            shedule_id={shedule_id}
                            shedule_name_def={sheduleName}
                            communic_point_def={communicPoint}
                            periodicity_def={shedule}
                            formHeader='Shedule settings'
                            status={status}
                            setIsAuthenticated={setIsAuthenticated}
                            key={shedule_id}
                            setNeedUpdate={setShedulesNeedUpdate}
                        ></SheduleModalWindow>

                        <EllipseBtnWithRightImg btnColor={'#D9C723'}
                            title='Просмотреть историю выполнения расписания'
                            btnTxt='Logs'
                            imgSrc={bodyActLogImgSrc} 
                            margin='0 1vw 0 0'
                            onClick={_setSelectedToLogsCustom}
                            />
                        
                        <EllipseBtnWithRightImgWithModalYesNo btnColor={'#8895B7'}
                            title='Изменить статус расписания'
                            btnTxt='Change status'
                            imgSrc={bodyActChangStatImgSrc}
                            header={<div style={{fontFamily:'Consolas'}}>Do you really want to change the schedule status?</div>}
                            body={<div>
                                <span style={{ fontFamily: 'Consolas' }}>The schedule status will be changed to </span>
                                <span style={{ fontFamily: 'Consolas',color:'red',fontWeight:'600' }}>{status == 1 ? ' CLOSED' : ' OPEN'}</span>
                            </div>}
                            onYesClick={_changeSheduleStatus}
                        />                         
                    </div>
                </div>

                <div className={style.bodyLastReading}>
                    <span className={style.lastReadDesc}>Last reading date:</span>
                    <span className={style.emptySpace}>...</span>
                    <span className={style.lastReadValue}>{lastReadingValue}</span>
                </div>
            </div>
            

        </div>
    );
}