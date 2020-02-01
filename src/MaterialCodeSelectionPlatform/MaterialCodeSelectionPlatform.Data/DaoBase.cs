using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Logging;
using SqlSugar;
using MaterialCodeSelectionPlatform.Data;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Utilities;


namespace MaterialCodeSelectionPlatform.Data
{
    public partial class EntityDaoBase<TEntity> :  IEntityDao<TEntity>, ILogable where TEntity :IDataEntity,new()
    {

        private ILog log = LogHelper.GetLogger<Object>();
        
        protected string EmptyGuid = Guid.Empty.ToString();
        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作
        public SimpleClient<TEntity> DbContext { get { return new SimpleClient<TEntity>(Db); } }//用来处理T表的常用操作
        public EntityDaoBase(ILoggerFactory loggerFactory)
        {
           
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.DBConnectionString,
                DbType = Config.DBProviderName.Equals("System.Data.SqlClient", StringComparison.OrdinalIgnoreCase) ? DbType.SqlServer : DbType.MySql,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样我就不多解释了
            });
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                log.Debug("sql=" + sql + @"\r\n参数为：" +
                                Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName,
                                    it => it.Value)));
            };
            Db.Aop.OnLogExecuted = (sql, pars) =>
                {
                    log.Debug("执行花费时间：" + Db.Ado.SqlExecutionTime.TotalMilliseconds + "mm");
                    //
                    if (Db.Ado.SqlExecutionTime.TotalMilliseconds > 5000)
                    {
                        log.Error("执行花费时间>5秒，SQL="+ sql);
                    }
                };
        }


        /// <summary>
        /// 获取总记录数
        /// </summary>`
        /// <returns>返回总记录数</returns>
        public virtual async Task<long> CountAsync()
        {
            return await DbContext.AsQueryable().CountAsync();
        }

        /// <summary>
        /// 根据Id删除实体
        /// </summary>
        /// <param name="id">要删除实体的Id</param>
        /// <returns>受影响的记录数，0表示没有符合条件的实体</returns>
        public virtual async Task<bool> DeleteByIdAsync(string id)
        {
            var entity = DbContext.AsQueryable().InSingle(id);
            return await Task.FromResult(DbContext.Delete(entity));
        }
        
        /// <summary>
        /// 根据ID获取实体.
        /// </summary>
        /// <param name="id">实体Id</param>
        /// <returns>返回实体对象或null（如果不存在)</returns>
        public virtual async Task<TEntity> GetAsync(string id)
        {
            return await Task.FromResult(DbContext.AsQueryable().InSingle(id));
        }

        /// <summary>
        /// 获取所有实体对象集合
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetListAsync()
        {
            return await Task.FromResult(DbContext.GetList().ToList());
        }

        /// <summary>
        /// 获取实体对象集合页
        /// </summary>
        /// <param name="page">分页信息</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetListPagedAsync(PageModel page)
        {
            var pageCount = 0;
            //常规写法
            var data = DbContext.AsQueryable().OrderBy(it => it.Id).ToPageList(page.PageIndex, page.PageSize,ref pageCount);

            page.PageCount = pageCount;
            return await Task.FromResult(data);


            ////针对rowNumber分页的优化写法，该写法可以达到分页最高性能，非对性能要求过高的可以不用这么写
            //var Tempdb = DbContext.AsQueryable();
            //int count = Tempdb.Count();
            //var Skip = (page.PageIndex - 1) * page.PageCount;
            //var Take = page.PageCount;
            //if (page.PageIndex * page.PageCount > page.PageCount / 2)//页码大于一半用倒序
            //{
            //    Tempdb.OrderBy(x => x.ID, OrderByType.Desc);
            //    var Mod = P.Count % R.PageCount;
            //    var Page = (int)Math.Ceiling((Decimal)P.Count / R.PageCount);
            //    if (R.Page * R.PageCount >= P.Count)
            //    {
            //        Skip = 0; Take = Mod == 0 ? R.PageCount : Mod;
            //    }
            //    else
            //    {
            //        Skip = (Page - R.Page - 1) * R.PageCount + Mod;
            //    }
            //}
            //else
            //{
            //    Tempdb.OrderBy(x => x.ID);//升序
            //}
            //Tempdb.Skip(Skip);
            //Tempdb.Take(Take);
            //var list = Tempdb.ToList();
        }

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <param name="entity">要删除的实体对象</param>
        /// <returns>受影响的记录数</returns>
        public virtual async Task<bool> DeleteAsync(TEntity entity)
        {
            return await Task.FromResult(DbContext.Delete(entity));
        }

        /// <summary>
        /// 保存实体对象
        /// </summary>
        /// <param name="entity">要保存的实体对象</param>
        /// <returns>返回保存成功后的实体Id</returns>
        public virtual async Task<bool> SaveAsync(TEntity entity)
        {
            if (entity.CreateTime < DateTime.Parse("1900-01-01"))
            {
                entity.CreateTime = DateTime.Now;
            }
            if (entity.LastModifyTime < DateTime.Parse("1900-01-01"))
            {
                entity.LastModifyTime = DateTime.Now;
            }
            if (string.IsNullOrEmpty(entity.CreateUserId))
            {
                entity.CreateUserId = Guid.Empty.ToString();
            }
            if (string.IsNullOrEmpty(entity.LastModifyUserId))
            {
                entity.LastModifyUserId = Guid.Empty.ToString();
            }

            return await Task.FromResult(DbContext.Insert(entity)) ;
        }

        /// <summary>
        /// 更新实体对象
        /// </summary>
        /// <param name="entity">要更新的实体对象</param>
        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            return await Task.FromResult(DbContext.Update(entity));
        }

        /// <summary>
        /// 通过某父Id获取列表数据
        /// </summary>
        /// <param name="parentName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> GetByParentId(string parentName, string value)
        {
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel(){FieldName = parentName,ConditionalType = ConditionalType.Equal,FieldValue = value});

            var result = await Db.Queryable<TEntity>().Where(conModels).ToListAsync();
            return result;
        }

    }
}
