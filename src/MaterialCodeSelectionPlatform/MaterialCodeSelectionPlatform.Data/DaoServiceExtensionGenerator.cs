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
using Microsoft.Extensions.DependencyInjection;
using MaterialCodeSelectionPlatform.Domain.Entities;

namespace MaterialCodeSelectionPlatform.Data.Exentions.DependencyInjection
{
/// <summary>
    /// Dao实例注入扩展
    /// </summary>
    public static class DaoServiceExtension
    {
		/// <summary>
        /// 注入Dao实例
        /// </summary>
        /// <param name="services"></param>
        public static void AddScopedDao(this IServiceCollection services)
        {
			services.AddScoped<IProjectCatalogMapDao, ProjectCatalogMapDaoImpl>();
			services.AddScoped<ITempComponentTypeDao, TempComponentTypeDaoImpl>();
			services.AddScoped<ITempCommodityCodeDao, TempCommodityCodeDaoImpl>();
			services.AddScoped<ITempPropertyValueDao, TempPropertyValueDaoImpl>();
			services.AddScoped<ICommodityCodeAttributeDao, CommodityCodeAttributeDaoImpl>();
			services.AddScoped<IDeviceDao, DeviceDaoImpl>();
			services.AddScoped<IMaterialTakeOffDao, MaterialTakeOffDaoImpl>();
			services.AddScoped<IProjectDao, ProjectDaoImpl>();
			services.AddScoped<ISynchroDetailDao, SynchroDetailDaoImpl>();
			services.AddScoped<ISynchroRecordDao, SynchroRecordDaoImpl>();
			services.AddScoped<IUserDao, UserDaoImpl>();
			services.AddScoped<IUserProjectMapDao, UserProjectMapDaoImpl>();
			services.AddScoped<IMaterialTakeOffDetailDao, MaterialTakeOffDetailDaoImpl>();
			services.AddScoped<ICatalogDao, CatalogDaoImpl>();
			services.AddScoped<IComponentTypeDao, ComponentTypeDaoImpl>();
			services.AddScoped<ITempPartNumberDao, TempPartNumberDaoImpl>();
			services.AddScoped<ITempPropertyDao, TempPropertyDaoImpl>();
			services.AddScoped<ICommodityCodeDao, CommodityCodeDaoImpl>();
			services.AddScoped<IPartNumberDao, PartNumberDaoImpl>();
			services.AddScoped<ITempCCPropertyValueDao, TempCCPropertyValueDaoImpl>();
		}
	}
}
