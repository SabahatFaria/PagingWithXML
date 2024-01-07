namespace MOCD
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RequestPayload
    {
        public string BenficiaryEID { get; set; }
        public string HoushouldEID { get; set; }
        public bool IaActive { get; set; }
        public string MonthBatch { get; set; }
        public int PageNumber { get; set; }
    }
}
