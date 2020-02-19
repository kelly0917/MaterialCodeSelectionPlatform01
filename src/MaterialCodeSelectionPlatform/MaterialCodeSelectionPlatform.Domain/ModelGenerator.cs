﻿// This file was automatically generated by the Dapper.SimpleCRUD T4 Template
// Do not make changes directly to this file - edit the template instead
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: ``
//     Provider:               `System.Data.SqlClient`
//     Connection String:      `data source=127.0.0.1;initial catalog=MaterialCodeSelectionPlatform;user id=devuser;password=devpwd`
//     Include Views:          `True`

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using SqlSugar;

namespace MaterialCodeSelectionPlatform.Domain.Entities
{
    /// <summary>
    /// A class which represents the ProjectCatalogMap table.
    /// </summary>
	[SugarTable("ProjectCatalogMap")]
	public partial class ProjectCatalogMap : IDataEntity
	{
		
		public virtual string ProjectId { get; set; }
		
		public virtual string CatalogId { get; set; }
								}

    /// <summary>
    /// A class which represents the Temp_ComponentType table.
    /// </summary>
	[SugarTable("Temp_ComponentType")]
	public partial class TempComponentType : IDataEntity
	{
	
		public virtual int? CLASS_NO { get; set; }
		
		[Key]	
		public virtual string CLASS_ID { get; set; }
		
		public virtual int? CATALOG_NO { get; set; }
		
		public virtual int? SEQ_NO { get; set; }
		
		public virtual string DESCR { get; set; }
		
		public virtual int? PARENT_CLASS_NO { get; set; }
		
		public virtual string CAN_INSTANTIATE { get; set; }
		
		public virtual string UNIT_ID { get; set; }
		
		public virtual string DRAW_DISCIPLINE_NO { get; set; }
		
		public virtual int? APPROVAL_STATUS_NO { get; set; }
		
		public virtual string CATALOG_ID { get; set; }
		}

    /// <summary>
    /// A class which represents the Temp_CommodityCode table.
    /// </summary>
	[SugarTable("Temp_CommodityCode")]
	public partial class TempCommodityCode : IDataEntity
	{
	
		public virtual string COMMODITY_NO { get; set; }
		
		[Key]	
		public virtual string COMMODITY_ID { get; set; }
		
		public virtual string COMMODITY_CLASS_NO { get; set; }
		
		public virtual string CATALOG_NO { get; set; }
		}

    /// <summary>
    /// A class which represents the Temp_PropertyValue table.
    /// </summary>
	[SugarTable("Temp_PropertyValue")]
	public partial class TempPropertyValue : IDataEntity
	{
	
		public virtual int? CATALOG_NO { get; set; }
		
		[Key]	
		public virtual int? INSTANCE_NO { get; set; }
		
		public virtual int? CLASS_NO { get; set; }
		
		public virtual int? ENTITY_PROPERTY_NO { get; set; }
		
		public virtual string PROPERTY_VALUE { get; set; }
		}

    /// <summary>
    /// A class which represents the CommodityCodeAttribute table.
    /// </summary>
	[SugarTable("CommodityCodeAttribute")]
	public partial class CommodityCodeAttribute : IDataEntity
	{
		
		public virtual string ComponentTypeId { get; set; }
		
		public virtual string CommodityCodeId { get; set; }
		
		public virtual int? SeqNo { get; set; }
		
		public virtual string AttributeName { get; set; }
		
		public virtual string AttributeValue { get; set; }
								}

    /// <summary>
    /// A class which represents the Device table.
    /// </summary>
	[SugarTable("Device")]
	public partial class Device : IDataEntity
	{
		
		public virtual string Name { get; set; }
		
		public virtual string Code { get; set; }
		
		public virtual string Remark { get; set; }
		
		public virtual string ProjectId { get; set; }
								}

    /// <summary>
    /// A class which represents the MaterialTakeOff table.
    /// </summary>
	[SugarTable("MaterialTakeOff")]
	public partial class MaterialTakeOff : IDataEntity
	{
		
		public virtual int? CheckStatus { get; set; }
		
		[Timestamp]
		public virtual int? Version { get; set; }
		
		public virtual string ProjectId { get; set; }
		
		public virtual string DeviceId { get; set; }
								}

    /// <summary>
    /// A class which represents the Project table.
    /// </summary>
	[SugarTable("Project")]
	public partial class Project : IDataEntity
	{
		
