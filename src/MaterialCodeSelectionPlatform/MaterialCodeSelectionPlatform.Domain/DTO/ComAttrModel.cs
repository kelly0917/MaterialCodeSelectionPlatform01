using System.Collections.Generic;

namespace MaterialCodeSelectionPlatform.Domain.DTO
{
    public class ComAttrModel
    {
        public string AttrbuteName { get; set; }

        public List<string> AttributeValues { get; set; }
    }


   

    public class AttributeModel
    {
        public string AttrName { get; set; }

        public string AttrValue { get; set; }
    }
}