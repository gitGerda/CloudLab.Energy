import React from "react";
import Pagination_v1 from "../../../../../components/UI/Pagination/ver1/Pagination_v1";
import Spinner1 from "../../../../../components/UI/Spinner1/Spinner1";
import Table1 from "../../../../../components/UI/Tables/Table1/Table1";

export default function TableDataBase({ headTable, bodyTable, pagesCount, pagesForView,curPageNum,setCurPageNum,isLoading}) {
    return (
        <div>
            
            <Table1
                headTable={headTable}
                bodyTable={bodyTable}
                tableClass="table-bordered"
                tableStyle={{ fontSize: "0.8em" }}
            ></Table1>
            
            <Pagination_v1 pagesCount={pagesCount}
                pagesForView={pagesForView}
                curPageNum={curPageNum}
                setCurPageNum={setCurPageNum}
                isLoading = {isLoading}
            ></Pagination_v1>
        </div>
    )
}