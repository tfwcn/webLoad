using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebLoad
{
    [DataContract]
    public class MData
    {
        [DataMember(Order = 0, IsRequired = true)]
        public string href { get; set; }

        [DataMember(Order = 1)]
        public string title { get; set; }

        [DataMember(Order = 2)]
        public string content { get; set; }
    }
}
