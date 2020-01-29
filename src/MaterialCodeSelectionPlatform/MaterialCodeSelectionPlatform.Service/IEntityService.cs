using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using CommodityCodeSelectionPlatform.Data;
using CommodityCodeSelectionPlatform.Domain;
using CommodityCodeSelectionPlatform.Domain.Entities;


namespace CommodityCodeSelectionPlatform.Service
{
    public partial interface IEntityService<TEntity> : ILogable where TEntity : IDataEntity
    {
        /// <summary>
        /// 根据实体Id获取实体对象
        /// </summary>
        /// <param name="id">要获取的实体对象Id</param>
        /// <returns>返回实体对象或null（实体不存在）</returns>
        Task<TEntity> GetAsync(string id);



        /// <summary>
        /// 获取所有实体对象集合
        /// </summary>
        /// <returns>所有实体对象集合</returns>
        Task<IEnumerable<TEntity>> GetListAsync();

        /// <summary>
        /// 获取实体对象集合页
        /// </summary>
        /// <param name="page">分页信息</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetListPagedAsync(PageModel page);

        /// <summary>
        /// 保存实体对象
        /// </summary>
        /// <param name="entity">要保存的实体对象</param>
        /// <returns>返回保存成功后的实体Id</returns>
        Task<bool> SaveAsync(TEntity entity);

        /// <summary>
        /// 根新实体对象
        /// </summary>
        /// <param name="entity">要更新的实体对象</param>
        Task<bool> UpdateAsync(TEntity entity);

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <param name="entity">要删除的实体对象</param>
        Task<bool> DeleteAsync(TEntity entity);

        ///// <summary>
        ///// 删除实体对象
        ///// </summary>
        ///// <param name="entity">要删除的实体对象</param>
        ///// <param name="force">是否物理删除</param>
        ///// <return>受影响的记录数</return>
        //Task<int DeleteAsync(TEntity entity, bool force);

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <param name="entity">要删除的实体对象Id</param>
        /// <param name="force">是否物理删除</param>
        /// <return>受影响的记录数</return>
        Task<bool> DeleteByIdAsync(string id);

        /// <summary>
        /// 记录总数
        /// </summary>
        /// <returns></returns>
        Task<long> CountAsync();

        /// <summary>
        /// 通过某父Id获取列表数据
        /// </summary>
        /// <param name="parentName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetByParentId(string parentName, string value);

        ///// <summary>
        ///// 获取实体对象集合
        ///// </summary>
        ///// <param name="parentProperty">父对象属性名</param>
        ///// <param name="parentId">父对象Id</param>
        ///// <returns></returns>
        //Task<IEnumerable<TEntity>> GetListByParentIdAsync(string parentProperty, string parentId);

        ///// <summary>
        ///// 获取实体对象集合页
        ///// </summary>
        ///// <param name="parentProperty">父对象属性名</param>
        ///// <param name="parentId">父对象Id</param>
        ///// <param name="page">分页信息</param>
        ///// <returns></returns>
        //Task<IEnumerable<TEntity>> GetListPagedByParentIdAsync(string parentProperty, string parentId, DataPage page);
    }
}
