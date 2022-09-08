import { background } from "@chakra-ui/react";
import React, { useEffect, useState } from "react";
import "../../../../node_modules/bootstrap/dist/css/bootstrap.min.css"
import style from "./CardsPages.module.css"
import CardsPagesNavs from "./decomp/CardsPagesNavs";

/* 
    Page - {navName, navPath, bodyElement}
    Pages - [Page, Page, Page ...]
*/

export default function CardsPages({ pages,activeNavName, setActiveNav }) {
    return (
        <div className={"card text-center " + style.mainDiv}>
            <div className="card-header">
                <ul className="nav nav-tabs card-header-tabs">
                    {
                        pages.map((value) => {
                            let navClassName = "nav-link";
                            if (value.navName == activeNavName) {
                                navClassName = navClassName + " active";
                            }
                            return <CardsPagesNavs
                                key={value.navName}
                                navClassName={navClassName}
                                navName={value.navName}
                                setNavActive={setActiveNav} 
                                
                                />
                        }
                        )
                    }
                </ul>
            </div>
            <div className="card-body">
                {
                    pages.map((value) => {
                        if (value.navName == activeNavName) {
                            return <div key={value.navName}>{value.bodyElement}</div>;
                        }
                    })
                }
            </div>

        </div>
    );
};