using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SqlSugar;
using MaterialCodeSelectionPlatform.Data;
using MaterialCodeSelectionPlatform.Domain;


namespace MaterialCodeSelectionPlatform.Service
{
    public partial class EntityServiceBase<TEntity> : IEntityService<TEntity> where TEntity : IDataEntity
    {
        private ILogger _logger;
        private ILoggerFactory loggerFactory;
        public ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = loggerFactory.CreateLogger(this.GetType());
                }
                return _logger;
            }
        }
        

        public EntityServiceBase(IEntityDao<TEntity> dao,ILoggerFactory loggerFactory)
        {
            _Dao = dao;
            this.loggerFactory = loggerFactory;
        }
        #region 字段               

        /// <summary>
        /// 实体Dao
        /// </summary>
        IEntityDao<TEntity> _Dao;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置Dao
        /// </summary>
        public IEntityDao<TEntity> Dao
        {
            get { return _Dao; }
            set { _Dao = value; }
        }



        #endregion

        /// <summary>
        /// 根据实体Id获取实体对象
        /// </summary>
        /// <param name="id">要获取的实体对象Id</param>
        /// <returns>返回实体对象或null（实体不存在）</returns>
        public async Task<TEntity> GetAsync(string id)
        {
            return await Dao.GetAsync(id);
        }


        /// <summary>
        /// 获取所有实体对象集合
        /// </summary>
        /// <returns>所有实体对象集合</returns>
        public async Task<IEnumerable<TEntity>> GetListAsync()
        {
            return await Dao.GetListAsync();
        }

        /// <summary>
        /// 获取实体对象集合页
        /// </summary>
        /// <param name="page">分页信息</param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetListPagedAsync(PageModel page)
        {
            return await Dao.GetListPagedAsync(page);
        }

        /// <summary>
        /// 保存实体对象
        /// </summary>
        /// <param name="entity">要保存的实体对象</param>
        /// <returns>返回保存成功后的实体Id</returns>
        public async Task<bool> SaveAsync(TEntity entity)
        {
            return await Dao.SaveAsync(entity);
        }

        /// <summary>
        /// 根新实体对象
        /// </summary>
        /// <param name="entity">要更新的实体对象</param>
        public async Task<bool> UpdateAsync(TEntity entity)
        {
            return await Dao.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <param name="entity">要删除的实体对象</param>
        public async Task<bool> DeleteAsync(TEntity entity)
        {
            return await Dao.DeleteAsync(entity);
        }

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <param name="entity">要删除的实体对象</param>
        /// <param name="force">是否物理删除</param>
        /// <return>受影响的记录数</return>
        public async Task<bool> DeleteAsync(TEntity entity, bool force)
        {
            return await Dao.DeleteAsync(entity);
        }


        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <param name="id">要删除的实体对象Id</param>
        /// <return>受影响的记录数</return>
        public async Task<bool> DeleteByIdAsync(string id)
        {
            return await Dao.DeleteByIdAsync(id);
        }

        /// <summary>
        /// 记录总数
        /// </summary>
        /// <returns></returns>
        public async Task<long> CountAsync()
        {
            return await Dao.CountAsync();
        }


        ///// <summary>
        ///// 获取实体对象集合
        ///// </summary>
        ///// <param name="parentProperty">父对象属性名</param>
        ///// <param name="parentId">父对象Id</param>
        ///// <returns></returns>
        //public Task<IEnumerable<TEntity>> GetListByParentIdAsync(string parentProperty, string parentId)
        //{
        //    return Dao.GetListPagedAsync()
        //}

        ///// <summary>
        ///// 获取实体对象集合页
        ///// </summary>
        ///// <param name="parentProperty">父对象属性名</param>
        ///// <param name="parentId">父对象Id</param>
        ///// <param name="page">分页信息</param>
        ///// <returns></returns>
        //public Task<IEnumerable<TEntity>> GetListPagedByParentIdAsync(string parentProperty, string parentId, DataPage page);

        /// <summary>
        /// 通过某父Id获取列表数据
        /// </summary>
        /// <param name="parentName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<List<TEntity>> GetByParentId(string parentName, string value)
        {
            return await Dao.GetByParentId(parentName, value);
        }
    }
}
