﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryggLarm.Admin
{
    [Serializable]
    class FormatKey
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public FormatKey(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