		public virtual string Name { get; set; }
		
		public virtual string Code { get; set; }
								}

    /// <summary>
    /// A class which represents the SynchroDetail table.
    /// </summary>
	[SugarTable("SynchroDetail")]
	public partial class SynchroDetail : IDataEntity
	{
		
		public virtual string SynchroRecordId { get; set; }
		
		public virtual int? Type { get; set; }
		
		public virtual int? OperateType { get; set; }
		
		public virtual string PurchasingCodeId { get; set; }
		
		public virtual string OldValue { get; set; }
		
		public virtual string NewValue { get; set; }
								}

    /// <summary>
    /// A class which represents the SynchroRecord table.
    /// </summary>
	[SugarTable("SynchroRecord")]
	public partial class SynchroRecord : IDataEntity
	{
		
		public virtual DateTime Date { get; set; }
		
		public virtual string UserId { get; set; }
		
		public virtual string Result { get; set; }
								}

    /// <summary>
    /// A class which represents the User table.
    /// </summary>
	[SugarTable("User")]
	public partial class User : IDataEntity
	{
		
		public virtual string Name { get; set; }
		
		public virtual string LoginName { get; set; }
		
		public virtual int? Role { get; set; }
		
		public virtual string Remark { get; set; }
		
		public virtual string Password { get; set; }
		
		public virtual string Ico { get; set; }
		
		public virtual int? Sex { get; set; }
								
		public virtual string DomainUserName { get; set; }
		
		public virtual string Discipline { get; set; }
		
		public virtual string Mobilephone { get; set; }
		
		public virtual string Telephone { get; set; }
		
		public virtual string Email { get; set; }
		}

    /// <summary>
    /// A class which represents the UserProjectMap table.
    /// </summary>
	[SugarTable("UserProjectMap")]
	public partial class UserProjectMap : IDataEntity
	{
		
		public virtual string UserId { get; set; }
		
		public virtual string ProjectId { get; set; }
								}

    /// <summary>
    /// A class which represents the MaterialTakeOffDetail table.
    /// </summary>
	[SugarTable("MaterialTakeOffDetail")]
	public partial class MaterialTakeOffDetail : IDataEntity
	{
		
		public virtual string MaterialTakeOffId { get; set; }
		
		public virtual string CommodityCodeId { get; set; }
		
		public virtual string PartNumberId { get; set; }
		
		public virtual string CN_CommodityShortDesc { get; set; }
		
		public virtual string EN_CommodityShortDesc { get; set; }
		
		public virtual string RU_CommodityShortDesc { get; set; }
		
		public virtual string CN_CommodityLongDesc { get; set; }
		
		public virtual string EN_CommodityLongDesc { get; set; }
		
		public virtual string RU_CommodityLongDesc { get; set; }
		
		public virtual string CN_PartNumberShortDesc { get; set; }
		
		public virtual string EN_PartNumberShortDesc { get; set; }
		
		public virtual string RU_PartNumberShortDesc { get; set; }
		
		public virtual string CN_PartNumberLongDesc { get; set; }
		
		public virtual string EN_PartNumberLongDesc { get; set; }
		
		public virtual string RU_PartNumberLongDesc { get; set; }
		
		public virtual string CN_SizeDesc { get; set; }
		
		public virtual string EN_SizeDesc { get; set; }
		
		public virtual string RU_SizeDesc { get; set; }
		
		public virtual string ProjectId { get; set; }
		
		public virtual string DeviceId { get; set; }
		
		public virtual double DesignQty { get; set; }
								}

    /// <summary>
    /// A class which represents the Catalog table.
    /// </summary>
	[SugarTable("Catalog")]
	public partial class Catalog : IDataEntity
	{
		
		public virtual string Code { get; set; }
		
		public virtual string Name { get; set; }
		
		public virtual string ConnectionString { get; set; }
		
		public virtual string CN_COMM_DESC_SHORT { get; set; }
		
		public virtual string EN_COMM_DESC_SHORT { get; set; }
		
		public virtual string RU_COMM_DESC_SHORT { get; set; }
		
		public virtual string CN_COMM_DESC_LONG { get; set; }
		
		public virtual string EN_COMM_DESC_LONG { get; set; }
		
		public virtual string RU_COMM_DESC_LONG { get; set; }
		
