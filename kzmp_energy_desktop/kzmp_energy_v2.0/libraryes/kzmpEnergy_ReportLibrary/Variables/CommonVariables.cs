using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzmpEnergyReportLibrary.Variables
{
    public class CommonVariables
    {
        internal const string TEMPLATE_FOR_XML80020_DOC = "<?xml version=\"1.0\" encoding=\"windows-1251\"?> < message class=\"80020\" version=\"2\"></ message >";

        internal static readonly int[] TIME = new int[] {0,30,100,130,200,230,300,330,400,430,500,530,600,630,700,730,800,830,900,930,1000,1030,1100,1130,1200,1230,
        1300,1330,1400,1430,1500,1530,1600,1630,1700,1730,1800,1830,1900,1930,2000,2030,2100,2130,2200,2230,2300,2330};

        internal static readonly string[] CHECK_TIME = new string[]
        {
            "0000","0030","0100","0130","0200","0230","0300","0330","0400","0430","0500","0530","0600","0630","0700","0730","0800","0830",
            "0900","0930","1000","1030","1100","1130","1200","1230","1300","1330","1400","1430","1500","1530","1600","1630","1700","1730",
            "1800","1830","1900","1930","2000","2030","2100","2130","2200","2230","2300","2330","0000"
        };

        internal static readonly List<string> CHECK_MONTH = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

    }
}
