using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using KzmpEnergyReportLibrary.Variables;
using KzmpEnergyReportLibrary.Models;

namespace KzmpEnergyReportLibrary.Actions
{
    public class DocumentXml80020
    {
        public void CreateDocXml80020(string companyInn, string companyName, string senderInn, string senderName, DateTime currentDate, string pathToSource, string timestamp_txt, string current_msg_number, List<MeterInfo> MeterInfoList, ref double GenFloatSum)
        {

            string currentDateStr = "";
            string pathToDoc = ReportFiles.GetPathToSaveDoc(companyInn, currentDate, current_msg_number, pathToSource, out currentDateStr);

            XmlDocument xDoc = new XmlDocument();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            XmlDeclaration xmldecl = xDoc.CreateXmlDeclaration("1.0", Encoding.GetEncoding("windows-1251").WebName, null);

            XmlElement? xRoot = xDoc.CreateElement("message");

            xRoot.SetAttribute("class", "80020");
            xRoot.SetAttribute("version", "2");
            xRoot.SetAttribute("number", current_msg_number);
            xDoc.AppendChild(xRoot);

            xDoc.InsertBefore(xmldecl, xRoot);

            #region <datetime></datetime><sender></sender>
            //<datetime></datetime>
            XmlElement datetimeElem = xDoc.CreateElement("datetime");
            XmlElement timestampElem = xDoc.CreateElement("timestamp");
            XmlElement daylightsavingtimeElem = xDoc.CreateElement("daylightsavingtime");
            XmlElement dayElem = xDoc.CreateElement("day");

            XmlText timestampText = xDoc.CreateTextNode(timestamp_txt);
            XmlText daylightsavingtimeText = xDoc.CreateTextNode("1");
            XmlText dayText = xDoc.CreateTextNode(currentDateStr);

            dayElem.AppendChild(dayText);
            daylightsavingtimeElem.AppendChild(daylightsavingtimeText);
            timestampElem.AppendChild(timestampText);

            datetimeElem.AppendChild(timestampElem);
            datetimeElem.AppendChild(daylightsavingtimeElem);
            datetimeElem.AppendChild(dayElem);

            xRoot?.AppendChild(datetimeElem);

            //<sender></sender>
            XmlElement SenderElem = xDoc.CreateElement("Sender");
            XmlElement SenderInnElem = xDoc.CreateElement("inn");
            XmlElement SenderNameElem = xDoc.CreateElement("name");

            XmlText SenderInnText = xDoc.CreateTextNode(senderInn);
            XmlText SenderNameText = xDoc.CreateTextNode(senderName);

            SenderInnElem.AppendChild(SenderInnText);
            SenderNameElem.AppendChild(SenderNameText);

            SenderElem.AppendChild(SenderInnElem);
            SenderElem.AppendChild(SenderNameElem);

            xRoot?.AppendChild(SenderElem);
            #endregion

            #region <area></area>
            XmlElement areaElem = xDoc.CreateElement("area");
            XmlElement innElem = xDoc.CreateElement("inn");
            XmlElement nameElem = xDoc.CreateElement("Name");

            XmlText innText = xDoc.CreateTextNode(companyInn);
            XmlText nameText = xDoc.CreateTextNode(companyName);

            innElem.AppendChild(innText);
            nameElem.AppendChild(nameText);

            areaElem.AppendChild(innElem);
            areaElem.AppendChild(nameElem);
            #endregion

            foreach (MeterInfo _meterInfo in MeterInfoList)
            {
                XmlElement measuringpointElem = xDoc.CreateElement("measuringpoint");

                XmlAttribute pointCodeAttr = xDoc.CreateAttribute("code");
                XmlAttribute pointNameAttr = xDoc.CreateAttribute("name");

                XmlText pointCodeAttrText = xDoc.CreateTextNode(_meterInfo.xml80020code);
                XmlText pointNameAttrText = xDoc.CreateTextNode(_meterInfo.MeausuringPointNameMI);

                pointCodeAttr.AppendChild(pointCodeAttrText);
                pointNameAttr.AppendChild(pointNameAttrText);

                measuringpointElem.Attributes.Append(pointCodeAttr);
                measuringpointElem.Attributes.Append(pointNameAttr);

                for (int j = 0; j < 2; j++)
                {
                    XmlElement measuringchannelElem = xDoc.CreateElement("measuringchannel");
                    XmlAttribute channelCodeAttr = xDoc.CreateAttribute("code");
                    XmlAttribute descAttr = xDoc.CreateAttribute("desc");

                    if (j == 0)
                    {
                        XmlText channelCodeAttrText = xDoc.CreateTextNode("01");
                        XmlText descAttrText = xDoc.CreateTextNode(_meterInfo.MeasuringChannelAnameMI);

                        channelCodeAttr.AppendChild(channelCodeAttrText);
                        descAttr.AppendChild(descAttrText);

                        measuringchannelElem.Attributes.Append(channelCodeAttr);
                        measuringchannelElem.Attributes.Append(descAttr);

                        int counter = 0;
                        foreach (PowerProfileTableRec _powerProfileTableRec in _meterInfo.PowerProfileTableRecList)
                        {

                            int K = Convert.ToInt32(_meterInfo.transformation_ratio) / 2;
                            double value_d = (_powerProfileTableRec.Pplus ?? 0) * Convert.ToDouble(K);
                            string periodValue = Convert.ToString(value_d);
                            string bdTimeStr = Convert.ToString(_powerProfileTableRec.Time) ?? "";

                            GenFloatSum = GenFloatSum + value_d;
                            CreatePeriodElement(periodValue, bdTimeStr, ref counter, ref xDoc, ref measuringchannelElem);

                        }

                        int K2 = Convert.ToInt32(_meterInfo.transformation_ratio) / 2;
                        double value_d2 = (_meterInfo.PowerProfileTableRecList_2330_0000.Pplus ?? 0) * Convert.ToDouble(K2);
                        GenFloatSum = GenFloatSum + value_d2;
                        string periodValue2330 = Convert.ToString(value_d2);
                        CreatePeriodElement2330_0000(periodValue2330, ref xDoc, ref measuringchannelElem);

                    }
                    else
                    {
                        XmlText channelCodeAttrText = xDoc.CreateTextNode("03");
                        XmlText descAttrText = xDoc.CreateTextNode(_meterInfo.MeasuringChannelRnameMI);

                        channelCodeAttr.AppendChild(channelCodeAttrText);
                        descAttr.AppendChild(descAttrText);

                        measuringchannelElem.Attributes.Append(channelCodeAttr);
                        measuringchannelElem.Attributes.Append(descAttr);

                        int counter = 0;
                        foreach (PowerProfileTableRec _powerProfileTableRec in _meterInfo.PowerProfileTableRecList)
                        {
                            int K = Convert.ToInt32(_meterInfo.transformation_ratio) / 2;
                            double value_d = (_powerProfileTableRec.Qplus ?? 0) * Convert.ToDouble(K);
                            string periodValue = Convert.ToString(value_d);
                            string bdTimeStr = Convert.ToString(_powerProfileTableRec.Time) ?? "";
                            CreatePeriodElement(periodValue, bdTimeStr, ref counter, ref xDoc, ref measuringchannelElem);
                        }

                        int K2 = Convert.ToInt32(_meterInfo.transformation_ratio) / 2;
                        double value_d2 = (_meterInfo.PowerProfileTableRecList_2330_0000.Qplus ?? 0) * Convert.ToDouble(K2);
                        string periodValue2330Qplus = Convert.ToString(value_d2);
                        CreatePeriodElement2330_0000(periodValue2330Qplus, ref xDoc, ref measuringchannelElem);
                    }
                    measuringpointElem.AppendChild(measuringchannelElem);
                }
                areaElem.AppendChild(measuringpointElem);
            }
            xRoot?.AppendChild(areaElem);

            //xDoc.Save(pathToDoc);
            using (FileStream stream = File.Create(pathToDoc))
            {
                xDoc.Save(stream);
            }
        }

