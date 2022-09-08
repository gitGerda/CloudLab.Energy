import { useDisclosure } from "@chakra-ui/react";
import React from "react";
import AlertDialogYesNo from "../../../Alert/AlertDialogYesNo/AlertDialogYesNo";
import EllipseBtnWithRightImg from "../EllipseBtnWithRightImg";

export default function EllipseBtnWithRightImgWithModalYesNo({ imgSrc, btnColor, btnTxt, title, txtFontStyle, txtFontWeight, onYesClick,header,body}) {
    const { isOpen, onOpen, onClose } = useDisclosure();

    function _yesClick() {
        onClose();
        onYesClick();
    }
    return (
        <div>
            <EllipseBtnWithRightImg imgSrc={imgSrc}
                btnColor={btnColor}
                btnTxt={btnTxt}
                title={title}
                txtFontStyle={txtFontStyle}
                txtFontWeight={txtFontWeight}
                onClick={onOpen}
            />
            <AlertDialogYesNo isOpenDisclosure={isOpen}
                onCloseDisclosure={onClose}
                onYesClick={_yesClick}
                header={header}
                body={body}
            />
        </div>
    )
}