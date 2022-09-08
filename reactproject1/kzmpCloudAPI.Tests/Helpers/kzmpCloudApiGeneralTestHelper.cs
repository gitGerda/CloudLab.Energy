using kzmpCloudAPI.Database.EF_Core.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kzmpCloudAPI.Tests.Helpers
{
    internal static class kzmpCloudApiGeneralTestHelper
    {
        internal static Meter CreateDefaultMeter()
        {
            return new Meter()
            {
                Address = 8756789,
                CenterName = "test center name",
                CompanyName = "test company name",
                Inn = "test company inn",
                Inncenter = "test center inn",
                Interface = "",
                MeasuringchannelA = "",
                MeasuringchannelR = "",
                MeasuringpointName = "",
                Sim = "",
                TransformationRatio = "",
                Type = "",
                Xml80020code = ""
            };
        }
    }
}
