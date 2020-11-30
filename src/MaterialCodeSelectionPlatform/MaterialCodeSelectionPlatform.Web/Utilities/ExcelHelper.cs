using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using log4net;
using MaterialCodeSelectionPlatform.Domain;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace MaterialCodeSelectionPlatform.Web.Common
{
    public class ExcelHelper
    {
        private static ILog log = LogHelper.GetLogger<Object>();

        /// <summary>
        /// 将excel文件内容读取到DataTable数据表中
        /// </summary>
        /// <param name="fileName">文件完整路径名</param>
        /// <param name="sheetName">指定读取excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名：true=是，false=否</param>
        /// <returns>DataTable数据表</returns>
        public static DataTable ReadExcelToDataTable(string fileName, string sheetName = null,
            bool isFirstRowColumn = true)
        {
            //定义要返回的datatable对象
            DataTable data = new DataTable();
            //excel工作表
            ISheet sheet = null;
            //数据开始行(排除标题行)
            int startRow = 0;
            try
            {
                if (!File.Exists(fileName))
                {
                    return null;
                }

                //根据指定路径读取文件
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                //根据文件流创建excel数据结构
                IWorkbook workbook = WorkbookFactory.Create(fs);
                //IWorkbook workbook = new HSSFWorkbook(fs);
                //如果有指定工作表名称
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheet(sheetName);
                    //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    //如果没有指定的sheetName，则尝试获取第一个sheet
                    sheet = workbook.GetSheetAt(0);
                }

                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    //一行最后一个cell的编号 即总的列数
                    int cellCount = firstRow.LastCellNum;
                    //如果第一行是标题列名
                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }

                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }

                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// 将文件流读取到DataTable数据表中
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="sheetNames">指定读取excel工作薄sheet的名称数组</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名：true=是，false=否</param>
        /// <returns>DataTable数据表</returns>
        public static Dictionary<string, DataTable> ReadStreamToDataTable(Stream fileStream, List<string> sheetNames,
            bool isFirstRowColumn = true)
        {
            Dictionary<string, DataTable> result = new Dictionary<string, DataTable>();
            //定义要返回的datatable对象
            //DataTable data = new DataTable();

            //excel工作表
            ISheet sheet = null;
            //数据开始行(排除标题行)
            int startRow = 0;
            try
            {
                //根据文件流创建excel数据结构,NPOI的工厂类WorkbookFactory会自动识别excel版本，创建出不同的excel数据结构
                IWorkbook workbook = WorkbookFactory.Create(fileStream);


                foreach (var sheetName in sheetNames)
                {
                    if (result.ContainsKey(sheetName))
                        continue;

                    DataTable data = new DataTable();
                    //如果有指定工作表名称
                    if (!string.IsNullOrEmpty(sheetName))
                    {
                        sheet = workbook.GetSheet(sheetName);
                        //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                        if (sheet == null)
                        {
                            sheet = workbook.GetSheetAt(0);
                        }
                    }
                    else
                    {
                        //如果没有指定的sheetName，则尝试获取第一个sheet
                        sheet = workbook.GetSheetAt(0);
                    }

                    if (sheet != null)
                    {
                        IRow firstRow = sheet.GetRow(0);
                        //一行最后一个cell的编号 即总的列数
                        int cellCount = firstRow.LastCellNum;
                        //如果第一行是标题列名
                        if (isFirstRowColumn)
                        {
                            for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                            {
                                ICell cell = firstRow.GetCell(i);
                                if (cell != null)
                                {
                                    string cellValue = cell.StringCellValue;
                                    if (cellValue != null)
                                    {
                                        DataColumn column = new DataColumn(cellValue);
                                        data.Columns.Add(column);
                                    }
                                }
                            }

                            startRow = sheet.FirstRowNum + 1;
                        }
                        else
                        {
                            startRow = sheet.FirstRowNum;
                        }

                        //最后一列的标号
                        int rowCount = sheet.LastRowNum;
                        for (int i = startRow; i <= rowCount; ++i)
                        {
                            IRow row = sheet.GetRow(i);
                            if (row == null || row.FirstCellNum < 0) continue; //没有数据的行默认是null　　　　　　　

                            DataRow dataRow = data.NewRow();
                            for (int j = row.FirstCellNum; j < cellCount; ++j)
                            {
                                //同理，没有数据的单元格都默认是null
                                ICell cell = row.GetCell(j);
                                if (cell != null)
                                {
                                    if (cell.CellType == CellType.Numeric)
                                    {
                                        //判断是否日期类型
                                        if (DateUtil.IsCellDateFormatted(cell))
                                        {
                                            dataRow[j] = row.GetCell(j).DateCellValue;
                                        }
                                        else
                                        {
                                            dataRow[j] = row.GetCell(j).ToString().Trim();
                                        }
                                    }
                                    else
                                    {
                                        dataRow[j] = row.GetCell(j).ToString().Trim();
                                    }
                                }
                            }

                            data.Rows.Add(dataRow);
                        }
                    }

                    result.Add(sheetName, data);
                }

                return result;
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                return null;
            }
        }



        /// <summary>
        /// Table转换为Excel，保存
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="sheetNames"></param>
        /// <returns></returns>
        public static bool WriteTableToExcel(List<DataTable> tables, List<string> sheetNames, string fileName)
        {
            try
            {
                XSSFWorkbook workbook = new XSSFWorkbook();
                using (MemoryStream ms = new MemoryStream())
                {
                    for (var i = 0; i < sheetNames.Count; i++)
                    {
                        var sheetName = sheetNames[i];
                        var table = tables[i];
                        var sheet = workbook.CreateSheet(sheetName);
                        var headerRow = sheet.CreateRow(0);
                        //设置Excel表头
                        for (var j = 0; j < table.Columns.Count; j++)
                        {
                            headerRow.CreateCell(j).SetCellValue(table.Columns[j].ColumnName);
                        }

                        for (var r = 0; r < table.Rows.Count; r++)
                        {
                            var row = sheet.CreateRow(r + 1);
                            for (var j = 0; j < table.Columns.Count; j++)
                            {
                                row.CreateCell(j).SetCellValue(table.Rows[r][table.Columns[j].ColumnName].ToString());
                            }
                        }
                    }

                    workbook.Write(ms);

                    var dir = Path.GetDirectoryName(fileName);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        byte[] data = ms.ToArray();
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                    }

                    return true;

                }
            }
            catch (Exception e)
            {
                log.LogError(e);
                return false;
            }


        }

        public static string WriteDataTable(List<Domain.PartNumberReport> dataList,string templatePath,string saveFilePath, string sheetName = null, int titleRowIndex=0)
        {
          
            //excel工作表
            ISheet sheet = null;
            //数据开始行(排除标题行)
            int startRow = 0;
            try
            {
                if (!File.Exists(templatePath))
                {
                    return null;
                }

                //根据指定路径读取文件
                FileStream fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
                //根据文件流创建excel数据结构
                IWorkbook workbook = WorkbookFactory.Create(fs);

                //IWorkbook workbook = new HSSFWorkbook(fs);

                var sheetCount=workbook.NumberOfSheets;
                var referenceNameList = GeReferencetNameList(workbook);//引用字段列表
                if (sheetCount > 0)
                {
                    for (var i = 0; i < sheetCount; i++)
                    {
                        sheet = workbook.GetSheetAt(i);
                        if (sheet != null)
                        {
                            setExcelTemplateValue(sheet, dataList);//填充模板$xxx$的值 
                            var refList = referenceNameList.Where(c => c.SheetName == sheet.SheetName).ToList();
                            var dict = GetRefColumnDic(refList, sheet.SheetName, ref startRow);//查找【名称管理器】                           
                            if (dict != null && dict.Count > 0)
                            {
                                #region 填充明细记录
                                int seqNo = 1;
                                var dtoList = new List<Domain.PartNumberReportDetail>();
                                foreach (var model in dataList)
                                {
                                    if (model.PartNumberReportDetailList != null && model.PartNumberReportDetailList.Count > 0)
                                    {
                                        dtoList.AddRange(model.PartNumberReportDetailList);
                                    }
                                }
                                #region 排序
                                if (dtoList != null && dtoList.Count > 0)
                                {
                                    var index = 1;
                                    foreach (IGrouping<string, PartNumberReportDetail> item in dtoList.GroupBy(c => c.T_Code))
                                    {
                                        item.ToList().ForEach((t) => { t.T_Seq = index; });
                                        index++;
                                        //newPartNumberReportDetail.AddRange(item);
                                    }
                                    index = 1;
                                    foreach (IGrouping<string, PartNumberReportDetail> item in dtoList.GroupBy(c => c.C_Code))
                                    {
                                        item.ToList().ForEach((t) => { t.C_Seq = index; });
                                        index++;
                                        // newPartNumberReportDetail.AddRange(item);
                                    }
                                    index = 1;
                                    dtoList.ToList().ForEach((t) => { t.P_Seq = index; index++; });
                                   
                                    dtoList = dtoList.OrderBy(c => c.T_Seq).ThenBy(c => c.C_Seq).ThenBy(c => c.P_Seq).ToList();
                                }
                                #endregion
                                createRowValue<Domain.PartNumberReportDetail>(dtoList, workbook, sheet, dict, titleRowIndex, startRow, ref seqNo);
                                #endregion
                            }
                        }
                    }
                }
                //保存文件
                var dirPath = Path.GetDirectoryName(templatePath);
                var name = Path.GetFileNameWithoutExtension(templatePath);
                //var newFileName = Path.GetDirectoryName(templatePath) +Path.DirectorySeparatorChar+ name+DateTime.Now.ToString("yyyyMMddHHmmss")+ ".xlsx";
                using (var ws = File.Create(saveFilePath))
                {
                    workbook.Write(ws);
                }
                return saveFilePath;
            }
            catch (Exception ex)
            {
                log.LogError(ex);
                return null;
            }
        }
        //插入
        private static void InsertRow(ISheet sheet, int insertRowIndex, int insertRowCount, IRow formatRow)
        {
            sheet.ShiftRows(insertRowIndex, sheet.LastRowNum, insertRowCount, true, false);
            for (int i = insertRowIndex; i < insertRowIndex + insertRowCount; i++)
            {
                IRow targetRow = null;
                ICell sourceCell = null;
                ICell targetCell = null;
                targetRow = sheet.CreateRow(i);
                for (int m = formatRow.FirstCellNum; m < formatRow.LastCellNum; m++)
                {
                    sourceCell = formatRow.GetCell(m);
                    if (sourceCell == null)
                    {
                        continue;
                    }
                    targetCell = targetRow.CreateCell(m);
                    targetCell.CellStyle = sourceCell.CellStyle;
                    targetCell.SetCellType(sourceCell.CellType);

                }
            }
        }
        private static void CopyRow(IWorkbook workbook, ISheet worksheet, int sourceRowNum, int startRowIndex, int insertCount)
        {
            // Get the source / new row
            var newRow = worksheet.GetRow(startRowIndex);
            var sourceRow = worksheet.GetRow(sourceRowNum);

            // If the row exist in destination, push down all rows by 1 else create a new row
            if (newRow != null)
            {
                worksheet.ShiftRows(startRowIndex, worksheet.LastRowNum, insertCount);
            }
            else
            {
                newRow = worksheet.CreateRow(startRowIndex);
            }

            // Loop through source columns to add to new row
            for (int i = 0; i < sourceRow.LastCellNum; i++)
            {
                // Grab a copy of the old/new cell
                var oldCell = sourceRow.GetCell(i);
                var newCell = newRow.CreateCell(i);

                // If the old cell is null jump to next cell
                if (oldCell == null)
                {
                    newCell = null;
                    continue;
                }

                // Copy style from old cell and apply to new cell
                var newCellStyle = workbook.CreateCellStyle();
                newCellStyle.CloneStyleFrom(oldCell.CellStyle); ;
                newCell.CellStyle = newCellStyle;

                // If there is a cell comment, copy
                if (newCell.CellComment != null) newCell.CellComment = oldCell.CellComment;

                // If there is a cell hyperlink, copy
                if (oldCell.Hyperlink != null) newCell.Hyperlink = oldCell.Hyperlink;

                // Set the cell data type
                newCell.SetCellType(oldCell.CellType);

                // Set the cell data value
                switch (oldCell.CellType)
                {
                    case CellType.Blank:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                    case CellType.Boolean:
                        newCell.SetCellValue(oldCell.BooleanCellValue);
                        break;
                    case CellType.Error:
                        newCell.SetCellErrorValue(oldCell.ErrorCellValue);
                        break;
                    case CellType.Formula:
                        newCell.SetCellFormula(oldCell.CellFormula);
                        break;
                    case CellType.Numeric:
                        newCell.SetCellValue(oldCell.NumericCellValue);
                        break;
                    case CellType.String:
                        newCell.SetCellValue(oldCell.RichStringCellValue);
                        break;
                    case CellType.Unknown:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                }
            }

            // If there are are any merged regions in the source row, copy to new row
            for (int i = 0; i < worksheet.NumMergedRegions; i++)
            {
                CellRangeAddress cellRangeAddress = worksheet.GetMergedRegion(i);
                if (cellRangeAddress.FirstRow == sourceRow.RowNum)
                {
                    CellRangeAddress newCellRangeAddress = new CellRangeAddress(newRow.RowNum,
                                                                                (newRow.RowNum +
                                                                                 (cellRangeAddress.FirstRow -
                                                                                  cellRangeAddress.LastRow)),
                                                                                cellRangeAddress.FirstColumn,
                                                                                cellRangeAddress.LastColumn);
                    worksheet.AddMergedRegion(newCellRangeAddress);
                }
            }

        }
        /// <summary>
        /// 复制行格式并插入指定行数
        /// </summary>
        /// <param name="sheet">当前sheet</param>
        /// <param name="startRowIndex">起始行位置</param>
        /// <param name="sourceRowIndex">模板行位置</param>
        /// <param name="insertCount">插入行数</param>
        public static void CopyRow(ISheet sheet, int startRowIndex, int sourceRowIndex, int insertCount)
        {
            IRow sourceRow =sheet.GetRow(sourceRowIndex);
            int sourceCellCount = sourceRow.Cells.Count;

            //1. 批量移动行,清空插入区域
            sheet.ShiftRows(startRowIndex, //开始行
                            sheet.LastRowNum, //结束行
                            insertCount, //插入行总数
                            true,        //是否复制行高
                            false        //是否重置行高
                            );

            int startMergeCell = -1; //记录每行的合并单元格起始位置
            for (int i = startRowIndex; i < startRowIndex + insertCount; i++)
            {
                IRow targetRow = null;
                ICell sourceCell = null;
                ICell targetCell = null;

                targetRow = sheet.CreateRow(i);
                targetRow.Height = sourceRow.Height;//复制行高

                for (int cellIndex = sourceRow.FirstCellNum; cellIndex < sourceRow.LastCellNum; cellIndex++)
                {
                    sourceCell = sourceRow.GetCell(cellIndex);
                    if (sourceCell == null)
                        continue;
                    targetCell = targetRow.CreateCell(cellIndex);
                    targetCell.CellStyle = sourceCell.CellStyle;//赋值单元格格式
                    targetCell.SetCellType(sourceCell.CellType);

                }

                //以下为复制模板行的单元格合并格式.
                //var mergeCells = GetMergeCells(sheet);
                //var sourceRowMgergeCells = mergeCells.Where(c => c.FirstRow == sourceRowIndex && c.LastRow == sourceRowIndex).ToList();
                //if (sourceRowMgergeCells != null && sourceRowMgergeCells.Count > 0)
                //{
                //    for (var k = startRowIndex; k < startRowIndex + insertCount; k++)
                //    {
                //        foreach (var mgerge in sourceRowMgergeCells)
                //        {
                //            sheet.AddMergedRegion(new CellRangeAddress(k, k, mgerge.FirstCol, mgerge.LastCol));
                //        }
                //    }
                //}

                ////if (sourceCell.IsMergedCell)
                ////{
                ////    if (startMergeCell <= 0)
                ////        startMergeCell = cellIndex;
                ////    else if (startMergeCell > 0 && sourceCellCount == cellIndex + 1)
                ////    {
                ////        sheet.AddMergedRegion(new CellRangeAddress(i, i, startMergeCell, cellIndex));
                ////        startMergeCell = -1;
                ////    }
                ////}
                ////else
                ////{
                ////    if (startMergeCell >= 0)
                ////    {
                ////        sheet.AddMergedRegion(new CellRangeAddress(i, i, startMergeCell, cellIndex - 1));
                ////        startMergeCell = -1;
                ////    }
                ////}
            }
        }
        /// <summary>
        /// 创建明细记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="dict">记录名称管理器的字典</param>
        /// <param name="titleRowIndex">指定一个标题的行</param>
        /// <param name="startRow">记录最大开始产生数据的列</param>
        /// <param name="seqNo">序号开始的索引 </param>
        private static void createRowValue<T>(List<T>  dataList,IWorkbook workbook, ISheet sheet,Dictionary<string,string> dict, int titleRowIndex,int startRow,ref int seqNo)
        {
            //var referenceNameList = GeReferencetNameList(workbook, sheet.SheetName);//引用字段列表
            if (titleRowIndex == 0)
            {
                titleRowIndex = startRow;
            }
            var mergeRowIndex = startRow + 2;
            var mergeCells = GetMergeCells(sheet);
            IRow titleRow = sheet.GetRow(titleRowIndex);
            int rowCount = sheet.LastRowNum;//总行数
            int cellCount = titleRow.Cells.Count;//一行最后一个cell的编号 即总的列数
                                                 //var dict = GetRefColumnDic(referenceNameList, sheet.SheetName, ref startRow);

            if (dataList != null && dataList.Count > 0)
            {
                startRow = startRow + 1;//移下一行填充数据             
                IRow styleRow = sheet.GetRow(startRow);
                var insertCount = dataList.Count == 1 ? 1 : dataList.Count - 1;
               CopyRow(sheet, startRow+1, startRow, insertCount);              

                foreach (var item in dataList)
                {
                    Type t = item.GetType();
                    IRow row = sheet.GetRow(startRow++);
                    for (var i = 0; i < cellCount; i++)
                    {
                        ICellStyle style = null;
                        var styleCell = styleRow.GetCell(i);
                        var cell = row.GetCell(i);
                        if (cell != null)
                        {
                            //style = workbook.CreateCellStyle();
                            //style.CloneStyleFrom(styleCell.CellStyle);//复制原本的样式
                        }
                        else
                        {
                            cell = row.CreateCell(i);
                            //if (styleCell != null)
                            //{
                            //    style = workbook.CreateCellStyle();
                            //    style.CloneStyleFrom(styleCell.CellStyle);//复制其中一列的样式
                            //}
                        }
                        if (dict.ContainsKey(i.ToString()))
                        {

                            //cell.CellStyle = style;
                            PropertyInfo propertyInfo = t.GetProperties().FirstOrDefault(w => w.Name.ToLower() == dict[i.ToString()].ToString());
                            if (propertyInfo != null)
                            {
                                cell.SetCellValue(propertyInfo.GetValue(item, null)?.ToString());
                            }
                            if (dict[i.ToString()].ToLower() == "seqno")
                            {
                                cell.SetCellValue(seqNo++);
                            }

                        }
                        else
                        {

                            //if (style != null)
                            //    cell.CellStyle = style;
                            cell.SetCellValue("");
                        }
                    }

                }

                //以下为复制模板行的单元格合并格式.
                var sourceRowIndex = mergeRowIndex;
                //sheet.AddMergedRegion(new CellRangeAddress(7, 7, 1, 3));
                //sheet.AddMergedRegion(new CellRangeAddress(7, 7, 4, 6));
                //sheet.AddMergedRegion(new CellRangeAddress(7, 7, 7, 9));
                var sourceRowMgergeCells = mergeCells.Where(c => c.FirstRow == sourceRowIndex && c.LastRow == sourceRowIndex).ToList();
                if (sourceRowMgergeCells != null && sourceRowMgergeCells.Count > 0)
                {
                    var mgergeRowCount = sourceRowIndex+ dataList.Count - 1;
                    for (var k = sourceRowIndex; k < mgergeRowCount; k++)
                    {
                        foreach (var mgerge in sourceRowMgergeCells)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(k, k, mgerge.FirstCol, mgerge.LastCol));
                        }
                    }
                }
            }
        }
        private static void setExcelTemplateValue(ISheet sheet, List<Domain.PartNumberReport> dataList)
        {
            int rowCount = sheet.LastRowNum+1;//总行数
            for (var i = 0; i < rowCount; i++)
            {
                var row = sheet.GetRow(i);
                if (row != null)
                {
                    var cellCount = row.Cells.Count;
                    for (var j = 0; j < cellCount; j++)
                    {
                        var cell = row.Cells[j];
                        var cellString = cell?.ToString();
                        if (!string.IsNullOrEmpty(cellString) && cellString.IndexOf("$") > -1)
                        {
                            foreach (var ent in dataList)
                            {
                                cellString = getColumnString<Domain.PartNumberReport>(ent, cellString);
                            }
                            cell.SetCellValue(cellString);
                        }
                    }
                }
            }
        }
        private static string getColumnString<T>(T item,string cellString)
        {
            var cellValue = cellString;           
            Type t = item.GetType();
            var propertyInfos = t.GetProperties();
            if (propertyInfos != null && propertyInfos.Length > 0)
            {
                foreach (var pro in propertyInfos)
                {
                    var name ="$"+pro.Name.ToLower()+"$";
                    var value = pro.GetValue(item, null);
                    if (value != null)
                    {
                        var vName=cellString.ToLower();
                        if (vName.IndexOf(name) > -1)
                        {
                            cellValue = value.ToString();
                        }
                    }
                  
                }
            }
            return cellValue;
        }
        public static Dictionary<string, string> GetRefColumnDic(List<IName> list, string sheetName,ref int startRow)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();       // Dictionary<列索引, 字段>    
            if (list != null && list.Count > 0)
            {
                foreach (var refName in list)
                {
                    if (refName.SheetName == sheetName)
                    {
                        if (refName.RefersToFormula.IndexOf(":") < 0)
                        {
                            var refstr = refName.RefersToFormula.Split('!')[1];
                            var rowIndex = int.Parse(refstr.Split('$')[2]);
                            var cellIndex = ColumnToIndex(refstr.Split('$')[1]);
                            var key = $"{cellIndex}";
                            if (!dic.ContainsKey(key))
                            {
                                dic.Add(key, refName.NameName.ToLower());
                                //dic.Add(key, refName.NameName.ToLower().Replace("p_",""));//去掉固定的标识 
                            }
                            if (startRow < rowIndex)
                            {
                                startRow = rowIndex;
                            }
                        }
                    }
                }
            }
            startRow = startRow>0? startRow - 1:startRow;//实际行，EXCEL从0开始
            return dic;
        }
        /// <summary>
        /// 获取引用字段
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sheetName"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        public static string GetRefColumn(List<IName> list,string sheetName, int rowIndex, int cellIndex)
        {
            string name = string.Empty;
            var cellName = IndexToColumn(cellIndex);
            var formula = sheetName+"!$" + cellName + "$"+ rowIndex ;
            if (list != null && list.Count > 0)
            {
               var ent= list.Where(c => c.RefersToFormula == formula).FirstOrDefault();
                if (ent != null)
                {
                    name = ent.NameName;
                }
            }
            return name;
        }
        /// <summary>
        /// 获取引用字段的列表
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static List<IName> GeReferencetNameList(IWorkbook workbook, string sheetName = "")
        {
            List<IName> nameList = new List<IName>();
            var count = workbook.NumberOfNames;
            for (var i = 0; i < count; i++)
            {
                var name = workbook.GetNameAt(i);
                // if (name.SheetIndex==-1) continue;name.IsDeleted == false不一定存在
                if (!string.IsNullOrEmpty(sheetName) && name.RefersToFormula.IndexOf("!$") > -1)
                {
                    if (name.SheetName == sheetName)
                    {
                        if (name.RefersToFormula.IndexOf(":") < 0)
                        {
                            nameList.Add(name);
                        }
                    }
                }
                else
                {
                    if ( name.RefersToFormula.IndexOf("!$") > -1)
                    {
                        nameList.Add(name);
                    }
                }
            }
            return nameList;
        }
       
        /// <summary>
        /// 用于excel表格中列号字母转成列索引，索引从0开始，返回形如 A,B,C,...,Z,AA,AB,...,AZ,BA,...,ZZ,AAA,AAB,......
        /// </summary>
        /// <param name="column">列号</param>
        /// <returns>列索引</returns>
        private static int ColumnToIndex(string column)
        {
            if (!Regex.IsMatch(column.ToUpper(), @"[A-Z]+"))
            {
                throw new Exception("Invalid parameter");
            }
            int index = -1;
            char[] chars = column.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }
            return index;
        }
        /// <summary>
        /// 用于将excel表格中列索引转成列号字母，从A对应0开始
        /// </summary>
        /// <param name="index">列索引</param>
        /// <returns></returns>
        private static string IndexToColumn(int index)
        {
          
          //  index--;
            string column = string.Empty;
            do
            {
                if (column.Length > 0)
                {
                    index--;
                }
                column = ((char)(index % 26 + (int)'A')).ToString() + column;
                index = (int)((index - index % 26) / 26);
            } while (index > 0);
            return column;
        }
        /// <summary>
        /// 获取所有的合并单元格
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static List<MergeCell> GetMergeCells(ISheet sheet)
        {
            List<MergeCell> list = new List<MergeCell>();
           var count = sheet.NumMergedRegions;
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var cell = sheet.GetMergedRegion(i);
                    if (cell != null)
                    {
                        list.Add(new MergeCell() {FirstRow=cell.FirstRow,LastRow=cell.LastRow,FirstCol=cell.FirstColumn,LastCol=cell.LastColumn });
                    }
                }
            }
            return list;
        }




        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public static string ExportData(DataTable data, string filePath, string sheetName)
        {
            IWorkbook workbook = null;
            FileStream fs = null;
            string fileName = filePath;

            int i = 0;
            int j = 0;
            int count = 0;
            //string filePath = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }
            ISheet sheet = null;
            bool isColumnWritten = true;
            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return null;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                workbook.Write(fs); //写入到excel

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// Sheet1 导出
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ExportData(DataTable data, string filePath)
        {
            return ExportData(data, filePath, "Sheet1");
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public static DataTable ImportData(string fileName, string sheetName)
        {
            IWorkbook workbook = null;
            FileStream fs = null;
            bool isFirstRowColumn = true;
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);

                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// Sheet1的数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataTable ImportData(string fileName)
        {
            return ImportData(fileName, "Sheet1");
        }
    }
    /// <summary>
    /// 合并单元格
    /// </summary>
    public class MergeCell
    {
        public int FirstRow { get; set; }
        public int LastRow { get; set; }
        public int FirstCol{ get; set; }
        public int LastCol { get; set; }

    }
}