using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disp
{
    public class ActionLine
    {
        List<DumpAction> _actions;

        public ActionLine()
        {
            _actions = new List<DumpAction>();
        }

        public void Add(DumpAction da)
        {
            _actions.Add(da);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DumpAction da in _actions)
            {
                sb.AppendFormat("{0}\n", da.ToString());
            }
            return sb.ToString();
        }
    }
    public class DumpAction
    {
        DateTime _datetime;
        GeoLocation _location;
        string _key;
        public DumpAction(DateTime datetime, GeoLocation location, string key)
        {
            _datetime = datetime;
            _location = location;
            _key = key;
        }
        public override string ToString()
        {
            return string.Format("{0}: {1}", _datetime, _key);
        }
    }
}
