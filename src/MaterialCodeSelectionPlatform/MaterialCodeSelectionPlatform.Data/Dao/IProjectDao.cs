﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CommodityCodeSelectionPlatform.Domain;
using CommodityCodeSelectionPlatform.Domain.Entities;

namespace CommodityCodeSelectionPlatform.Data
{
    public partial interface IProjectDao
    {
        /// <summary>
        /// 搜索项目
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        Task<List<Project>> GetDataList(ProjectSearchCondition searchCondition);

        /// <summary>
        /// 验证页面元素
        /// </summary>
        /// <param name="nameCode"></param>
        /// <returns></returns>
        Task<bool> VerifNameOrCodeAsync(string nameCode, string id);
    }
}