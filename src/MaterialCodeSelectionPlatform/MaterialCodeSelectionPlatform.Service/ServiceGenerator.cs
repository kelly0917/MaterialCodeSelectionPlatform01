﻿// This file was automatically generated by the Dapper.SimpleCRUD T4 Template
// Do not make changes directly to this file - edit the template instead
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: ``
//     Provider:               `System.Data.SqlClient`
//     Connection String:      `data source=127.0.0.1;initial catalog=MaterialCodeSelectionPlatform;user id=devuser;password=devpwd`
//     Include Views:          `True`

using MaterialCodeSelectionPlatform.Domain.Entities;


namespace MaterialCodeSelectionPlatform.Service
{
    /// <summary>
    /// A Service interface for ProjectCatalogMap table.
    /// </summary>
	public partial interface IProjectCatalogMapService : IEntityService<ProjectCatalogMap>
	{
	}

    /// <summary>
    /// A Service interface for Temp_ComponentType table.
    /// </summary>
	public partial interface ITempComponentTypeService : IEntityService<TempComponentType>
	{
	}

    /// <summary>
    /// A Service interface for Temp_CommodityCode table.
    /// </summary>
	public partial interface ITempCommodityCodeService : IEntityService<TempCommodityCode>
	{
	}

    /// <summary>
    /// A Service interface for Temp_PropertyValue table.
    /// </summary>
	public partial interface ITempPropertyValueService : IEntityService<TempPropertyValue>
	{
	}

    /// <summary>
    /// A Service interface for CommodityCodeAttribute table.
    /// </summary>
	public partial interface ICommodityCodeAttributeService : IEntityService<CommodityCodeAttribute>
	{
	}

    /// <summary>
    /// A Service interface for Device table.
    /// </summary>
	public partial interface IDeviceService : IEntityService<Device>
	{
	}

    /// <summary>
    /// A Service interface for MaterialTakeOff table.
    /// </summary>
	public partial interface IMaterialTakeOffService : IEntityService<MaterialTakeOff>
	{
	}

    /// <summary>
    /// A Service interface for Project table.
    /// </summary>
	public partial interface IProjectService : IEntityService<Project>
	{
	}

    /// <summary>
    /// A Service interface for SynchroDetail table.
    /// </summary>
	public partial interface ISynchroDetailService : IEntityService<SynchroDetail>
	{
	}

    /// <summary>
    /// A Service interface for SynchroRecord table.
    /// </summary>
	public partial interface ISynchroRecordService : IEntityService<SynchroRecord>
	{
	}

    /// <summary>
    /// A Service interface for User table.
    /// </summary>
	public partial interface IUserService : IEntityService<User>
	{
	}

    /// <summary>
    /// A Service interface for UserProjectMap table.
    /// </summary>
	public partial interface IUserProjectMapService : IEntityService<UserProjectMap>
	{
	}

    /// <summary>
    /// A Service interface for MaterialTakeOffDetail table.
    /// </summary>
	public partial interface IMaterialTakeOffDetailService : IEntityService<MaterialTakeOffDetail>
	{
	}

    /// <summary>
    /// A Service interface for Catalog table.
    /// </summary>
	public partial interface ICatalogService : IEntityService<Catalog>
	{
	}

    /// <summary>
    /// A Service interface for ComponentType table.
    /// </summary>
	public partial interface IComponentTypeService : IEntityService<ComponentType>
	{
	}

    /// <summary>
    /// A Service interface for Temp_PartNumber table.
    /// </summary>
	public partial interface ITempPartNumberService : IEntityService<TempPartNumber>
	{
	}

    /// <summary>
    /// A Service interface for Temp_Property table.
    /// </summary>
	public partial interface ITempPropertyService : IEntityService<TempProperty>
	{
	}

    /// <summary>
    /// A Service interface for CommodityCode table.
    /// </summary>
	public partial interface ICommodityCodeService : IEntityService<CommodityCode>
	{
	}

    /// <summary>
    /// A Service interface for PartNumber table.
    /// </summary>
	public partial interface IPartNumberService : IEntityService<PartNumber>
	{
	}

    /// <summary>
    /// A Service interface for Temp_CCPropertyValue table.
    /// </summary>
	public partial interface ITempCCPropertyValueService : IEntityService<TempCCPropertyValue>
	{
	}

}
