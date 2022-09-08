import React from "react";
import "../../../../../node_modules/bootstrap/dist/css/bootstrap.min.css"
import style from "./Table1.module.css"

export default function Table1({ headTable, bodyTable, descTable, tableClass, tableStyle }) {
  return (
    <div>
      <span className={style.desc} >{descTable}</span>
      <table id={style.mainTable} className={"table table-hover" + " " + tableClass} style={tableStyle}>
        <thead>
          {headTable}
        </thead>
        <tbody>
          {
            bodyTable != null ?
              bodyTable.map((tr_value) => tr_value)
              : <tr></tr>
          }
        </tbody>
      </table>
    </div>
  )
}

/*
 <thead>
    <tr>
      <th scope="col">#</th>
      <th scope="col">First</th>
      <th scope="col">Last</th>
      <th scope="col">Handle</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <th scope="row">1</th>
      <td>Mark</td>
      <td>Otto</td>
      <td>@mdo</td>
    </tr>
    <tr>
      <th scope="row">2</th>
      <td>Jacob</td>
      <td>Thornton</td>
      <td>@fat</td>
    </tr>
    <tr>
      <th scope="row">3</th>
      <td colspan="2">Larry the Bird</td>
      <td>@twitter</td>
    </tr>
  </tbody>
*/