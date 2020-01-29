using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SqlSugar;
using CommodityCodeSelectionPlatform.Domain;

namespace CommodityCodeSelectionPlatform.Data
{
    public interface IEntityDao<TDataEntity> : IDao<TDataEntity, string> where TDataEntity : IDataEntity
    {
    }

    /// <summary>
    /// 泛型 DAO 接口.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TID"></typeparam>
    public partial interface IDao<TEntity, TID> : ILogable where TEntity : IIdEntity<TID>
    {
        /// <summary>
        /// 获取总记录数
        /// </summary>
        /// <returns>返回总记录数</returns>
        Task<long> CountAsync();

        /// <summary>
        /// 根据Id删除实体
        /// </summary>
        /// <param name="id">要删除实体的Id</param>
        /// <returns>受影响的记录数，0表示没有符合条件的实体</returns>
        Task<bool> DeleteByIdAsync(string id);

        /// <summary>
        /// 根据ID获取实体.
        /// </summary>
        /// <param name="id">实体Id</param>
        /// <returns>返回实体对象或null（如果不存在)</returns>
        Task<TEntity> GetAsync(TID id);


        /// <summary>
        /// 获取所有实体对象集合
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetListAsync();

        /// <summary>
        /// 获取实体对象集合页
        /// </summary>
        /// <param name="page">分页信息</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetListPagedAsync(PageModel page);


        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <param name="entity">要删除的实体对象</param>
        /// <returns>受影响的记录数</returns>
        Task<bool> DeleteAsync(TEntity entity);


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
        /// 通过某父Id获取列表数据
        /// </summary>
        /// <param name="parentName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetByParentId(string parentName, string value);


    }

    /// <summary>
    /// 记录日志
    /// </summary>
    public interface ILogable
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        ILogger Logger { get; }
    }
}
