using System;
using System.Collections.Generic;
using System.Text;

namespace MaterialCodeSelectionPlatform.Domain
{
    public interface IIdEntity<TID>
    {
        TID Id { get; set; }
    }
}