		public virtual string CN_PART_DESC_SHORT { get; set; }
		
		public virtual string EN_PART_DESC_SHORT { get; set; }
		
		public virtual string RU_PART_DESC_SHORT { get; set; }
		
		public virtual string CN_PART_DESC_LONG { get; set; }
		
		public virtual string EN_PART_DESC_LONG { get; set; }
		
		public virtual string RU_PART_DESC_LONG { get; set; }
		
		public virtual string CN_SIZE_DESC { get; set; }
		
		public virtual string EN_SIZE_DESC { get; set; }
		
		public virtual string RU_SIZE_DESC { get; set; }
		
		public virtual string COMM_REPRESENT_TYPE { get; set; }
								}

    /// <summary>
    /// A class which represents the ComponentType table.
    /// </summary>
	[SugarTable("ComponentType")]
	public partial class ComponentType : IDataEntity
	{
		
		public virtual string CatalogId { get; set; }
		
		public virtual string CatalogName { get; set; }
		
		public virtual string ParentId { get; set; }
		
		public virtual string Code { get; set; }
		
		public virtual string Desc { get; set; }
		
		public virtual int? CanInstantiate { get; set; }
		
		public virtual string Unit { get; set; }
		
		public virtual string Discipline { get; set; }
								}

    /// <summary>
    /// A class which represents the Temp_PartNumber table.
    /// </summary>
	[SugarTable("Temp_PartNumber")]
	public partial class TempPartNumber : IDataEntity
	{
	
		public virtual int? PART_NO { get; set; }
		
		[Key]	
		public virtual string PART_ID { get; set; }
		
		public virtual int? CATALOG_NO { get; set; }
		
		public virtual int? COMMODITY_NO { get; set; }
		
		public virtual int? SIZE_REF_NO { get; set; }
		}

    /// <summary>
    /// A class which represents the Temp_Property table.
    /// </summary>
	[SugarTable("Temp_Property")]
	public partial class TempProperty : IDataEntity
	{
	
		public virtual int? ENTITY_PROPERTY_NO { get; set; }
		
		[Key]	
		public virtual string ENTITY_PROPERTY_ID { get; set; }
		
		public virtual int? CATALOG_NO { get; set; }
		}

    /// <summary>
    /// A class which represents the CommodityCode table.
    /// </summary>
	[SugarTable("CommodityCode")]
	public partial class CommodityCode : IDataEntity
	{
		
		public virtual string ComponentTypeId { get; set; }
		
		public virtual string Code { get; set; }
		
		public virtual string CN_ShortDesc { get; set; }
		
		public virtual string EN_ShortDesc { get; set; }
		
		public virtual string RU_ShortDesc { get; set; }
		
		public virtual string CN_LongDesc { get; set; }
		
		public virtual string EN_LongDesc { get; set; }
		
		public virtual string RU_LongDesc { get; set; }
		
		public virtual long? Hits { get; set; }
								}

    /// <summary>
    /// A class which represents the PartNumber table.
    /// </summary>
	[SugarTable("PartNumber")]
	public partial class PartNumber : IDataEntity
	{
		
		public virtual string Code { get; set; }
		
		public virtual string ComponentTypeId { get; set; }
		
		public virtual string CommodityCodeId { get; set; }
		
		public virtual string CN_ShortDesc { get; set; }
		
		public virtual string EN_ShortDesc { get; set; }
		
		public virtual string RU_ShortDesc { get; set; }
		
		public virtual string CN_LongDesc { get; set; }
		
		public virtual string EN_LongDesc { get; set; }
		
		public virtual string RU_LongDesc { get; set; }
		
		public virtual string CN_SizeDesc { get; set; }
		
		public virtual string EN_SizeDesc { get; set; }
		
		public virtual string RU_SizeDesc { get; set; }
								}

    /// <summary>
    /// A class which represents the Temp_CCPropertyValue table.
    /// </summary>
	[SugarTable("Temp_CCPropertyValue")]
	public partial class TempCCPropertyValue : IDataEntity
	{
	
		public virtual string descr { get; set; }
		
		public virtual string VALUE_TEXT { get; set; }
		
		[Key]	
		public virtual int? instance_no { get; set; }
		
		public virtual int? catalog_no { get; set; }
		
		public virtual int? SEQ_NO { get; set; }
		}

}
