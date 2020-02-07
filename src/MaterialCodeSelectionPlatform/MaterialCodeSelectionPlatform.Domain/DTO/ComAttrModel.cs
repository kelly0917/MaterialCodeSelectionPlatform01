using System.Collections.Generic;

namespace MaterialCodeSelectionPlatform.Domain.DTO
{
    public class ComAttrModel
    {
        public string AttrbuteName { get; set; }

        public List<AttributeValueModel> AttributeValueModels { get; set; }
    }


    public class AttributeValueModel
    {
        public string Id { get; set; }

        public string Value { get; set; }
    }


    public class AttributeModel
    {
        public string AttrName { get; set; }

        public string AttrValue { get; set; }
    }
}