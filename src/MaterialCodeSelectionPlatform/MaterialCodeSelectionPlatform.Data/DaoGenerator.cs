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
using MaterialCodeSelectionPlatform.Domain.Entities;


namespace MaterialCodeSelectionPlatform.Data
{
    /// <summary>
    /// A Dao interface for Catalog table.
    /// </summary>
	public partial interface ICatalogDao : IEntityDao<Catalog>
	{
	}

    /// <summary>
    /// A Dao interface for CommodityCode table.
    /// </summary>
	public partial interface ICommodityCodeDao : IEntityDao<CommodityCode>
	{
	}

    /// <summary>
    /// A Dao interface for CommodityCodeAttribute table.
    /// </summary>
	public partial interface ICommodityCodeAttributeDao : IEntityDao<CommodityCodeAttribute>
	{
	}

    /// <summary>
    /// A Dao interface for ComponentType table.
    /// </summary>
	public partial interface IComponentTypeDao : IEntityDao<ComponentType>
	{
	}

    /// <summary>
    /// A Dao interface for Device table.
    /// </summary>
	public partial interface IDeviceDao : IEntityDao<Device>
	{
	}

    /// <summary>
    /// A Dao interface for MaterialTakeOff table.
    /// </summary>
	public partial interface IMaterialTakeOffDao : IEntityDao<MaterialTakeOff>
	{
	}

    /// <summary>
    /// A Dao interface for MaterialTakeOffDetail table.
    /// </summary>
	public partial interface IMaterialTakeOffDetailDao : IEntityDao<MaterialTakeOffDetail>
	{
	}

    /// <summary>
    /// A Dao interface for PartNumber table.
    /// </summary>
	public partial interface IPartNumberDao : IEntityDao<PartNumber>
	{
	}

    /// <summary>
    /// A Dao interface for Project table.
    /// </summary>
	public partial interface IProjectDao : IEntityDao<Project>
	{
	}

    /// <summary>
    /// A Dao interface for ProjectCatalogMap table.
    /// </summary>
	public partial interface IProjectCatalogMapDao : IEntityDao<ProjectCatalogMap>
	{
	}

    /// <summary>
    /// A Dao interface for SynchroDetail table.
    /// </summary>
	public partial interface ISynchroDetailDao : IEntityDao<SynchroDetail>
	{
	}

    /// <summary>
    /// A Dao interface for SynchroRecord table.
    /// </summary>
	public partial interface ISynchroRecordDao : IEntityDao<SynchroRecord>
	{
	}

    /// <summary>
    /// A Dao interface for Temp_CCPropertyValue table.
    /// </summary>
	public partial interface ITempCCPropertyValueDao : IEntityDao<TempCCPropertyValue>
	{
	}

    /// <summary>
    /// A Dao interface for Temp_CommodityCode table.
    /// </summary>
	public partial interface ITempCommodityCodeDao : IEntityDao<TempCommodityCode>
	{
	}

    /// <summary>
    /// A Dao interface for Temp_ComponentType table.
    /// </summary>
	public partial interface ITempComponentTypeDao : IEntityDao<TempComponentType>
	{
	}

    /// <summary>
    /// A Dao interface for Temp_PartNumber table.
    /// </summary>
	public partial interface ITempPartNumberDao : IEntityDao<TempPartNumber>
	{
	}

    /// <summary>
    /// A Dao interface for Temp_Property table.
    /// </summary>
	public partial interface ITempPropertyDao : IEntityDao<TempProperty>
	{
	}

    /// <summary>
    /// A Dao interface for Temp_PropertyValue table.
    /// </summary>
	public partial interface ITempPropertyValueDao : IEntityDao<TempPropertyValue>
	{
	}

    /// <summary>
    /// A Dao interface for User table.
    /// </summary>
	public partial interface IUserDao : IEntityDao<User>
	{
	}

    /// <summary>
    /// A Dao interface for UserProjectMap table.
    /// </summary>
	public partial interface IUserProjectMapDao : IEntityDao<UserProjectMap>
	{
	}

    /// <summary>
    /// A Dao interface for ChangeHistory table.
    /// </summary>
	public partial interface IChangeHistoryDao : IEntityDao<ChangeHistory>
	{
	}

}