        public int CalcMsgNumber(DateTime reportStartDate, int msgNumberFromDb, DateTime startDateForMsgNumberFromDb)
        {
            string beginDate = reportStartDate.ToShortDateString();
            var beginDateVar = DateTime.Parse(beginDate);

            string endDate = startDateForMsgNumberFromDb.ToShortDateString();
            var endDateVar = DateTime.Parse(endDate);

            var count = (beginDateVar - endDateVar).Duration();
            int msgNumber = msgNumberFromDb + Convert.ToInt32(count.Days);

            return msgNumber;
        }

        void CreatePeriodElement2330_0000(string periodValue, ref XmlDocument xDoc, ref XmlElement parentElement)
        {
            XmlElement periodElem2 = xDoc.CreateElement("period");
            XmlAttribute startAttr2 = xDoc.CreateAttribute("start");
            XmlAttribute endAttr2 = xDoc.CreateAttribute("end");
            XmlElement valueElem2 = xDoc.CreateElement("value");

            XmlText startAttrText2 = xDoc.CreateTextNode("2330");
            XmlText endAttrText2 = xDoc.CreateTextNode("0000");

            startAttr2.AppendChild(startAttrText2);
            endAttr2.AppendChild(endAttrText2);

            XmlText valueElemtext2 = xDoc.CreateTextNode(periodValue);

            periodElem2.Attributes.Append(startAttr2);
            periodElem2.Attributes.Append(endAttr2);

            valueElem2.AppendChild(valueElemtext2);

            periodElem2.AppendChild(valueElem2);
            parentElement.AppendChild(periodElem2);
        }
        void CreatePeriodElement(string periodValue, string bdTimeStr, ref int counter, ref XmlDocument xDoc, ref XmlElement parentElement)
        {
            XmlElement periodElem = xDoc.CreateElement("period");
            XmlAttribute startAttr = xDoc.CreateAttribute("start");
            XmlAttribute endAttr = xDoc.CreateAttribute("end");
            XmlElement valueElem = xDoc.CreateElement("value");

            if (bdTimeStr.Contains(":"))
            {
                bdTimeStr = bdTimeStr.Replace(":", "");
            }
            bdTimeStr = bdTimeStr.Substring(0, 4);
            int bdTime = Convert.ToInt32(bdTimeStr);

            for (int i = 0; i < CommonVariables.TIME.Length; i++)
            {
                if (bdTime > CommonVariables.TIME[i] && bdTime <= CommonVariables.TIME[i + 1])
                {
                    bdTime = CommonVariables.TIME[i + 1];
                }
            }

            bdTime = bdTime - 5;
            //вычисление атрибута start
            for (int m = 0; m < CommonVariables.TIME.Length; m++)
            {
                if (bdTime >= CommonVariables.TIME[m] && bdTime < CommonVariables.TIME[m + 1])
                {
                    bdTime = CommonVariables.TIME[m];
                    bdTimeStr = Convert.ToString(CommonVariables.CHECK_TIME[m + 1]);
                }
            }
            string startTime = Convert.ToString(bdTime);
            int b = 4 - startTime.Length;
            if (b == 1)
                startTime = startTime.Insert(0, "0");
            if (b == 2)
                startTime = startTime.Insert(0, "00");
            if (b == 3)
                startTime = startTime.Insert(0, "000");

            bool flagLocal = true;
            while (flagLocal)
            {
                if (startTime != CommonVariables.CHECK_TIME[counter] && counter < CommonVariables.CHECK_TIME.Length)
                {
                    XmlElement periodElem3 = xDoc.CreateElement("period");
                    XmlAttribute startAttr3 = xDoc.CreateAttribute("start");
                    XmlAttribute endAttr3 = xDoc.CreateAttribute("end");
                    XmlElement valueElem3 = xDoc.CreateElement("value");

                    XmlText startAttrText3 = xDoc.CreateTextNode(CommonVariables.CHECK_TIME[counter]);
                    XmlText endAttrText3 = xDoc.CreateTextNode(CommonVariables.CHECK_TIME[counter + 1]);

                    startAttr3.AppendChild(startAttrText3);
                    endAttr3.AppendChild(endAttrText3);

                    XmlText valueElemtext3 = xDoc.CreateTextNode("0");

                    periodElem3.Attributes.Append(startAttr3);
                    periodElem3.Attributes.Append(endAttr3);

                    valueElem3.AppendChild(valueElemtext3);
                    periodElem3.AppendChild(valueElem3);

                    parentElement.AppendChild(periodElem3);
                    counter++;
                }
                else
                {
                    flagLocal = false;
                }
            }
            XmlText startAttrText = xDoc.CreateTextNode(startTime);
            XmlText endAttrText = xDoc.CreateTextNode(bdTimeStr);

            startAttr.AppendChild(startAttrText);
            endAttr.AppendChild(endAttrText);

            XmlText valueElemtext = xDoc.CreateTextNode(periodValue);

            periodElem.Attributes.Append(startAttr);
            periodElem.Attributes.Append(endAttr);

            valueElem.AppendChild(valueElemtext);

            periodElem.AppendChild(valueElem);
            parentElement.AppendChild(periodElem);

            counter++;
        }

        void WriteTemplateToDoc(string pathToDoc)
        {
            using (FileStream stream = File.Create(pathToDoc))
            {
                byte[] template = new UTF8Encoding(true).GetBytes(CommonVariables.TEMPLATE_FOR_XML80020_DOC);
                stream.Write(template, 0, template.Length);
            }
        }

    }
}
