using System.Collections.Generic;
using System.Linq;
using MaterialCodeSelectionPlatform.SysDataTool.IServices;
using MaterialCodeSelectionPlatform.SysDataTool.Utilities;

namespace MaterialCodeSelectionPlatform.SysDataTool.Services
{
    public class SysContext
    {
        /// <summary>
        /// 所有编码库
        /// </summary>
        public List<string> Catlogs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SysContext()
        {
            Catlogs = CacheData.ConfigCache.Select(c => c.Name).ToList();
        }


        public void SysData()
        {
            ISysDataService sysDataClassService = new SysClassDataService();
            sysDataClassService.SysData(string.Empty);


            ISysDataService sysCommodityCodeService = new SysCommodityCodeService();
            sysCommodityCodeService.SysData(string.Empty);
        }

    }
}