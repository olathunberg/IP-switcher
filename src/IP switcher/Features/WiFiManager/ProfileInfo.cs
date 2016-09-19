using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTech.IP_Switcher.Features.WiFiManager
{
    public class ProfileInfo
    {
        public ProfileInfo(string Header)
        {
            this.Header = Header;
        }

        public string Header { get; set; }

        public List<ProfileInfo> Children { get; set; }

        public bool HasChildren { get { return Children != null && Children.Count > 0; } }
    }
}
