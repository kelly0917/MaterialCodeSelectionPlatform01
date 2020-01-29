using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaterialCodeSelectionPlatform.ManagerWeb
{
    public class ListResultModel
    {
        public ListResultModel()
        {
            this.msg = "";
            this.code = 0;
        }

        public int code { get; set; }

        public string msg { get; set; }

        public int count { get; set; }

        public object data { get; set; }

    }
}
