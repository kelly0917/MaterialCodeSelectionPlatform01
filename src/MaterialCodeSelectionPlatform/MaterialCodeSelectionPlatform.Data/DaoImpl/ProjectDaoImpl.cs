﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.Utilities;
using SqlSugar;

namespace MaterialCodeSelectionPlatform.Data
{
    public partial class ProjectDaoImpl
    {


        /// <summary>
        /// 搜索编码库
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public async Task<List<Project>> GetDataList(ProjectSearchCondition searchCondition)
        {
            var query = Db.Queryable<Project>();

            //if (searchCondition.UserId.IsNotNullAndNotEmpty())
            //{
            //    query = query.Where(c => c.CreateUserId==searchCondition.UserId);
            //}
            if (searchCondition.Name.IsNotNullAndNotEmpty())
            {
                query = query.Where(c => c.Name.Contains(searchCondition.Name));
            }
            if (searchCondition.Code.IsNotNullAndNotEmpty())
            {
                query = query.Where(c => c.Code.Contains(searchCondition.Code));
            }

            if (searchCondition.Status != -1)
            {
                query = query.Where(c => c.Status == searchCondition.Status);
            }

            var total = 0;
            var data = await query.ToPageListAsync(searchCondition.Page.PageNo, searchCondition.Page.PageSize, total);
            searchCondition.Page.RecordCount = data.Value;
            return data.Key;
        }


        /// <summary>
        /// 验证页面元素
        /// </summary>
        /// <param name="nameCode"></param>
        /// <returns></returns>
        public async Task<bool> VerifNameOrCodeAsync(string nameCode, string id)
        {
            var result = false;
            if (string.IsNullOrEmpty(id))
            {
                var model =await DbContext.AsQueryable().Where(c => c.Name == nameCode || c.Code == nameCode).FirstAsync();
                if (model == null)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                var model = await DbContext.AsQueryable().Where(c => c.Name == nameCode || c.Code == nameCode).Where(c=>c.Id !=id).FirstAsync();
                if (model == null)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return await Task.FromResult(result);
        }


        /// <summary>
        /// 获取待分配的编码库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Catalog>> GetLeftCatalogs(string id)
        {
            var ids = await Db.Queryable<Catalog, ProjectCatalogMap>((c, p) => new object[]
            {
                JoinType.Inner,c.Id ==p.CatalogId
            }).Where((c,p) => p.ProjectId == id).Select((c,p) => c.Id).ToListAsync();

            var idsStr = string.Join(",", ids);
            var list = await Db.Queryable<Catalog>().Where(c => !SqlFunc.Contains(idsStr, c.Id)).ToListAsync();
            return list;
        }


        /// <summary>
        /// 获取已分配的编码库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Catalog>> GetRightCatalogs(string id)
        {
            var query = Db.Queryable<Catalog, ProjectCatalogMap>((c, p) => new object[]
            {
                JoinType.Inner,c.Id ==p.CatalogId
            }).Where((c, p) => p.ProjectId == id).Select((c,p) => c);

            return await query.ToListAsync();
        }

        /// <summary>
        /// 保存用户分配的编码库
        /// </summary>
        /// <param name="catalogs"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<int> SaveProjectCatlogs(List<string> catalogs, string userId, string projectId)
        {
            var list = await Db.Queryable<ProjectCatalogMap>().Where(c => c.ProjectId == projectId).Select(c => c.Id).ToListAsync();
            await Db.Deleteable<ProjectCatalogMap>().In(list).ExecuteCommandAsync();

            if (catalogs.Count == 0)
            {
                return 0;
            }
            List<ProjectCatalogMap> ProjectCatalogMaps = new List<ProjectCatalogMap>();
            foreach (var catlogId in catalogs)
            {
                ProjectCatalogMap ProjectCatalogMap = new ProjectCatalogMap();
                ProjectCatalogMap.Id = Guid.NewGuid().ToString();
                ProjectCatalogMap.ProjectId = projectId;
                ProjectCatalogMap.Flag = 0;
                ProjectCatalogMap.ProjectId = projectId;
                ProjectCatalogMap.Status = 0;
                ProjectCatalogMap.CatalogId = catlogId;
                ProjectCatalogMap.CreateTime = DateTime.Now;
                ProjectCatalogMap.LastModifyTime = DateTime.Now;
                ProjectCatalogMap.LastModifyUserId = userId;
                ProjectCatalogMap.CreateUserId = userId;
                ProjectCatalogMaps.Add(ProjectCatalogMap);
            }
            return await Db.Insertable<ProjectCatalogMap>(ProjectCatalogMaps).ExecuteCommandAsync();
        }




        /// <summary>
        /// 获取待分配的用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<User>> GetLeftUsers(string id)
        {
            var ids = await Db.Queryable<User, UserProjectMap>((u, p) => new object[]
            {
                JoinType.Inner,u.Id ==p.UserId
            }).Where((u, p) => p.ProjectId == id).Select((u, p) => u.Id).ToListAsync();

            var idsStr = string.Join(",", ids);
            var list = await Db.Queryable<User>().Where(c => !SqlFunc.Contains(idsStr, c.Id)).ToListAsync();
            return list;
        }


        /// <summary>
        /// 获取已分配的编码库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<User>> GetRightUsers(string id)
        {
            var query = Db.Queryable<User, UserProjectMap>((u, p) => new object[]
            {
                JoinType.Inner,u.Id ==p.UserId
            }).Where((u, p) => p.ProjectId == id).Select((u, p) => u);

            return await query.ToListAsync();
        }


        /// <summary>
        /// 保存用户分配的编码库
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> SaveUserProjects(List<string> userIds, string userId, string projectId)
        {
            userIds = userIds.Distinct().ToList();
            var list = await Db.Queryable<UserProjectMap>().Where(c => c.ProjectId == projectId).Select(c => c.Id).ToListAsync();
            await Db.Deleteable<UserProjectMap>().In(list).ExecuteCommandAsync();

            if (userIds.Count == 0)
            {
                return 0;
            }
            List<UserProjectMap> userProjectMaps = new List<UserProjectMap>();
            foreach (var users in userIds)
            {
                UserProjectMap userProjectMap = new UserProjectMap();
                userProjectMap.Id = Guid.NewGuid().ToString();
                userProjectMap.ProjectId = projectId;
                userProjectMap.Flag = 0;
                userProjectMap.ProjectId = projectId;
                userProjectMap.Status = 0;
                userProjectMap.UserId = users;
                userProjectMap.CreateTime = DateTime.Now;
                userProjectMap.LastModifyTime = DateTime.Now;
                userProjectMap.LastModifyUserId = userId;
                userProjectMap.CreateUserId = userId;
                userProjectMaps.Add(userProjectMap);
            }
            return await Db.Insertable<UserProjectMap>(userProjectMaps).ExecuteCommandAsync();
        }

    }
}