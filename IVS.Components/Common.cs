﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVS.Components
{
    public class Common
    {
        public enum ReturnCode
        {
            Succeed = 0,
            UnSuccess = 1
        }
        public enum ActionType
        {
            Index = 0,
            Add = 1,
            Update = 2,
            Delete = 3
        }
        public enum ValueDefaul
        {
            ValueDefaul = 0
        }
    }
}
