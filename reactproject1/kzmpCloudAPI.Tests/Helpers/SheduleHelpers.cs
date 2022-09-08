using kzmpCloudAPI.Components.IndicationsReading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kzmpCloudAPI.Tests.Helpers
{
    public class SheduleHelpers
    {
        public static SheduleCreateUpdateModel _get_default_shedule()
        {
            var _shedule = new SheduleCreateUpdateModel()
            {
                communic_point_id = 0,
                periodicity = "every day",
                selected_meters_id = new List<int>() { 1, 2, 3, 4, 5 },
                shedule_name = "test",
                shedule_id = 1
            };
            return _shedule;
        }

    }
}
