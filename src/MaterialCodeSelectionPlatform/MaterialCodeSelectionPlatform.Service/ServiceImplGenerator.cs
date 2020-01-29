﻿// This file was automatically generated by the Dapper.SimpleCRUD T4 Template
// Do not make changes directly to this file - edit the template instead
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: ``
//     Provider:               `System.Data.SqlClient`
//     Connection String:      `data source=127.0.0.1;initial catalog=CommodityCodeSelectionPlatform;user id=devuser;password=devpwd`
//     Include Views:          `True`

using CommodityCodeSelectionPlatform.Data;
using CommodityCodeSelectionPlatform.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CommodityCodeSelectionPlatform.Service
{
    /// <summary>
    /// A Service Impl for Temp_ComponentType table.
    /// </summary>
	public partial class TempComponentTypeServiceImpl : EntityServiceBase<TempComponentType>, ITempComponentTypeService
	{
		 ITempComponentTypeDao _TempComponentTypeDao { get; set; }
		public TempComponentTypeServiceImpl(ITempComponentTypeDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_TempComponentTypeDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for Temp_CommodityCode table.
    /// </summary>
	public partial class TempCommodityCodeServiceImpl : EntityServiceBase<TempCommodityCode>, ITempCommodityCodeService
	{
		 ITempCommodityCodeDao _TempCommodityCodeDao { get; set; }
		public TempCommodityCodeServiceImpl(ITempCommodityCodeDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_TempCommodityCodeDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for Temp_PartNumber table.
    /// </summary>
	public partial class TempPartNumberServiceImpl : EntityServiceBase<TempPartNumber>, ITempPartNumberService
	{
		 ITempPartNumberDao _TempPartNumberDao { get; set; }
		public TempPartNumberServiceImpl(ITempPartNumberDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_TempPartNumberDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for Temp_PartNumberAttribute table.
    /// </summary>
	public partial class TempPartNumberAttributeServiceImpl : EntityServiceBase<TempPartNumberAttribute>, ITempPartNumberAttributeService
	{
		 ITempPartNumberAttributeDao _TempPartNumberAttributeDao { get; set; }
		public TempPartNumberAttributeServiceImpl(ITempPartNumberAttributeDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_TempPartNumberAttributeDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for Temp_Property table.
    /// </summary>
	public partial class TempPropertyServiceImpl : EntityServiceBase<TempProperty>, ITempPropertyService
	{
		 ITempPropertyDao _TempPropertyDao { get; set; }
		public TempPropertyServiceImpl(ITempPropertyDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_TempPropertyDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for Catalog table.
    /// </summary>
	public partial class CatalogServiceImpl : EntityServiceBase<Catalog>, ICatalogService
	{
		 ICatalogDao _CatalogDao { get; set; }
		public CatalogServiceImpl(ICatalogDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_CatalogDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for CommodityCode table.
    /// </summary>
	public partial class CommodityCodeServiceImpl : EntityServiceBase<CommodityCode>, ICommodityCodeService
	{
		 ICommodityCodeDao _CommodityCodeDao { get; set; }
		public CommodityCodeServiceImpl(ICommodityCodeDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_CommodityCodeDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for CommodityCodeAttribute table.
    /// </summary>
	public partial class CommodityCodeAttributeServiceImpl : EntityServiceBase<CommodityCodeAttribute>, ICommodityCodeAttributeService
	{
		 ICommodityCodeAttributeDao _CommodityCodeAttributeDao { get; set; }
		public CommodityCodeAttributeServiceImpl(ICommodityCodeAttributeDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_CommodityCodeAttributeDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for ComponentType table.
    /// </summary>
	public partial class ComponentTypeServiceImpl : EntityServiceBase<ComponentType>, IComponentTypeService
	{
		 IComponentTypeDao _ComponentTypeDao { get; set; }
		public ComponentTypeServiceImpl(IComponentTypeDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_ComponentTypeDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for Device table.
    /// </summary>
	public partial class DeviceServiceImpl : EntityServiceBase<Device>, IDeviceService
	{
		 IDeviceDao _DeviceDao { get; set; }
		public DeviceServiceImpl(IDeviceDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_DeviceDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for MaterialTakeOff table.
    /// </summary>
	public partial class MaterialTakeOffServiceImpl : EntityServiceBase<MaterialTakeOff>, IMaterialTakeOffService
	{
		 IMaterialTakeOffDao _MaterialTakeOffDao { get; set; }
		public MaterialTakeOffServiceImpl(IMaterialTakeOffDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_MaterialTakeOffDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for MaterialTakeOffDetail table.
    /// </summary>
	public partial class MaterialTakeOffDetailServiceImpl : EntityServiceBase<MaterialTakeOffDetail>, IMaterialTakeOffDetailService
	{
		 IMaterialTakeOffDetailDao _MaterialTakeOffDetailDao { get; set; }
		public MaterialTakeOffDetailServiceImpl(IMaterialTakeOffDetailDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_MaterialTakeOffDetailDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for PartNumber table.
    /// </summary>
	public partial class PartNumberServiceImpl : EntityServiceBase<PartNumber>, IPartNumberService
	{
		 IPartNumberDao _PartNumberDao { get; set; }
		public PartNumberServiceImpl(IPartNumberDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_PartNumberDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for Project table.
    /// </summary>
	public partial class ProjectServiceImpl : EntityServiceBase<Project>, IProjectService
	{
		 IProjectDao _ProjectDao { get; set; }
		public ProjectServiceImpl(IProjectDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_ProjectDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for ProjectCatalogMap table.
    /// </summary>
	public partial class ProjectCatalogMapServiceImpl : EntityServiceBase<ProjectCatalogMap>, IProjectCatalogMapService
	{
		 IProjectCatalogMapDao _ProjectCatalogMapDao { get; set; }
		public ProjectCatalogMapServiceImpl(IProjectCatalogMapDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_ProjectCatalogMapDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for SynchroDetail table.
    /// </summary>
	public partial class SynchroDetailServiceImpl : EntityServiceBase<SynchroDetail>, ISynchroDetailService
	{
		 ISynchroDetailDao _SynchroDetailDao { get; set; }
		public SynchroDetailServiceImpl(ISynchroDetailDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_SynchroDetailDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for SynchroRecord table.
    /// </summary>
	public partial class SynchroRecordServiceImpl : EntityServiceBase<SynchroRecord>, ISynchroRecordService
	{
		 ISynchroRecordDao _SynchroRecordDao { get; set; }
		public SynchroRecordServiceImpl(ISynchroRecordDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_SynchroRecordDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for User table.
    /// </summary>
	public partial class UserServiceImpl : EntityServiceBase<User>, IUserService
	{
		 IUserDao _UserDao { get; set; }
		public UserServiceImpl(IUserDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_UserDao = dao;
		}
	}

    /// <summary>
    /// A Service Impl for UserProjectMap table.
    /// </summary>
	public partial class UserProjectMapServiceImpl : EntityServiceBase<UserProjectMap>, IUserProjectMapService
	{
		 IUserProjectMapDao _UserProjectMapDao { get; set; }
		public UserProjectMapServiceImpl(IUserProjectMapDao dao,ILoggerFactory loggerFactory) : base(dao,loggerFactory) 
		{
			_UserProjectMapDao = dao;
		}
	}

}
