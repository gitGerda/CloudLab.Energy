import { useToast } from "@chakra-ui/react";
import React, { useEffect, useState, useMemo } from "react";
import "../../../../../node_modules/bootstrap/dist/css/bootstrap.min.css"
import "./Pagination_v1.css"

export default function Pagination_v1({ pagesCount, pagesForView, curPageNum, setCurPageNum, isLoading }) {

    const toast = useToast();
    const [startPageInCurrentView, setStartPageInCurrentView] = useState(1);
    const [lastPageInCurrentView, setLastPageInCurrentView] = useState(1);
    const [pagesNumInCurView, setPagesNumInCurView] = useState([]);

    function pageClick(e) {
        if (isLoading == false) {
            for (let el of e.target.parentElement.parentElement.children) {
                el.classList.remove("active");
            }
            e.target.parentElement.classList.add("active");
            setCurPageNum(Number(e.target.innerHTML));
        }
        else {
            toast({
                title: "Ожидайте...",
                position: 'bottom',
                isClosable: true,
                status: 'info',
                duration: 3000
            });
        }
    }
    function nextPagesClick(e) {
        if (isLoading == false) {
            let lastOfNextView = lastPageInCurrentView + pagesForView;
            let nextPagesArr = [];
            if (lastOfNextView <= pagesCount) {
                for (var i = lastPageInCurrentView + 1; i <= lastOfNextView; i++) {
                    nextPagesArr.push(i);
                }
                setStartPageInCurrentView(lastPageInCurrentView + 1);
                setLastPageInCurrentView(lastOfNextView);
                setCurPageNum(lastPageInCurrentView + 1);
                setPagesNumInCurView(nextPagesArr);
            }
            else {
                for (var i = lastPageInCurrentView + 1; i <= pagesCount; i++) {
                    nextPagesArr.push(i);
                }
                if (nextPagesArr.length != 0) {
                    setStartPageInCurrentView(lastPageInCurrentView + 1);
                    setLastPageInCurrentView(pagesCount);
                    setCurPageNum(lastPageInCurrentView + 1);
                    setPagesNumInCurView(nextPagesArr);
                }
            }
        } else {
            toast({
                title: "Ожидайте...",
                position: 'bottom',
                isClosable: true,
                status: 'info',
                duration: 3000
            });
        }
    }
    function prevPagesClick(e) {
        if (isLoading == false) {
            let prevPagesArr = [];
            let lastOfPrevView = startPageInCurrentView - 1;
            if (lastOfPrevView > 0) {
                for (var i = startPageInCurrentView - pagesForView; i <= lastOfPrevView; i++) {
                    prevPagesArr.push(i);
                }
                setStartPageInCurrentView(startPageInCurrentView - pagesForView);
                setLastPageInCurrentView(lastOfPrevView);
                setCurPageNum(lastOfPrevView);
                setPagesNumInCurView(prevPagesArr);
            }
        } else {
            toast({
                title: "Ожидайте...",
                position: 'bottom',
                isClosable: true,
                status: 'info',
                duration: 3000
            });
        }
    }

    useMemo(() => {
        if (pagesCount == null || pagesCount == 0) {
            setStartPageInCurrentView(0);
            setLastPageInCurrentView(0);
            return;
        }
        if (pagesForView <= pagesCount) {
            setStartPageInCurrentView(1);
            setCurPageNum(1);
            setLastPageInCurrentView(pagesForView);
            var localPages = [];
            for (var i = 1; i <= pagesForView; i++) {
                localPages.push(i);
            }
            setPagesNumInCurView(localPages);
        }
        else {
            setStartPageInCurrentView(1);
            setCurPageNum(1);
            setLastPageInCurrentView(pagesCount);
            var localPages = [];
            for (var i = 1; i <= pagesCount; i++) {
                localPages.push(i);
            }
            setPagesNumInCurView(localPages);
        }
    }, [pagesCount, pagesForView])

    return (
        <nav className="pagination-outer" aria-label="Page navigation">
            <ul className="pagination">

                <li className="page-item">
                    <button className="page-link" aria-label="Previous" onClick={prevPagesClick}>
                        <span aria-hidden="true">«</span>
                    </button>
                </li>

                {
                    pagesNumInCurView.map((value) => {
                        if (value == curPageNum) {
                            return <li key={value} className="page-item active"> <button className="page-link" onClick={pageClick}>{value}</button></li>
                        }
                        else {
                            return <li key={value} className="page-item"> <button className="page-link" onClick={pageClick}>{value}</button></li>
                        }
                    })
                }

                <li className="page-item">
                    <button className="page-link" aria-label="Next" onClick={nextPagesClick}>
                        <span aria-hidden="true">»</span>
                    </button>
                </li>

            </ul>
        </nav>
    )
}