namespace MaterialCodeSelectionPlatform.Domain.DTO
{
    public class MaterialTakeOffDetailDto
    {
        public string ComponentTypeCode { get; set; }

        public string ComponentTypeDesc { get; set; }

        public string PartNumberCNLongDesc { get; set; }
        public string PartNumberCNSizeDesc { get; set; }

        public double DesignQty { get; set; }

        public double? Allowance { get; set; }

        public string AllowanceStr { get; set; }

        public int? RoundUpDigit { get; set; }

        public string MaterialTakeOffDetailId { get; set; }


        public double? RoundUp { get; set; }
    }
}