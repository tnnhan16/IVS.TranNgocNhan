using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVS.Components
{
 public static  class Utilities
    {

        public static bool isNullOrEmpty(this object obj)
        {
            try
            {
                if (obj == null)
                    return true;
                if (obj.ToString().Trim().Equals(""))
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
