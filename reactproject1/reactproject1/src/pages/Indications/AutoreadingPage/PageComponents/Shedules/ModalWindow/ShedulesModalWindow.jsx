import {
    Input,
    Modal,
    ModalBody,
    ModalCloseButton,
    ModalContent,
    ModalHeader,
    ModalOverlay,
    Select, Table, TableContainer, Thead, Th, Tr, Td, Tbody, useDisclosure, ModalFooter, Checkbox, List, useToast
} from "@chakra-ui/react";
import React, { useEffect, useMemo, useState } from "react";
import EllipseBtnWithRightImg from "../../../../../../components/UI/Buttons/EllipseBtnWithRightImg/EllipseBtnWithRightImg";
import style from './SheduleModalWindow.module.css'
import '../../../../../../../node_modules/bootstrap/dist/js/bootstrap.js'
import GetSheduleSettingsMeters from "../../../../../../API/IndicationsReading/GetSheduleSettingsMeters";
import GetCommunicPoints from '../../../../../../API/IndicationsReading/GetCommunicPoints'
import CreateUpdateShedule from "../../../../../../API/IndicationsReading/CreateUpdateShedule";

export default function SheduleModalWindow({ imgSrc, btnColor, btnTxt, title, txtFontStyle, txtFontWeight, setError, shedule_id = null, shedule_name_def ='', communic_point_def = null, periodicity_def = '',formHeader='Add new shedule',status=1,setIsAuthenticated,setNeedUpdate=null}) {

    const { isOpen, onOpen, onClose } = useDisclosure();
    const saveImgSrc = '/images/save.svg';
    const toast = useToast();
    const [metersMs, setMetersMs] = useState([]);
    const [communic_points, set_communic_points] = useState([]);
    const [selected_periodicity, set_selected_periodicity] = useState(periodicity_def);
    const [selected_communic_point, set_selected_communic_point] = useState();
    const [shedule_name_state, set_shedule_name_state] = useState(shedule_name_def);
    const [start_date_state, set_start_date_state] = useState("");
    const helpInfoAboutPeriodicity = [
        {
            value: '',
            description: 'select periodicity for more information'
        },
        {
            value: 'every day',
            description: 'start reading indications every day at 00:00'
        },
        {
            value: 'every week',
            description: 'start reading indications every week on Monday at 00:00'
        },
        {
            value: 'every month',
            description: 'start reading indications every first day of the month at 00:00'
        }];

    useEffect(async () => {
        GetSheduleSettingsMeters({
            shedule_id: shedule_id,
            setMeters: setMetersMs,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
        GetCommunicPoints({
            setCommunicPoints: set_communic_points,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
    }, []);

    useMemo(() => {
        if (communic_points.length != 0) {
            communic_points.findIndex((element, index) => {
                if (element.name == communic_point_def) {
                    set_selected_communic_point(communic_points[index].id);
                    return true;
                }
            });
        }
    }, [communic_points]);

    function _check_form_before_save() {
        var message = 'Сheck the form data.The fields';
        var error_flag = false;
        var selected_meter_exist_flag = false;

        metersMs.forEach((value) => {
            if (value.select_flag == true) {
                selected_meter_exist_flag = true;
            }
        });

        if (shedule_name_state == '') { message += ' [Name]'; error_flag = true; };
        if (selected_periodicity == '') { message += ' [Periodicity by time]'; error_flag = true; }
        if (selected_communic_point == null) { message += ' [Communication Point]'; error_flag = true; }
        message += ' must be field in';
        if (selected_meter_exist_flag == false) { message += ' and at least one meter is selected'; error_flag = true; }

        if (error_flag == true) {
            toast({
                title: "Warning",
                description: message,
                position: 'bottom-right',
                isClosable: true,
                status: 'warning',
                duration: 3000
            });
            return false;
        }
        return true;
    }

    async function _save_and_close_on_click() {
        if (_check_form_before_save() == true) {
            var selectedMetersID = [];
            metersMs.forEach((value) => {
                if (value.select_flag == true) {
                    selectedMetersID.push(value.meter_id);
                }
            });
            
            var request_data = {
                shedule_id: shedule_id,
                shedule_name: shedule_name_state,
                communic_point_id: selected_communic_point,
                selected_meters_id: selectedMetersID,
                periodicity: selected_periodicity,
                status: Boolean(status),
                start_date: start_date_state
            }
            
            var result = await CreateUpdateShedule({
                data: request_data,
                setError: setError,
                setIsAuthenticated: setIsAuthenticated
            });
            
            if (result == true) {
                toast({
                    title: "Success",
                    description: 'Changes have been successfully applied',
                    position: 'bottom-right',
                    isClosable: true,
                    status: 'success',
                    duration: 3000
                });
                
                if (setNeedUpdate != null) {
                    setNeedUpdate(true);
                }
            };
            onClose();
        }
    }

    function _modal_window_on_open() {
        GetSheduleSettingsMeters({
            shedule_id: shedule_id,
            setMeters: setMetersMs,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
        GetCommunicPoints({
            setCommunicPoints: set_communic_points,
            setError: setError,
            setIsAuthenticated: setIsAuthenticated
        });
        if (shedule_id == null && shedule_name_state!=null) {
            set_shedule_name_state('');
            set_selected_periodicity('');
            set_selected_communic_point(null);
        }
        onOpen();
    }

    function _shedule_name_on_change(e) {
        if (shedule_name_def == '') {
            var symbol = e.nativeEvent.data;
            if (e.nativeEvent.inputType == 'insertText') {
                set_shedule_name_state(shedule_name_state + symbol);
            }
            else if (e.nativeEvent.inputType == 'deleteContentBackward') {
                set_shedule_name_state(shedule_name_state.substr(0, shedule_name_state.length - 1));
            }
        }
        else {
            console.log(e);
            toast({
                title: "Notification",
                description: 'You cannot change the name of the schedule after it is created',
                position: 'bottom-right',
                isClosable: true,
                status: 'info',
                duration: 3000
            });
        }
    }

    function _communic_point_on_change(e) {
        var index = e.target.options.selectedIndex;
        set_selected_communic_point(e.target.options[index].value);
    }

    function _periodicity_on_change(e) {
        var index = e.target.options.selectedIndex;
        set_selected_periodicity(e.target.options[index].value);
    }

    function _checkbox_on_change(e) {
        var meterMsCopy = [...metersMs];
        var index = meterMsCopy.findIndex((value) => {
            if (value.meter_id == e.target.id) {
                return true;
            }
        });
        if (e.target.checked) {
            meterMsCopy[index].select_flag = true;
        }
        else {
            meterMsCopy[index].select_flag = false;
        }
        setMetersMs(meterMsCopy);
    }
    function _start_date_on_change(e) {
        set_start_date_state(e.target.value);
    }

    return (
        <div>
            <EllipseBtnWithRightImg imgSrc={imgSrc}
                btnColor={btnColor}
                btnTxt={btnTxt}
                title={title}
                txtFontStyle={txtFontStyle}
                txtFontWeight={txtFontWeight}
                onClick={_modal_window_on_open}
            ></EllipseBtnWithRightImg>

            <Modal onClose={onClose} isOpen={isOpen} >
                <ModalOverlay />

                <ModalContent className={style.modalWindowContainer}>
                    <ModalHeader className={style.modalHeader}>{formHeader}</ModalHeader>
                    <ModalCloseButton></ModalCloseButton>
                    <ModalBody className={style.modalBody}>
                        <div className={style.modalBodyBlock}>
                            <div className={style.modalBlockRow}>
                                <span>Name:</span>
                                <Input className={style.modalBlockRowInput}
                                    variant='filled'
                                    placeholder='Enter shedule name'
                                    value={shedule_name_state}
                                    onChange={_shedule_name_on_change}
                                ></Input>
                            </div>
                            <div className={style.modalBlockRow}>
                                <span>Communication point:</span>
                                <Select className={style.modalBlockRowSelect}
                                    placeholder='Select communication point'
                                    defaultValue={selected_communic_point}
                                    onChange={_communic_point_on_change}
                                >
                                    {
                                        communic_points.map((value) => {
                                            return <option key={value.name} value={value.id}>{value.name}</option>
                                        })
                                    }
                                </Select>
                                
                            </div>
                            <div className={style.modalBlockTable}>
                                <span>Meters:</span>
                                <div className={style.tableGlobal}>
                                    <TableContainer>
                                        <Table className={style.tableBlock} variant='striped' colorScheme='blue'>
                                            <Thead>
                                                <Tr className={style.tableHead}>
                                                    <Th className={style.tableThLeft}></Th>
                                                    <Th>Type</Th>
                                                    <Th>Address</Th>
                                                    <Th>Company</Th>
                                                    <Th>XML80020</Th>
                                                    <Th>Interface</Th>
                                                    <Th className={style.tableThRight}>SIM</Th>
                                                </Tr>
                                            </Thead>
                                            <Tbody className={style.tableBody}>
                                                {
                                                    metersMs.map((value, index) => {
                                                        if (index != metersMs.length) {
                                                            return <Tr key={value.meter_id}>
                                                                <Td>
                                                                    <Checkbox className={style.checkBox}
                                                                        colorScheme={"green"}
                                                                        isChecked={Boolean(value.select_flag)}
                                                                        onChange={_checkbox_on_change}
                                                                        id={value.meter_id}

                                                                        ></Checkbox>
                                                                </Td>
                                                                <Td>{value.type}</Td>
                                                                <Td>{value.address}</Td>
                                                                <Td>{value.company}</Td>
                                                                <Td>{value.xml80020}</Td>
                                                                <Td>{value.communic_interface}</Td>
                                                                <Td>{value.sim_number}</Td>
                                                           </Tr>
                                                        }   
                                                        else {
                                                            return <Tr className={style.lastTr} key={value.meter_id}>
                                                                <Td>
                                                                    <Checkbox className={style.checkBox}
                                                                        colorScheme={"green"}
                                                                        isChecked={Boolean(value.select_flag)}
                                                                        onChange={_checkbox_on_change}
                                                                    ></Checkbox>
                                                                </Td>
                                                                <Td>{value.type}</Td>
                                                                <Td>{value.address}</Td>
                                                                <Td>{value.company}</Td>
                                                                <Td>{value.xml80020}</Td>
                                                                <Td>{value.communic_interface}</Td>
                                                                <Td>{value.sim_number}</Td>
                                                            </Tr>
                                                        }
                                                    })
                                                }
                                            </Tbody>
                                        </Table>
                                    </TableContainer>
                                </div>
                            </div>
                            <div className={style.modalBlockRow}>
                                <span>Periodicity by time:</span>
                                <Select className={style.modalBlockRowSelect}
                                    placeholder='Select periodicity'
                                    onChange={_periodicity_on_change}
                                    defaultValue={periodicity_def}
                                >
                                    <option value='every day'>every day</option>
                                    <option value='every week'>every week</option>
                                    <option value='every month'>every month</option>
                                </Select>
                            </div>

                            {
                                helpInfoAboutPeriodicity.map((value) => {
                                    if (value.value == selected_periodicity) {
                                        return <div key={shedule_id} className={style.helpMsg}>info: {value.description}</div>
                                    }
                                })
                            }
                            {shedule_id == null ?
                                <div className={style.modalBlockRow}>
                                    <span>Start date:</span>
                                    <Input className={style.modalBlockRowInput}
                                        type="datetime-local"
                                        variant='filled'
                                        onChange={_start_date_on_change}
                                    >
                                    </Input>
                                </div>
                                : <div></div>}

                            <div className={style.saveCloseBtns}>
                                <EllipseBtnWithRightImg
                                    imgSrc={saveImgSrc}
                                    btnColor= '#3EB62A'
                                    btnTxt='Save and close'
                                    txtFontWeight='600'
                                    title='Сохранить настройки'
                                    onClick={_save_and_close_on_click}
                                ></EllipseBtnWithRightImg>
                            </div>
                        </div>
                    </ModalBody>
                </ModalContent>
            </Modal>

        </div>
    )
}



// communic_points_obj signature
// {
//     "id": 1,
//     "name": "ws-oit-007",
//     "port": "COM3",
//     "description": "test"
// },
// metersMs_obj signature
// {
//     "meter_id": 8,
//     "type": "Меркурий 230",
//     "address": 8,
//     "company": "АО Кукморский завод Металлопосуды",
//     "xml80020": "040055560001104",
//     "communic_interface": "GSM",
//     "sim_number": "89196910238",
//     "select_flag": false
// },

// BODY SIGNATURE
//
// public class SheduleCreateUpdateModel {
//     public int?shedule_id
//         {
//     get; set;
// }
//         public string shedule_name
// {
//     get; set;
// } = "";
//         public int communic_point_id
// {
//     get; set;
// }
//         public List < int > selected_meters_id = new List < int > ();
//         public string periodicity
// {
//     get; set;
// } = "";
//     }