using System;
using System.Collections.Generic;
using System.Text;

namespace CommodityCodeSelectionPlatform.Domain
{
    public interface IIdEntity<TID>
    {
        TID Id { get; set; }
    }
}
