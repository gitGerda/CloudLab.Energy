import react, { useEffect, useMemo, useState } from "react";
import style from "./CommunicPointModalWindow.module.css"
import EllipseBtnWithRightImg from "../../../../../../components/UI/Buttons/EllipseBtnWithRightImg/EllipseBtnWithRightImg";
import {
    Input,
    Modal,
    ModalBody,
    ModalCloseButton,
    ModalContent,
    ModalHeader,
    ModalOverlay,
    Select, Table, TableContainer, Thead, Th, Tr, Td, Tbody, useDisclosure, ModalFooter, Checkbox, List, useToast } from "@chakra-ui/react";
import GetCommunicPoints from "../../../../../../API/IndicationsReading/GetCommunicPoints";
import AddNewCommunicPoint from "../../../../../../API/IndicationsReading/AddNewCommunicPoint";
import ChangeCommunicPoint from "../../../../../../API/IndicationsReading/ChangeCommunicPoint";

export default function CommunicPointModalWindow({changeBtnFlag=false,communic_point_id="",imgSrc,btnTxt,title,txtFontStyle,txtFontWeight,btnColor,formHeader,setError,setIsAuthenticated, setCommunicPointsNeedUpdate}) {
    
    const { isOpen, onOpen, onClose } = useDisclosure();
    const saveImgSrc = '/images/save.svg';

    const toast = useToast();
    const [name_state, set_name_state] = useState("");
    const [port_state, set_port_state] = useState("");
    const [desc_state, set_desc_state] = useState("");
    const [exist_comm_points_state, set_exist_comm_points] = useState([]);

    useMemo(async () => {
        if (isOpen == true) {
            await GetCommunicPoints({
                setCommunicPoints: set_exist_comm_points_custom, 
                setError: setError,
                setIsAuthenticated: setIsAuthenticated
            });
        }
    }, [isOpen]);

    function set_exist_comm_points_custom(communic_points) {
        if (communic_point_id != "") {
            var _filtred = communic_points.filter((element) => {
                if (element?.id == communic_point_id) {
                    set_name_state(element?.name);
                    set_port_state(element?.port);
                    set_desc_state(element?.description);
                    return element;
                }
            });
            if (_filtred.length == 0) {
                toast({
                    title: "Error",
                    description: "Can not find this communic_point",
                    position: 'bottom-right',
                    isClosable: true,
                    status: 'error',
                    duration: 3000
                });
                onClose();      
            }               
        }
        set_exist_comm_points(communic_points); 
    }
    async function _on_save_and_close() {
        //проверить name и port на заполненность 
        if (name_state == "" || port_state == "") {
            toast({
                title: "Warning",
                description: "Check name and port fields. These fields are required.",
                position: 'bottom-right',
                isClosable: true,
                status: 'warning',
                duration: 3000
            });
            return;
        }
        if (communic_point_id == "") {
            var result = await AddNewCommunicPoint({
                name: name_state,
                port: port_state,
                desc: desc_state == "" ? "..." : desc_state,
                setError: setError,
                setIsAuthenticated: setIsAuthenticated
            });
            _proccessingTrueResult(result, "New communication  point was successfully added");
        }
        else {
            var result = await ChangeCommunicPoint({
                communic_point_id: communic_point_id,
                port: port_state,
                desc: desc_state,
                setError: setError,
                setIsAuthenticated: setIsAuthenticated
            });
            _proccessingTrueResult(result, "Communication point was successfully reconfigured");
        }
    }
    function _proccessingTrueResult(result, message) {
        if (result == true) {
            onClose();
            toast({
                title: "Success",
                description: message,
                position: 'bottom-right',
                isClosable: true,
                status: 'success',
                duration: 3000
            });
            setCommunicPointsNeedUpdate(true);
        }
    }    
    function _port_onBlur(e) {
        set_port_state(e.target.value);
    }
    function _desc_onBlur(e) {
        set_desc_state(e.target.value);
    }
    function _name_onFocus(e) {
        e.target.classList.remove(style.redborder);
    }
    function _name_onBlur(e) {
        var _value = e.target.value;
        var _check_existing_ms = exist_comm_points_state.filter((value) => {
            if (value?.name == _value)
                return value;
        });
        if (_check_existing_ms.length != 0) {
            e.target.classList.add(style.redborder);
            toast({
                title: "Warning",
                description: "You can not use this name for communication point, because this name is already taken",
                position: 'bottom-right',
                isClosable: true,
                status: 'warning',
                duration: 3000
            });
            set_name_state("");
        }
        else {
            set_name_state(_value);
        }
    }
    return( 
        <div>
            {changeBtnFlag == true ? <button className={style.tableActionChange} onClick={onOpen}>change</button>
                :
                <EllipseBtnWithRightImg imgSrc={imgSrc}
                    btnColor={btnColor}
                    btnTxt={btnTxt}
                    title={title}
                    txtFontStyle={txtFontStyle}
                    txtFontWeight={txtFontWeight}
                    onClick={onOpen}
                ></EllipseBtnWithRightImg>
            }

            <Modal onClose={onClose} isOpen={isOpen} >
                <ModalOverlay />

                <ModalContent className={style.modalWindowContainer}>
                    <ModalHeader className={style.modalHeader}>{formHeader}</ModalHeader>
                    <ModalCloseButton></ModalCloseButton>
                    <ModalBody className={style.modalBody}>
                        <div className={style.modalBodyBlock}>
                            <div className={style.modalBlockRow}>
                                <span>Name:</span>
                                <span className={style.requiredField}>*</span>
                                <Input className={style.modalBlockRowInput}
                                    variant='filled'
                                    placeholder='Enter communication point name'
                                    defaultValue={name_state}
                                    onBlur={_name_onBlur}
                                    onFocus={_name_onFocus}
                                    disabled={communic_point_id!=""?true:false}
                                ></Input>
                            </div>
                            <div className={style.modalBlockRow}>
                                <span>Port:</span>
                                <span className={style.requiredField}>*</span>
                                <Input className={style.modalBlockRowInput}
                                    variant='filled'
                                    placeholder='Enter communication point port'
                                    defaultValue={port_state}
                                    onBlur={_port_onBlur}
                                ></Input>
                            </div>
                            <div className={style.modalBlockRow}>
                                <span>Descr:</span>
                                <Input className={style.modalBlockRowInput}
                                    variant='filled'
                                    placeholder='Enter description for this communication point'
                                    defaultValue={desc_state}
                                    onBlur={_desc_onBlur}
                                ></Input>
                            </div>
                            
                            <div className={style.saveCloseBtns}>
                                <EllipseBtnWithRightImg
                                    imgSrc={saveImgSrc}
                                    btnColor='#3EB62A'
                                    btnTxt='Save and close'
                                    txtFontWeight='600'
                                    title='Сохранить настройки'
                                    onClick={_on_save_and_close}
                                ></EllipseBtnWithRightImg>
                            </div>
                        </div>
                    </ModalBody>
                </ModalContent>
            </Modal> 
        </div>
    )
}