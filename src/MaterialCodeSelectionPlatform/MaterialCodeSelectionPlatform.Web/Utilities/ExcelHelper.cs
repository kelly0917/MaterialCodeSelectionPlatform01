using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using log4net;
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
            IRow titleRow = sheet.GetRow(titleRowIndex);
            int rowCount = sheet.LastRowNum;//总行数
            int cellCount = titleRow.Cells.Count;//一行最后一个cell的编号 即总的列数
            //var dict = GetRefColumnDic(referenceNameList, sheet.SheetName, ref startRow);
            if (dataList != null && dataList.Count > 0)
            {               
                ICellStyle style = workbook.CreateCellStyle();
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                foreach (var item in dataList)
                {
                    Type t = item.GetType();
                    IRow row = sheet.CreateRow(startRow++);
                    for (var i = 0; i < cellCount; i++)
                    {
                        var cell = row.CreateCell(i);
                        cell.CellStyle = style;
                        if (dict.ContainsKey(i.ToString()))
                        {
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
                            cell.SetCellValue("");
                        }
                    }
                }
            }
        }
        private static void setExcelTemplateValue(ISheet sheet, List<Domain.PartNumberReport> dataList)
        {
            int rowCount = sheet.LastRowNum;//总行数
            for (var i = 0; i < rowCount; i++)
            {
                var row = sheet.GetRow(i);
                var cellCount = row.Cells.Count;
                for (var j = 0; j < cellCount; j++)
                {
                    var cell = row.Cells[j];
                    var cellString = cell?.ToString();
                    if (!string.IsNullOrEmpty(cellString)&& cellString.IndexOf("$")>-1)
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
        private static string getColumnString<T>(T item,string cellString)
        {
            cellString = cellString.ToLower();
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
                        if (cellString.IndexOf(name) > -1)
                        {
                            cellString = cellString.Replace(name, value.ToString());
                        }
                    }
                }
            }
            return cellString;
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
                                //dic.Add(key, refName.NameName.ToLower());
                                dic.Add(key, refName.NameName.ToLower().Replace("p_",""));//去掉固定的标识 
                            }
                            if (startRow < rowIndex)
                            {
                                startRow = rowIndex;
                            }
                        }
                    }
                }
            }
          //  startRow = startRow + 1;//移下一行
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
                if (!string.IsNullOrEmpty(sheetName))
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
                    if (name.RefersToFormula.IndexOf(":") < 0)
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
    }
}