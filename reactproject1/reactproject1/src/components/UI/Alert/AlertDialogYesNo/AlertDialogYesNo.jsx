import { AlertDialog, AlertDialogBody, AlertDialogCloseButton, AlertDialogContent, AlertDialogFooter, AlertDialogHeader, AlertDialogOverlay, Button } from "@chakra-ui/react";
import React from "react";

export default function AlertDialogYesNo({isOpenDisclosure,onCloseDisclosure, onYesClick,header,body}) {
    const cancelRef = React.useRef();
    return (
        <AlertDialog
            motionPreset="slideInBottom"
            leastDestructiveRef={cancelRef}
            onClose={onCloseDisclosure}    
            isOpen={isOpenDisclosure}
            isCentered
            closeOnEsc
        >
            <AlertDialogOverlay />
            <AlertDialogContent>
                <AlertDialogHeader>{header}</AlertDialogHeader>
                <AlertDialogCloseButton />

                <AlertDialogBody>{body}</AlertDialogBody>

                <AlertDialogFooter>
                    <Button colorScheme='telegram'  onClick={onYesClick}>Yes</Button>
                    <Button colorScheme='whatsapp' ml={3} ref={cancelRef} onClick={onCloseDisclosure}>No</Button>                 
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}