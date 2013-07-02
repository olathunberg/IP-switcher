using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deucalion.IP_Switcher.Features.Location
{
    public class LocationExport
    {
        private string _Version;
        public string Version
        {
            get { return _Version; }
            set { _Version = value; }
        }

        private List<Location> _Locations = new List<Location>();
        public List<Location> Locations
        {
            get { return _Locations; }
            set { _Locations = value; }
        }
    }
}
