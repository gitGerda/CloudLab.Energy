import { styled } from "@chakra-ui/react";
import React from "react";
import style from "./AnimatedStars.module.css"

export default function AnimatedStars() {
    return (
        <div>
            <div className={style.desc}>Раздел находится на стадии разработки</div>
            <div className={style.stars}>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
                <div className={style.star}></div>
            </div>
        </div>
    )
}