using NPOI.HSSF.UserModel;
using NPOI.SS.Converter;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Data;

namespace DShop.Infrastructure
{
    public class NPOIImageAnchor
    {
        public int Row1 { get; set; }
        public int Col1 { get; set; }
        public int Row2 { get; set; }
        public int Col2 { get; set; }
        public int Dx1 { get; set; }
        public int Dy1 { get; set; }
        public int Dx2 { get; set; }
        public int Dy2 { get; set; }
        public NPOIAnchorType AnchorType { get; set; }

        public NPOIImageAnchor()
        {
            Dx1 = 0;
            Dy1 = 0;
            Dx2 = 0;
            Dy2 = 0;
        }
    }

    public enum NPOIAnchorType : int
    {
        MoveAndResize = 0,
        MoveDontResize = 2,
        DontMoveAndResize = 3
    }

    public class NPOIName
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string NameName { get; set; }
        /// <summary>
        /// 引用公式
        /// </summary>
        public string RefersToFormula { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 是否函数
        /// </summary>
        public bool IsFunctionName { get; set; }
        /// <summary>
        /// sheet名称
        /// </summary>
        public string SheetName { get; set; }
    }

    public class NPOICellReference
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }


    /// <summary>
    /// 封装的NPOI方法
    /// Shibo
    /// Version: 11
    /// 2022-10-20
    /// </summary>
    public class NPOIExcelHelper : IDisposable
    {
        #region 静态方法

        #region 读取Excel到DataTable
        /// <summary>
        /// 读取excel数据到datatable
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <returns></returns>
        public static DataTable ReadExcelFromFilePath(string filePath)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return ReadExcelFromStream(fs);
            }
            catch { throw; }
            finally
            {
                if (null != fs) fs.Close();
            }
        }

        /// <summary>
        /// 读取excel数据到datatable
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <param name="sheetName">要读取的sheet页名称</param>
        /// <returns></returns>
        public static DataTable ReadExcelFromFilePath(string filePath, string sheetName)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return ReadExcelFromStream(fs, sheetName);
            }
            catch { throw; }
            finally
            {
                if (null != fs) fs.Close();
            }
        }

        /// <summary>
        /// 读取excel数据到datatable
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <param name="sheetName">要读取的sheet页名称</param>
        /// <param name="iColumnNameRowIndex">标题行号（从0开始，默认为0，-1表示不要标题行）</param>
        /// <param name="iDataRowIndex">数据开始的行号（从0开始，默认为1）</param>
        /// <returns></returns>
        public static DataTable ReadExcelFromFilePath(string filePath, string sheetName, int iColumnNameRowIndex, int iDataRowIndex)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return ReadExcelFromStream(fs, sheetName, iColumnNameRowIndex, iDataRowIndex);
            }
            catch { throw; }
            finally
            {
                if (null != fs) fs.Close();
            }
        }

        /// <summary>
        /// 读取excel数据到datatable
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <param name="sheetIndex">要读取的sheet页索引号（从0开始）</param>
        /// <returns></returns>
        public static DataTable ReadExcelFromFilePath(string filePath, int sheetIndex)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return ReadExcelFromStream(fs, sheetIndex);
            }
            catch { throw; }
            finally
            {
                if (null != fs) fs.Close();
            }
        }

        /// <summary>
        /// 读取excel数据到datatable
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <param name="sheetIndex">要读取的sheet页索引号（从0开始）</param>
        /// <param name="iColumnNameRowIndex">标题行号（从0开始，默认为0，-1表示不要标题行）</param>
        /// <param name="iDataRowIndex">数据开始的行号（从0开始，默认为1）</param>
        /// <returns></returns>
        public static DataTable ReadExcelFromFilePath(string filePath, int sheetIndex, int iColumnNameRowIndex, int iDataRowIndex)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return ReadExcelFromStream(fs, sheetIndex, iColumnNameRowIndex, iDataRowIndex);
            }
            catch { throw; }
            finally
            {
                if (null != fs) fs.Close();
            }
        }

        /// <summary>
        /// 读取excel数据到datatable
        /// </summary>
        /// <param name="fileStream">excel文件流</param>
        /// <returns></returns>
        public static DataTable ReadExcelFromStream(Stream fileStream)
        {
            try
            {
                IWorkbook workbook = WorkbookFactory.Create(fileStream);
                return ReadExcelFromWorkbook(workbook, 0);
            }
            catch { throw; }
        }

        /// <summary>
        /// 读取excel数据到datatable
        /// </summary>
        /// <param name="fileStream">excel文件流</param>
        /// <param name="sheetName">要读取的sheet页名称</param>
        /// <returns></returns>
        public static DataTable ReadExcelFromStream(Stream fileStream, string sheetName)
        {
            try
            {
                IWorkbook workbook = WorkbookFactory.Create(fileStream);
                int sheetIndex = string.IsNullOrEmpty(sheetName) ? 0 : workbook.GetSheetIndex(sheetName);
                return ReadExcelFromWorkbook(workbook, sheetIndex);
            }
            catch { throw; }
        }

        /// <summary>
        /// 读取excel数据到datatable
        /// </summary>
        /// <param name="fileStream">excel文件流</param>
        /// <param name="sheetName">要读取的sheet页名称</param>
        /// <param name="iColumnNameRowIndex">标题行号（从0开始，默认为0，-1表示不要标题行）</param>
        /// <param name="iDataRowIndex">数据开始的行号（从0开始，默认为1）</param>
        /// <returns></returns>
        public static DataTable ReadExcelFromStream(Stream fileStream, string sheetName, int iColumnNameRowIndex, int iDataRowIndex)
        {
            try
            {
                IWorkbook workbook = WorkbookFactory.Create(fileStream);
                int sheetIndex = string.IsNullOrEmpty(sheetName) ? 0 : workbook.GetSheetIndex(sheetName);
                return ReadExcelFromWorkbook(workbook, sheetIndex, iColumnNameRowIndex, iDataRowIndex);
            }
            catch { throw; }
        }

        /// <summary>
        /// 读取excel数据到datatable
        /// </summary>
        /// <param name="fileStream">excel文件流</param>
        /// <param name="sheetIndex">要读取的sheet页索引号（从0开始）</param>
        /// <returns></returns>
        public static DataTable ReadExcelFromStream(Stream fileStream, int sheetIndex)
        {
            try
            {
                IWorkbook workbook = WorkbookFactory.Create(fileStream);
                return ReadExcelFromWorkbook(workbook, sheetIndex);
            }
            catch { throw; }
        }

        /// <summary>
        /// 读取excel数据到datatable
        /// </summary>
        /// <param name="fileStream">excel文件流</param>
        /// <param name="sheetIndex">要读取的sheet页索引号（从0开始）</param>
        /// <param name="iColumnNameRowIndex">标题行号（从0开始，默认为0，-1表示不要标题行）</param>
        /// <param name="iDataRowIndex">数据开始的行号（从0开始，默认为1）</param>
        /// <returns></returns>
        public static DataTable ReadExcelFromStream(Stream fileStream, int sheetIndex, int iColumnNameRowIndex, int iDataRowIndex)
        {
            try
            {
                IWorkbook workbook = WorkbookFactory.Create(fileStream);
                return ReadExcelFromWorkbook(workbook, sheetIndex, iColumnNameRowIndex, iDataRowIndex);
            }
            catch { throw; }
        }

        /// <summary>
        /// 读取excel数据到datatable
        /// </summary>
        /// <param name="workbook">npoi workbook，一个excel文件对象</param>
        /// <param name="sheetIndex">要读取的sheet页索引号（从0开始）</param>
        /// <param name="iColumnNameRowIndex">标题行号（从0开始，默认为0，-1表示不要标题行）</param>
        /// <param name="iDataRowIndex">数据开始的行号（从0开始，默认为1）</param>
        /// <returns></returns>
        private static DataTable ReadExcelFromWorkbook(IWorkbook workbook, int sheetIndex = 0, int iColumnNameRowIndex = 0, int iDataRowIndex = 1)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            if (null == sheet)
            {
                throw new Exception("没有找到sheet");
            }
            if (sheet.LastRowNum < 1)
            {
                throw new Exception("excel没有数据");
            }

            DataTable dt = new DataTable();
            //有标题行
            //IList<int> colIndexList = new List<int>();
            List<string> colNameList = new List<string>();
            if (iColumnNameRowIndex >= 0)
            {
                IRow firstRow = sheet.GetRow(iColumnNameRowIndex);
                for (int i = 0; i < firstRow.LastCellNum; i++)
                {
                    ICell cell = firstRow.GetCell(i);
                    if (null == cell)
                    {
                        continue;
                    }
                    string colName = cell.ToString();
                    int count = colNameList.Count(x => x == colName);
                    dt.Columns.Add(colName + (count > 0 ? "_" + count : string.Empty), typeof(string));
                    colNameList.Add(colName);
                    //colIndexList.Add(i);
                }
            }
            //else
            //{
            //    IRow firstRow = sheet.GetRow(0);
            //    for (int i = 0; i < firstRow.LastCellNum; i++)
            //    {
            //        ICell cell = firstRow.GetCell(i);
            //        if (null == cell)
            //        {
            //            continue;
            //        }
            //        dt.Columns.Add("Column" + i, typeof(string));
            //        colIndexList.Add(i);
            //    }
            //}

            //int totalColNum = colIndexList.Count;//总列数

            for (int i = iDataRowIndex; i <= sheet.LastRowNum; i++)
            {
                IRow dRow = sheet.GetRow(i);
                if (null != dRow)
                {
                    object[] arrRow = new object[dRow.LastCellNum + 1];
                    for (int k = 0; k < dRow.LastCellNum + 1; k++)
                    {
                        if (k >= dt.Columns.Count)
                        {
                            dt.Columns.Add("Column" + k, typeof(string));
                        }
                        ICell dCell = dRow.GetCell(k);

                        IFormulaEvaluator eva = WorkbookFactory.CreateFormulaEvaluator(workbook);
                        object cellValue = GetCellValue(dCell, eva);

                        arrRow[k] = null == cellValue ? null : cellValue.ToString();
                    }
                    dt.Rows.Add(arrRow);
                }
            }
            return dt;
        }
        #endregion

        /// <summary>
        /// 将Excel文件转换成Html文档
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <returns></returns>
        public static MemoryStream ExcelToHtml(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new Exception("文件路径不能为空");
                }
                string suffix = filePath.Substring(filePath.LastIndexOf('.') + 1);
                if (suffix.ToLower() != "xls" && suffix.ToLower() != "xlsx")
                {
                    throw new Exception("文件不是excel文档");
                }
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return ExcelToHtml(fs);
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// 将Excel文件转换成Html文档
        /// </summary>
        /// <param name="fileStream">Excel文件流</param>
        /// <returns></returns>
        public static MemoryStream ExcelToHtml(Stream fileStream)
        {
            try
            {
                IWorkbook workbook = WorkbookFactory.Create(fileStream);
                workbook.RemoveSheetAt(2);
                workbook.RemoveSheetAt(1);
                ExcelToHtmlConverter converter = new ExcelToHtmlConverter();
                converter.OutputColumnHeaders = false;
                converter.OutputHiddenColumns = false;
                converter.OutputHiddenRows = false;
                converter.OutputLeadingSpacesAsNonBreaking = false;
                converter.OutputRowNumbers = false;
                converter.UseDivsToSpan = false;
                converter.ProcessWorkbook(workbook);

                MemoryStream ms = new MemoryStream();
                converter.Document.Save(ms);
                ms.Position = 0;
                return ms;
            }
            catch { throw; }
        }

        /// <summary>
        /// DataTable 写入Excel文件
        /// </summary>
        /// <param name="targetFilePath">Excel文件路径</param>
        /// <param name="dt">要写入的数据表</param>
        /// <param name="colNames">要写入的列的列名</param>
        /// <param name="sheetIndex">要写入的sheet页编号</param>
        /// <param name="startRowIndex">开始的行号（0开始）</param>
        /// <param name="startColIndex">开始的列号（0开始）</param>
        public static void ExportExcelToFile(string targetFilePath, DataTable dt, string[] colNames = null, int sheetIndex = 0, int startRowIndex = 0, int startColIndex = 0)
        {
            FileInfo targetFile = new FileInfo(targetFilePath);
            if (!targetFile.Directory.Exists)
            {
                targetFile.Directory.Create();
            }
            using (FileStream targetFileStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                ExportExcelToStream(targetFileStream, dt, colNames, sheetIndex, startRowIndex, startColIndex);
            }
        }

        /// <summary>
        /// DataTable 写入Excel文件
        /// </summary>
        /// <param name="templateFilePath"></param>
        /// <param name="targetFilePath"></param>
        /// <param name="dt"></param>
        /// <param name="colNames"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="startColIndex"></param>
        public static void ExportExcelToFile(string templateFilePath, string targetFilePath, DataTable dt, string[] colNames = null, int sheetIndex = 0, int startRowIndex = 0, int startColIndex = 0)
        {
            FileInfo targetFile = new FileInfo(targetFilePath);
            if (!targetFile.Directory.Exists)
            {
                targetFile.Directory.Create();
            }

            using (FileStream targetFileStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                ExportExcelToStream(templateFilePath, targetFileStream, dt, colNames, sheetIndex, startRowIndex, startColIndex);
            }
        }

        /// <summary>
        /// DataTable 写入Stream
        /// </summary>
        /// <param name="templateFilePath">模板文件路径</param>
        /// <param name="dt">要写入的数据表</param>
        /// <param name="colNames">要写入的列的列名</param>
        /// <param name="sheetIndex">要写入的sheet页编号</param>
        /// <param name="startRowIndex">开始的行号（0开始）</param>
        /// <param name="startColIndex">开始的列号（0开始）</param>
        public static void ExportExcelToStream(string templateFilePath, Stream targetStream, DataTable dt, string[] colNames = null, int sheetIndex = 0, int startRowIndex = 0, int startColIndex = 0)
        {
            try
            {
                if (!File.Exists(templateFilePath))
                {
                    throw new Exception("文件不存在：" + templateFilePath);
                }

                string postfix = templateFilePath.Substring(templateFilePath.LastIndexOf('.'));
                if (string.IsNullOrEmpty(postfix) || ".xls" != postfix.ToLower() && ".xlsx" != postfix.ToLower())
                {
                    throw new Exception("模板文件后缀名只能是xls或者xlsx");
                }

                using (MemoryStream msSource = new MemoryStream())
                {
                    using (FileStream fs = File.OpenRead(templateFilePath))
                    {
                        fs.CopyTo(msSource);
                        msSource.Position = 0;

                        IWorkbook workbook = WorkbookFactory.Create(msSource);
                        ExportExcelToWorkbook(workbook, dt, colNames, sheetIndex, startRowIndex, startColIndex);
                        workbook.Write(targetStream);
                        targetStream.Position = 0;
                    }
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// DataTable 写入Stream
        /// </summary>
        /// <param name="excelVersion"></param>
        /// <param name="targetStream"></param>
        /// <param name="dt"></param>
        /// <param name="colNames"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="startColIndex"></param>
        public static void ExportExcelToStream(Stream targetStream, DataTable dt, string[] colNames = null, int sheetIndex = 0, int startRowIndex = 0, int startColIndex = 0)
        {
            IWorkbook workbook = WorkbookFactory.Create(targetStream);
            ExportExcelToWorkbook(workbook, dt, colNames, sheetIndex, startRowIndex, startColIndex);
            workbook.Write(targetStream);
            targetStream.Position = 0;
        }

        /// <summary>
        /// DataTable 写入Excel
        /// </summary>
        /// <param name="workbook">Excel文件（null表示新增excel文件）</param>
        /// <param name="dt">要写入的数据表</param>
        /// <param name="colNames">要写入的列的列名</param>
        /// <param name="sheetIndex">要写入的sheet页编号</param>
        /// <param name="startRowIndex">开始的行号（0开始）</param>
        /// <param name="startColIndex">开始的列号（0开始）</param>
        private static void ExportExcelToWorkbook(IWorkbook workbook, DataTable dt, string[] colNames = null, int sheetIndex = 0, int startRowIndex = 0, int startColIndex = 0)
        {
            try
            {
                if (null == dt || dt.Rows.Count == 0)
                {
                    return;
                }
                if (null == colNames)
                {
                    colNames = new string[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        colNames[i] = dt.Columns[i].ColumnName;
                    }
                }
                else
                {
                    foreach (var colName in colNames)
                    {
                        if (!dt.Columns.Contains(colName))
                        {
                            throw new Exception("数据源不包含列：" + colName);
                        }
                    }
                }

                int iRowCount = dt.Rows.Count;
                int iColCount = colNames.Length;

                //多线程处理
                ISheet sheet = workbook.NumberOfSheets > sheetIndex ? workbook.GetSheetAt(sheetIndex) : workbook.CreateSheet();
                int iThreadCount = 1;
                Task[] tasks = new Task[iThreadCount];
                for (var i = 0; i < iThreadCount; i++)
                {
                    tasks[i] = new Task(oIndex =>
                    {
                        int threadIndex = (int)oIndex;
                        for (var k = threadIndex; k < iRowCount; k = k + iThreadCount)
                        {
                            int rowIndex = k + startRowIndex;
                            IRow row = sheet.GetRow(rowIndex);
                            if (null == row)
                            {
                                row = sheet.CreateRow(rowIndex);
                            }
                            for (int m = 0; m < iColCount; m++)
                            {
                                int colIndex = m + startColIndex;
                                ICell cell = row.GetCell(colIndex);
                                if (null == cell)
                                {
                                    cell = row.CreateCell(colIndex);
                                }
                                cell.SetCellValue(Convert.ToString(dt.Rows[k][colNames[m]]));
                            }
                        }
                    }, i);
                    tasks[i].Start();
                }
                Task.WaitAll(tasks);
            }
            catch (AggregateException ex)
            {
                string msg = string.Empty;
                foreach (var iex in ex.InnerExceptions)
                {
                    msg += "    " + iex.Message + "\r\n    " + iex.StackTrace + "\r\n";
                }
                throw new Exception("发生多个错误：\r\n" + msg);
            }
            catch { throw; }
        }

        /// <summary>
        /// 在最后追加数据
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="dt"></param>
        private static void NewRows(IWorkbook workbook, int sheetIndex, DataTable dt)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            if (null == sheet)
            {
                throw new Exception("没有找到sheet");
            }


        }

        private static object GetCellValue(ICell cell, IFormulaEvaluator formulaEvaluator)
        {
            if (null == cell)
            {
                return null;
            }

            object objValue = null;
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        objValue = cell.DateCellValue?.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        objValue = cell.NumericCellValue;
                    }
                    break;
                case CellType.String:
                    objValue = cell.StringCellValue;
                    break;
                case CellType.Formula:
                    if (null == formulaEvaluator)
                    {
                        objValue = cell.StringCellValue;
                    }
                    else
                    {
                        cell = formulaEvaluator.EvaluateInCell(cell);
                        objValue = GetCellValue(cell, formulaEvaluator);
                    }
                    break;
                case CellType.Boolean:
                    objValue = cell.BooleanCellValue;
                    break;
                case CellType.Blank:
                case CellType.Error:
                default:
                    break;
            }

            return objValue;
        }


        public static byte[] InsertRowBelowAndFillData(Stream ms, int sheetIndex, int targetRowIndex)
        {
            // 1. 从流加载工作簿
            HSSFWorkbook workbook = new HSSFWorkbook(ms);

            // 2. 获取指定工作表
            ISheet sheet = workbook.GetSheetAt(sheetIndex);

            // 3. 处理行索引越界
            if (targetRowIndex < 0)
                targetRowIndex = 0;
            else if (targetRowIndex > sheet.LastRowNum)
                targetRowIndex = sheet.LastRowNum; // 在最后追加


            // 4. 要插入的行数（这里假设插入一行，可根据需要修改）
            int insertCount = 100;

            // 5. 移动下方行
            if (targetRowIndex < sheet.LastRowNum)
            {
                sheet.ShiftRows(targetRowIndex, sheet.LastRowNum, insertCount);
            }

            ICellStyle borderedStyle = workbook.CreateCellStyle();
            borderedStyle.BorderTop = BorderStyle.Thin;
            borderedStyle.BorderBottom = BorderStyle.Thin;
            borderedStyle.BorderLeft = BorderStyle.Thin;
            borderedStyle.BorderRight = BorderStyle.Thin;
            borderedStyle.TopBorderColor = IndexedColors.Black.Index;
            borderedStyle.BottomBorderColor = IndexedColors.Black.Index;
            borderedStyle.LeftBorderColor = IndexedColors.Black.Index;
            borderedStyle.RightBorderColor = IndexedColors.Black.Index;
            borderedStyle.Alignment = HorizontalAlignment.Center;
            borderedStyle.VerticalAlignment = VerticalAlignment.Center;

            // 7. 循环创建新行并填充数据
            for (int i = 0; i < insertCount; i++)
            {
                int newRowIndex = targetRowIndex + i;
                IRow newRow = sheet.CreateRow(newRowIndex);

                // 假设填充三列，为每个单元格设置边框样式
                ICell cell0 = newRow.CreateCell(0);
                cell0.SetCellValue("新插入的行");
                cell0.CellStyle = borderedStyle;

                ICell cell1 = newRow.CreateCell(1);
                cell1.SetCellValue(123.45);
                cell1.CellStyle = borderedStyle;

                ICell cell2 = newRow.CreateCell(2);
                cell2.SetCellValue(true);
                cell2.CellStyle = borderedStyle;
            }

            // 8. 保存到内存流并返回字节数组
            using (MemoryStream outMemoryStream = new MemoryStream())
            {
                workbook.Write(outMemoryStream);
                return outMemoryStream.ToArray();
            }
        }


        public static byte[] InsertRowBelowHeader(Stream ms, int sheetIndex, DataTable dataTable)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(ms);
            ISheet sheet = workbook.GetSheetAt(sheetIndex);

            string[] keywords = { "计量仪器名称", "Name" };
            int targetRowIndex = FindTargetRow(sheet, keywords);
            if (targetRowIndex == -1)
                throw new Exception("未找到包含所有关键词的行，无法插入数据。");

            int insertCount = dataTable.Rows.Count;

            // --- 合并区域处理 ---
            // 收集所有起始行在目标行下方的合并区域
            List<CellRangeAddress> movingRegions = new List<CellRangeAddress>();
            for (int i = sheet.NumMergedRegions - 1; i >= 0; i--)
            {
                CellRangeAddress region = sheet.GetMergedRegion(i);
                if (region.FirstRow >= targetRowIndex + 1) // 完全位于移动范围内
                {
                    movingRegions.Add(region);
                    sheet.RemoveMergedRegion(i);
                }
                // 如果需要处理跨越目标行的区域，可在此扩展逻辑
            }

            // --- 行移动（您的原代码，从后往前复制并删除）---
            int lastRow = sheet.LastRowNum;
            for (int r = lastRow; r >= targetRowIndex + 1; r--)
            {
                IRow sourceRow = sheet.GetRow(r);
                if (sourceRow != null)
                {
                    IRow newRow = sheet.CreateRow(r + insertCount);
                    CopyRow(sourceRow, newRow, sheet);
                    sheet.RemoveRow(sourceRow);
                }
                else
                {
                    sheet.CreateRow(r + insertCount);
                }
            }

            // --- 重新添加调整后的合并区域 ---
            foreach (CellRangeAddress region in movingRegions)
            {
                // 创建新的合并区域，行索引下移 insertCount
                CellRangeAddress newRegion = new CellRangeAddress(
                    region.FirstRow + insertCount,
                    region.LastRow + insertCount,
                    region.FirstColumn,
                    region.LastColumn
                );
                sheet.AddMergedRegion(newRegion);
            }

            // --- 创建带黑色边框的样式 ---
            ICellStyle borderedStyle = workbook.CreateCellStyle();
            borderedStyle.BorderTop = BorderStyle.Thin;
            borderedStyle.BorderBottom = BorderStyle.Thin;
            borderedStyle.BorderLeft = BorderStyle.Thin;
            borderedStyle.BorderRight = BorderStyle.Thin;
            borderedStyle.TopBorderColor = IndexedColors.Black.Index;
            borderedStyle.BottomBorderColor = IndexedColors.Black.Index;
            borderedStyle.LeftBorderColor = IndexedColors.Black.Index;
            borderedStyle.RightBorderColor = IndexedColors.Black.Index;
            borderedStyle.Alignment = HorizontalAlignment.Center;
            borderedStyle.VerticalAlignment = VerticalAlignment.Center;

            // --- 插入新数据行 ---
            for (int i = 0; i < insertCount; i++)
            {
                int newRowIndex = targetRowIndex + i + 1;
                IRow newRow = sheet.GetRow(newRowIndex);
                if (newRow == null) newRow = sheet.CreateRow(newRowIndex);

                //ICell cell0 = newRow.CreateCell(0);
                //cell0.SetCellValue(i.ToString());
                //cell0.CellStyle = borderedStyle;

                //ICell cell1 = newRow.CreateCell(1);
                //cell1.SetCellValue("测试设备");
                //cell1.CellStyle = borderedStyle;

                //ICell cell2 = newRow.CreateCell(2);
                //cell2.SetCellValue(true);
                //cell2.CellStyle = borderedStyle;

                DataRow dataRow = dataTable.Rows[i];
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    string value = dataRow[j].ToString();
                    ICell cell = newRow.CreateCell(j);
                    cell.SetCellValue(value);
                    cell.CellStyle = borderedStyle;
                }
            }

            using (MemoryStream outMemoryStream = new MemoryStream())
            {
                workbook.Write(outMemoryStream);
                return outMemoryStream.ToArray();
            }
        }


        public static byte[] InsertRowBelowHeader(byte[]? data, decimal totalAmount, int sheetIndex, DataTable dataTable)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(new MemoryStream(data));
            ISheet sheet = workbook.GetSheetAt(sheetIndex);

            string[] keywords = { "计量仪器名称", "Name" };
            int targetRowIndex = FindTargetRow(sheet, keywords);
            if (targetRowIndex == -1)
                throw new Exception("未找到包含所有关键词的行，无法插入数据。");

            const int amountColumnIndex = 5; // 假设金额在第6列（索引从0开始）
            IRow row = sheet.GetRow(targetRowIndex + 1);
            ICell cell = row.GetCell(amountColumnIndex);
            cell.SetCellValue(totalAmount.ToString("F2"));

            int insertCount = dataTable.Rows.Count;

            // --- 合并区域处理 ---
            // 收集所有起始行在目标行下方的合并区域
            List<CellRangeAddress> movingRegions = new List<CellRangeAddress>();
            for (int i = sheet.NumMergedRegions - 1; i >= 0; i--)
            {
                CellRangeAddress region = sheet.GetMergedRegion(i);
                if (region.FirstRow >= targetRowIndex + 1) // 完全位于移动范围内
                {
                    movingRegions.Add(region);
                    sheet.RemoveMergedRegion(i);
                }
                // 如果需要处理跨越目标行的区域，可在此扩展逻辑
            }

            // --- 行移动（您的原代码，从后往前复制并删除）---
            int lastRow = sheet.LastRowNum;
            for (int r = lastRow; r >= targetRowIndex + 1; r--)
            {
                IRow sourceRow = sheet.GetRow(r);
                if (sourceRow != null)
                {
                    IRow newRow = sheet.CreateRow(r + insertCount);
                    CopyRow(sourceRow, newRow, sheet);
                    sheet.RemoveRow(sourceRow);
                }
                else
                {
                    sheet.CreateRow(r + insertCount);
                }
            }

            // --- 重新添加调整后的合并区域 ---
            foreach (CellRangeAddress region in movingRegions)
            {
                // 创建新的合并区域，行索引下移 insertCount
                CellRangeAddress newRegion = new CellRangeAddress(
                    region.FirstRow + insertCount,
                    region.LastRow + insertCount,
                    region.FirstColumn,
                    region.LastColumn
                );
                sheet.AddMergedRegion(newRegion);
            }

            // --- 创建带黑色边框的样式 ---
            ICellStyle borderedStyle = workbook.CreateCellStyle();
            borderedStyle.BorderTop = BorderStyle.Thin;
            borderedStyle.BorderBottom = BorderStyle.Thin;
            borderedStyle.BorderLeft = BorderStyle.Thin;
            borderedStyle.BorderRight = BorderStyle.Thin;
            borderedStyle.TopBorderColor = IndexedColors.Black.Index;
            borderedStyle.BottomBorderColor = IndexedColors.Black.Index;
            borderedStyle.LeftBorderColor = IndexedColors.Black.Index;
            borderedStyle.RightBorderColor = IndexedColors.Black.Index;
            borderedStyle.Alignment = HorizontalAlignment.Center;
            borderedStyle.VerticalAlignment = VerticalAlignment.Center;

            // --- 插入新数据行 ---
            for (int i = 0; i < insertCount; i++)
            {
                int newRowIndex = targetRowIndex + i + 1;
                IRow newRow = sheet.GetRow(newRowIndex);
                if (newRow == null) newRow = sheet.CreateRow(newRowIndex);
                DataRow dataRow = dataTable.Rows[i];
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    string value = dataRow[j].ToString();
                    ICell newCell = newRow.CreateCell(j);
                    newCell.SetCellValue(value);
                    newCell.CellStyle = borderedStyle;
                }
            }

            using (MemoryStream outMemoryStream = new MemoryStream())
            {
                workbook.Write(outMemoryStream);
                return outMemoryStream.ToArray();
            }
        }

        /// <summary>
        /// 查找同时包含所有关键词的行，若该行属于合并区域则返回合并区域的最后一行
        /// </summary>
        private static int FindTargetRow(ISheet sheet, string[] keywords)
        {
            int targetRow = -1;
            int maxRow = -1;
            for (int rowIdx = 0; rowIdx <= sheet.LastRowNum; rowIdx++)
            {
                IRow row = sheet.GetRow(rowIdx);
                if (row == null) continue;

                bool allFound = true;
                foreach (string keyword in keywords)
                {
                    bool found = false;
                    foreach (ICell cell in row.Cells)
                    {
                        string cellValue = cell.ToString();
                        if (!string.IsNullOrEmpty(cellValue) &&
                            cellValue.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            // 若单元格为合并区域的一部分，记录合并区域最后一行
                            CellRangeAddress mergedRegion = GetMergedRegion(sheet, cell.RowIndex, cell.ColumnIndex);
                            if (mergedRegion != null)
                            {
                                maxRow = Math.Max(maxRow, mergedRegion.LastRow);
                            }
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        allFound = false;
                        break;
                    }
                }
                if (allFound)
                {
                    targetRow = maxRow; // 使用合并区域最后一行（若无合并则 maxRow 可能为 -1，但至少 targetRow 应为 rowIdx）
                    if (targetRow == -1) targetRow = rowIdx;
                    break;
                }
            }
            return targetRow;
        }

        /// <summary>
        /// 获取单元格所在的合并区域，若不属于任何合并区域则返回 null
        /// </summary>
        private static CellRangeAddress GetMergedRegion(ISheet sheet, int row, int col)
        {
            for (int i = 0; i < sheet.NumMergedRegions; i++)
            {
                CellRangeAddress region = sheet.GetMergedRegion(i);
                if (region.FirstRow <= row && region.LastRow >= row &&
                    region.FirstColumn <= col && region.LastColumn >= col)
                {
                    return region;
                }
            }
            return null;
        }

        /// <summary>
        /// 复制行的所有单元格内容、样式、公式等到目标行
        /// </summary>
        private static void CopyRow(IRow sourceRow, IRow targetRow, ISheet sheet)
        {
            // 复制行高（可选）
            targetRow.Height = sourceRow.Height;

            // 遍历源行所有单元格
            foreach (ICell sourceCell in sourceRow.Cells)
            {
                int colIndex = sourceCell.ColumnIndex;
                ICell targetCell = targetRow.CreateCell(colIndex);

                // 复制单元格样式
                targetCell.CellStyle = sourceCell.CellStyle;

                // 复制单元格类型和值
                switch (sourceCell.CellType)
                {
                    case CellType.Numeric:
                        targetCell.SetCellValue(sourceCell.NumericCellValue);
                        break;
                    case CellType.String:
                        targetCell.SetCellValue(sourceCell.StringCellValue);
                        break;
                    case CellType.Boolean:
                        targetCell.SetCellValue(sourceCell.BooleanCellValue);
                        break;
                    case CellType.Formula:
                        targetCell.SetCellFormula(sourceCell.CellFormula);
                        // 公式可能依赖其他单元格，无需额外处理
                        break;
                    case CellType.Blank:
                        targetCell.SetCellType(CellType.Blank);
                        break;
                    case CellType.Error:
                        targetCell.SetCellErrorValue(sourceCell.ErrorCellValue);
                        break;
                    default:
                        // 其他类型忽略或根据需求处理
                        break;
                }

                // 复制单元格注释（如果有）
                if (sourceCell.CellComment != null)
                {
                    targetCell.CellComment = sourceCell.CellComment;
                }

                // 复制超链接（如果有）
                if (sourceCell.Hyperlink != null)
                {
                    targetCell.Hyperlink = sourceCell.Hyperlink;
                }
            }

            // 注意：合并区域未自动调整，需自行处理。
            // 如果希望合并区域随行移动，需要遍历所有合并区域并调整受影响的区域。
            // 由于当前需求未涉及，此处省略。
        }
        #endregion


        #region 实例方法
        /// <summary>
        /// 当前处理的文档，调用实例方法前，先Load模板文件或者Create新的文档
        /// </summary>
        private IWorkbook workbook = null;
        private bool disposedValue;

        #region 新建
        /// <summary>
        /// 新建Excel文件，默认xlsx格式
        /// </summary>
        public void Create()
        {
            CreateNewXSSFWorkbook();
        }

        /// <summary>
        /// 新建xls格式的excel文件
        /// </summary>
        public void CreateNewHSSFWorkbook()
        {
            Close();
            workbook = new HSSFWorkbook();
        }

        /// <summary>
        /// 新建xlsx格式的excel文件
        /// </summary>
        public void CreateNewXSSFWorkbook()
        {
            Close();
            workbook = new XSSFWorkbook();
        }
        #endregion

        #region 打开
        public void Open(Stream stream)
        {
            workbook = WorkbookFactory.Create(stream);
        }

        public void Open(string filePath)
        {
            workbook = WorkbookFactory.Create(filePath);
        }
        #endregion

        public void CopyAndOpen(Stream stream)
        {
            Stream newStream = new MemoryStream();
            stream.CopyTo(newStream);
            newStream.Position = 0;
            workbook = WorkbookFactory.Create(newStream);
        }

        #region 计算所用公式
        /// <summary>
        /// 计算所有公式
        /// </summary>
        public void EvaluateAllFormula()
        {
            workbook.GetCreationHelper().CreateFormulaEvaluator().EvaluateAll();
            //workbook.GetCreationHelper().CreateFormulaEvaluator().EvaluateAllFormulaCells(new XSSFDataFormatter());
        }
        public void EvaluateAllFormula(IWorkbook wookbook)
        {
            foreach (var sheetItem in GetAllSheetNameList())
            {
                //if (sheetItem=="SaveData"||sheetItem=="SampleData")
                //{
                //    continue;
                //}
                ISheet sheet = wookbook.GetSheet(sheetItem);
                Dictionary<ICell, string> dateCellFormats = new Dictionary<ICell, string>();
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    foreach (ICell cell in row)
                    {
                        if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                        {
                            dateCellFormats.Add(cell, cell.CellStyle.GetDataFormatString());
                        }
                    }
                }
                workbook.GetCreationHelper().CreateFormulaEvaluator().EvaluateAll();
                foreach (var cellFormatPair in dateCellFormats)
                {
                    ICell cell = cellFormatPair.Key;
                    string formatString = cellFormatPair.Value;
                    cell.CellStyle = workbook.CreateCellStyle();
                    cell.CellStyle.CloneStyleFrom(cell.CellStyle);
                    cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat(formatString);
                }
            }

        }

        public void EvaluateAllFormula(string sheetName)
        {
            int sheetIndex = string.IsNullOrEmpty(sheetName) ? 0 : workbook.GetSheetIndex(sheetName);
            if (sheetIndex < 0)
            {
                return;
            }
            EvaluateAllFormula(sheetIndex);
        }

        public void EvaluateAllFormula(int sheetIndex)
        {
            IFormulaEvaluator formulaEvaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();

            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            if (null == sheet)
            {
                throw new Exception("没有找到sheet");
            }
            if (sheet.LastRowNum < 1)
            {
                throw new Exception("excel没有数据");
            }

            int lastRowNum = sheet.LastRowNum;

            for (int i = 0; i <= lastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                {
                    break;
                }

                int lastCellNum = row.LastCellNum;
                for (int k = 0; k <= lastCellNum; k++)
                {
                    ICell cell = row.GetCell(k);

                    if (cell == null)
                    {
                        break;
                    }

                    if (cell.CellType == CellType.Formula)
                    {
                        try
                        {
                            formulaEvaluator.EvaluateInCell(cell);
                        }
                        catch { }
                    }
                }
            }
        }

        public void EvaluateFormula(string sheetName, int rowIndex, int colIndex)
        {
            int sheetIndex = string.IsNullOrEmpty(sheetName) ? 0 : workbook.GetSheetIndex(sheetName);
            EvaluateFormula(sheetIndex, rowIndex, colIndex);
        }

        public void EvaluateFormula(int sheetIndex, int rowIndex, int colIndex)
        {
            IFormulaEvaluator formulaEvaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();

            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            if (null == sheet)
            {
                throw new Exception("没有找到sheet");
            }
            if (sheet.LastRowNum < 1)
            {
                throw new Exception("excel没有数据");
            }

            IRow row = sheet.GetRow(rowIndex);
            if (row == null)
            {
                return;
            }

            ICell cell = row.GetCell(colIndex);

            if (cell == null)
            {
                return;
            }

            if (cell.CellType == CellType.Formula)
            {
                try
                {
                    formulaEvaluator.EvaluateInCell(cell);
                }
                catch { }
            }
        }

        #endregion

        #region 日期格式
        // 对包含日期的单元格进行重新计算并转换为日期格式
        public void RecalculateAndAdjustDateCells()
        {
            var evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();
            var sheetCount = workbook.NumberOfSheets;
            //for (int i = 0; i < sheetCount; i++)
            //{

            //}
            foreach (var item in GetAllSheetNameList())
            {
                if (item == "SampleData")
                {
                    var sheet = workbook.GetSheet(item);
                    var rows = sheet.GetRowEnumerator();
                    while (rows.MoveNext())
                    {
                        var row = (IRow)rows.Current;
                        foreach (var cell in row.Cells)
                        {
                            if (cell.CellType == CellType.Numeric)
                            {
                                var cellValue = evaluator.Evaluate(cell);
                                if (cellValue is CellValue)
                                {
                                    DateTime? date = GetCellDateValue(cell);
                                    if (date != null)
                                    {
                                        cell.SetCellValue((DateTime)date);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // 获取单元格的日期值并转换为日期格式
        private DateTime? GetCellDateValue(ICell cell)
        {
            double numericValue;
            if (cell.CellType == CellType.Numeric)
            {
                numericValue = cell.NumericCellValue;
            }
            else if (cell.CellType == CellType.Formula)
            {
                numericValue = cell.NumericCellValue;
            }
            else
            {
                return cell?.DateCellValue;
            }
            DateTime? date;
            if (DateUtil.IsCellDateFormatted(cell))
            {
                date = cell.DateCellValue;
            }
            else
            {
                date = DateUtil.GetJavaDate(numericValue);
            }
            return date;
        }
        #endregion

        #region 返回所有sheet页名称集合
        /// <summary>
        /// 返回所有sheet页名称集合
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllSheetNameList()
        {
            List<string> sheetNameList = new List<string>();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                sheetNameList.Add(workbook.GetSheetName(i));
            }

            return sheetNameList;
        }
        #endregion


        #region 新建sheet页
        /// <summary>
        /// 新建sheet页
        /// </summary>
        /// <param name="sheetName"></param>
        public void CreateSheet(string sheetName)
        {
            workbook.CreateSheet(sheetName);
        }
        #endregion


        /// <summary>
        /// 通过sheet页名称获取sheet页索引位置
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public int GetSheetIndex(string sheetName)
        {
            int sheetIndex = string.IsNullOrEmpty(sheetName) ? -1 : workbook.GetSheetIndex(sheetName);

            return sheetIndex;
        }


        public void CopyAndOpen(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                CopyAndOpen(fs);
            }
        }

        #region 给单元格赋值
        public void SetCellValue(int sheetIndex, int rowIndex, int colIndex, string value)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            IRow row = sheet.GetRow(rowIndex);
            if (null == row)
            {
                row = sheet.CreateRow(rowIndex);
            }
            ICell cell = row.GetCell(colIndex);
            if (null == cell)
            {
                cell = row.CreateCell(colIndex);
            }

            cell.SetCellValue(value);
        }

        /// <summary>
        /// 给合并单元格赋值
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <param name="value"></param>
        public void SetMergedCellValue(int sheetIndex, int rowIndex, int colIndex, string value)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            IRow row = sheet.GetRow(rowIndex);
            if (null == row)
            {
                row = sheet.CreateRow(rowIndex);
            }
            ICell cell = row.GetCell(colIndex);
            cell = MergedCell(cell);
            //int rowspan;
            //int colspan;
            //bool result = isMergeCell(sheet,rowIndex+1,colIndex+1,out rowspan,out colspan);
            //if (result==true)
            //{

            //}
            //cell = sheet.GetRow(27).GetCell(3);
            if (null == cell)
            {
                cell = row.CreateCell(colIndex);
            }

            cell.SetCellValue(value);
        }
        #endregion

        /// <summary>
        /// 读取合并单元格的值
        /// </summary>
        /// <param name="cell">查询的单元格</param>
        /// <returns>返回有数值的单元格</returns>
        private ICell MergedCell(ICell cell)
        {
            if (cell.IsMergedCell)//是否是合并单元格
            {
                for (int i = 0; i < cell.Sheet.NumMergedRegions; i++)//遍历所有的合并单元格
                {
                    var cellRange = cell.Sheet.GetMergedRegion(i);
                    if (cell.ColumnIndex >= cellRange.FirstColumn && cell.ColumnIndex <= cellRange.LastColumn
                        && cell.RowIndex >= cellRange.FirstRow && cell.RowIndex <= cellRange.LastRow)//判断查询的单元格是否在合并单元格内
                    {
                        return cell.Sheet.GetRow(cellRange.FirstRow).GetCell(cellRange.FirstColumn);
                    }
                }
            }
            return cell;
        }

        public string GetCellValue(int sheetIndex, int rowIndex, int colIndex)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            IRow row = sheet.GetRow(rowIndex);
            if (null == row)
            {
                return null;
            }
            ICell cell = row.GetCell(colIndex);
            if (null == cell)
            {
                return null;
            }

            IFormulaEvaluator eva = WorkbookFactory.CreateFormulaEvaluator(workbook);
            object cellValue = GetCellValue(cell, eva);

            return null == cellValue ? null : cellValue.ToString();
        }

        #region DataTable 写入Sheet页
        /// <summary>
        /// DataTable 写入Sheet页
        /// </summary>
        /// <param name="dt">要写入的数据表</param>
        /// <param name="colNames">要写入的列的列名</param>
        /// <param name="sheetName">要写入的sheet页名称</param>
        /// <param name="startRowIndex">开始的行号（0开始）</param>
        /// <param name="startColIndex">开始的列号（0开始）</param>
        public void FillSheetFromDataTable(DataTable dt, string[] colNames = null, string sheetName = null, int startRowIndex = 0, int startColIndex = 0)
        {
            if (string.IsNullOrEmpty(sheetName))
            {
                throw new ArgumentException("参数sheetName不能为空");
            }
            int sheetIndex = GetSheetIndex(sheetName);

            FillSheetFromDataTable(dt, colNames, sheetIndex, startRowIndex, startColIndex);
        }

        /// <summary>
        /// DataTable 写入Sheet页
        /// </summary>
        /// <param name="dt">要写入的数据表</param>
        /// <param name="colNames">要写入的列的列名</param>
        /// <param name="sheetIndex">要写入的sheet页编号</param>
        /// <param name="startRowIndex">开始的行号（0开始）</param>
        /// <param name="startColIndex">开始的列号（0开始）</param>
        public void FillSheetFromDataTable(DataTable dt, string[] colNames = null, int sheetIndex = 0, int startRowIndex = 0, int startColIndex = 0)
        {
            if (null == dt || dt.Rows.Count == 0)
            {
                return;
            }
            if (null == colNames)
            {
                colNames = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    colNames[i] = dt.Columns[i].ColumnName;
                }
            }
            else
            {
                foreach (var colName in colNames)
                {
                    if (!dt.Columns.Contains(colName))
                    {
                        throw new Exception("数据源不包含列：" + colName);
                    }
                }
            }

            int iRowCount = dt.Rows.Count;
            int iColCount = colNames.Length;

            ISheet sheet = workbook.NumberOfSheets > sheetIndex ? workbook.GetSheetAt(sheetIndex) : workbook.CreateSheet();

            for (int i = 0; i < iRowCount; i++)
            {
                int rowIndex = i + startRowIndex;
                IRow row = sheet.GetRow(rowIndex);
                if (null == row)
                {
                    row = sheet.CreateRow(rowIndex);
                }
                for (int m = 0; m < iColCount; m++)
                {
                    int colIndex = m + startColIndex;
                    ICell cell = row.GetCell(colIndex);
                    if (null == cell)
                    {
                        cell = row.CreateCell(colIndex);
                        cell.CellStyle = row.RowStyle;
                    }
                    cell.SetCellValue(Convert.ToString(dt.Rows[i][colNames[m]]));
                }
            }
            sheet.ForceFormulaRecalculation = true;
            EvaluateAllFormula();
        }

        /// <summary>
        /// DataTable 写入Sheet页
        /// </summary>
        /// <param name="dt">要写入的数据表</param>
        /// <param name="IsEvaluateAllFormula">是否计算公式</param>
        /// <param name="colNames">要写入的列的列名</param>
        /// <param name="sheetIndex">要写入的sheet页编号</param>
        /// <param name="startRowIndex">开始的行号（0开始）</param>
        /// <param name="startColIndex">开始的列号（0开始）</param>
        public void FillSheetFromDataTable(bool IsEvaluateAllFormula, DataTable dt, string[] colNames = null, int sheetIndex = 0, int startRowIndex = 0, int startColIndex = 0)
        {
            if (null == dt || dt.Rows.Count == 0)
            {
                return;
            }
            if (null == colNames)
            {
                colNames = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    colNames[i] = dt.Columns[i].ColumnName;
                }
            }
            else
            {
                foreach (var colName in colNames)
                {
                    if (!dt.Columns.Contains(colName))
                    {
                        throw new Exception("数据源不包含列：" + colName);
                    }
                }
            }

            int iRowCount = dt.Rows.Count;
            int iColCount = colNames.Length;

            ISheet sheet = workbook.NumberOfSheets > sheetIndex ? workbook.GetSheetAt(sheetIndex) : workbook.CreateSheet();

            for (int i = 0; i < iRowCount; i++)
            {
                int rowIndex = i + startRowIndex;
                IRow row = sheet.GetRow(rowIndex);
                if (null == row)
                {
                    row = sheet.CreateRow(rowIndex);
                }
                for (int m = 0; m < iColCount; m++)
                {
                    int colIndex = m + startColIndex;
                    ICell cell = row.GetCell(colIndex);
                    if (null == cell)
                    {
                        cell = row.CreateCell(colIndex);
                        cell.CellStyle = row.RowStyle;
                    }
                    cell.SetCellValue(Convert.ToString(dt.Rows[i][colNames[m]]));
                }
            }
            sheet.ForceFormulaRecalculation = true;
            if (IsEvaluateAllFormula)
            {
                EvaluateAllFormula();
            }
        }

        #endregion

        #region 读取excel数据到datatable
        public DataTable ReadExcelBySheetName(string sheetName)
        {
            int sheetIndex = string.IsNullOrEmpty(sheetName) ? 0 : workbook.GetSheetIndex(sheetName);
            return ReadExcelBySheetIndex(sheetIndex);
        }

        public DataTable ReadExcelBySheetName(string sheetName, int iColumnNameRowIndex, int iDataRowIndex)
        {
            int sheetIndex = string.IsNullOrEmpty(sheetName) ? 0 : workbook.GetSheetIndex(sheetName);
            return ReadExcelBySheetIndex(sheetIndex, iColumnNameRowIndex, iDataRowIndex);
        }

        /// <summary>
        /// 读取excel数据到datatable
        /// </summary>
        /// <param name="sheetIndex">要读取的sheet页索引号（从0开始）</param>
        /// <returns></returns>
        public DataTable ReadExcelBySheetIndex(int sheetIndex)
        {
            return ReadExcelBySheetIndex(sheetIndex, 0, 1);
        }

        /// <summary>
        /// 读取excel数据到datatable
        /// </summary>
        /// <param name="sheetIndex">要读取的sheet页索引号（从0开始）</param>
        /// <param name="iColumnNameRowIndex">标题行号（从0开始，默认为0，-1表示不要标题行）</param>
        /// <param name="iDataRowIndex">数据开始的行号（从0开始，默认为1）</param>
        /// <returns></returns>
        public DataTable ReadExcelBySheetIndex(int sheetIndex, int iColumnNameRowIndex, int iDataRowIndex)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            if (null == sheet)
            {
                throw new Exception("没有找到sheet");
            }
            if (sheet.LastRowNum < 1)
            {
                throw new Exception("excel没有数据");
            }

            DataTable dt = new DataTable();
            //有标题行
            //IList<int> colIndexList = new List<int>();
            if (iColumnNameRowIndex >= 0)
            {
                IRow firstRow = sheet.GetRow(iColumnNameRowIndex);
                for (int i = 0; i < firstRow.LastCellNum; i++)
                {
                    ICell cell = firstRow.GetCell(i);
                    if (null == cell)
                    {
                        continue;
                    }
                    if (dt.Columns.Contains(cell.ToString()))
                    {

                        throw new Exception($"Excel文件出现相同列名“{cell.ToString()}”");
                    }
                    dt.Columns.Add(cell.ToString(), typeof(string));
                    //colIndexList.Add(i);
                }
            }
            //else
            //{
            //    IRow firstRow = sheet.GetRow(0);
            //    for (int i = 0; i < firstRow.LastCellNum; i++)
            //    {
            //        ICell cell = firstRow.GetCell(i);
            //        if (null == cell)
            //        {
            //            continue;
            //        }
            //        dt.Columns.Add("Column" + i, typeof(string));
            //        colIndexList.Add(i);
            //    }
            //}

            //int totalColNum = colIndexList.Count;//总列数

            for (int i = iDataRowIndex; i <= sheet.LastRowNum; i++)
            {
                IRow dRow = sheet.GetRow(i);
                if (null != dRow)
                {
                    object[] arrRow = new object[dRow.LastCellNum + 1];
                    for (int k = 0; k < dRow.LastCellNum + 1; k++)
                    {
                        if (k >= dt.Columns.Count)
                        {
                            dt.Columns.Add("Column" + k, typeof(string));
                        }
                        ICell dCell = dRow.GetCell(k);

                        IFormulaEvaluator eva = WorkbookFactory.CreateFormulaEvaluator(workbook);
                        object cellValue = GetCellValue(dCell, eva);

                        arrRow[k] = null == cellValue ? null : cellValue.ToString();
                    }
                    dt.Rows.Add(arrRow);
                }
            }
            return dt;
        }
        #endregion

        #region 判断Excel指定sheet页是否有相同列名
        public List<string> GetJudgeReportTempFaulty(int sheetIndex)
        {
            return GetJudgeReportTempFaulty(sheetIndex, 0, 1);
        }
        public List<string> GetJudgeReportTempFaulty(int sheetIndex, int iColumnNameRowIndex, int iDataRowIndex)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            List<string> resultMessage = new List<string>();
            if (null == sheet)
            {
                return resultMessage;
            }
            if (sheet.LastRowNum < 1)
            {
                return resultMessage;
            }

            DataTable dt = new DataTable();
            //有标题行
            //IList<int> colIndexList = new List<int>();
            if (iColumnNameRowIndex >= 0)
            {
                IRow firstRow = sheet.GetRow(iColumnNameRowIndex);
                for (int i = 0; i < firstRow.LastCellNum; i++)
                {
                    ICell cell = firstRow.GetCell(i);
                    if (null == cell)
                    {
                        continue;
                    }
                    if (dt.Columns.Contains(cell.ToString()))
                    {
                        resultMessage.Add($"Excel文件SampleDataSheet页中出现相同列名“{cell.ToString()}”");
                        //throw new Exception($"Excel文件出现相同列名“{cell.ToString()}”");
                        continue;
                    }
                    dt.Columns.Add(cell.ToString(), typeof(string));
                }
            }
            return resultMessage;
        }
        #endregion

        public void MergeCell(int sheetIndex, int beginRowIndex, int endRowIndex, int beginColIndex, int endColIndex)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            sheet.AddMergedRegion(new CellRangeAddress(beginRowIndex, endRowIndex, beginColIndex, endColIndex));
        }


        private static int GetMaxColumnCount(ISheet sheet)
        {
            int maxCol = 0;

            // 遍历所有行，找出最大的列号
            for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row != null && row.LastCellNum > maxCol)
                {
                    maxCol = row.LastCellNum;
                }
            }

            return maxCol;
        }

        public static float GetCellHeight(IRow row)
        {
            // 如果行高未明确设置，则使用默认行高
            // Excel中以1/20磅为单位，所以需要除以20
            return row.Height == -1 ? row.Sheet.DefaultRowHeight / 20.0f : row.Height / 20.0f;
        }


        #region 文件保存

        /// <summary>
        /// 另存为Stream
        /// </summary>
        /// <param name="stream"></param>
        public void SaveTo(Stream stream)
        {
            workbook.Write(stream);
        }

        /// <summary>
        /// 另存为文件
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveTo(string filePath)
        {
            var f = new FileInfo(filePath);
            if (f.Exists)
            {
                f.Delete();
            }

            using (var fs = f.Create())
            {
                SaveTo(fs);
            }
        }
        #endregion

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (null != workbook)
            {
                workbook.Close();
            }

            workbook = null;
        }

        #region 释放资源
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    Close();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~NPOIExcelHelper()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="imageBytes"></param>
        /// <param name="anchor"></param>
        public void InsertImage(int sheetIndex, byte[] imageBytes, NPOIImageAnchor anchor)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);

            int pictureIdx = sheet.Workbook.AddPicture(imageBytes, PictureType.PNG);

            // 创建绘图容器
            HSSFPatriarch drawing = (HSSFPatriarch)sheet.CreateDrawingPatriarch();

            // 创建锚点
            HSSFClientAnchor clientAnchor = new HSSFClientAnchor();
            clientAnchor.Col1 = anchor.Col1;
            clientAnchor.Row1 = anchor.Row1;
            clientAnchor.Col2 = anchor.Col2;
            clientAnchor.Row2 = anchor.Row2;

            HSSFPicture picture = (HSSFPicture)drawing.CreatePicture(clientAnchor, pictureIdx);
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="imageSrc"></param>
        public void InsertImage(int sheetIndex, string imageSrc, NPOIImageAnchor anchor)
        {
            byte[] imageBytes = File.ReadAllBytes(imageSrc);
            InsertImage(sheetIndex, imageBytes, anchor);
        }



        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="imageData"></param>
        /// <param name="anchor"></param>
        /// <param name="pictureType"></param>
        public void InsertImage(int sheetIndex, byte[] imageData, NPOIImageAnchor anchor, PictureType pictureType)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            int pictureIdx = sheet.Workbook.AddPicture(imageData, pictureType);
            HSSFPatriarch drawing = (HSSFPatriarch)sheet.CreateDrawingPatriarch();
            IClientAnchor clientAnchor = workbook.GetCreationHelper().CreateClientAnchor();
            clientAnchor.Col1 = anchor.Col1;
            clientAnchor.Row1 = anchor.Row1;
            clientAnchor.Col2 = anchor.Col2;
            clientAnchor.Row2 = anchor.Row2;
            clientAnchor.AnchorType = (AnchorType)anchor.AnchorType;
            HSSFPicture picture = (HSSFPicture)drawing.CreatePicture(clientAnchor, pictureIdx);
        }

        public void InsertPngImage(int sheetIndex, byte[] imageData, NPOIImageAnchor anchor)
        {
            InsertImage(sheetIndex, imageData, anchor, PictureType.PNG);
        }

        public void InsertJpegImage(int sheetIndex, byte[] imageData, NPOIImageAnchor anchor)
        {
            InsertImage(sheetIndex, imageData, anchor, PictureType.JPEG);
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="imageSrc"></param>
        public void InsertPngImage(int sheetIndex, string imageSrc, NPOIImageAnchor anchor)
        {
            byte[] imageData = File.ReadAllBytes(imageSrc);
            InsertPngImage(sheetIndex, imageData, anchor);
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="imageData"></param>
        /// <param name="pictureType"></param>
        public void InsertImage(int sheetIndex, byte[] imageData, int row1, int col1, double scaleX, double scaleY, PictureType pictureType)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            int pictureIndex = workbook.AddPicture(imageData, pictureType);
            var drawing = sheet.CreateDrawingPatriarch();
            IClientAnchor anchor = workbook.GetCreationHelper().CreateClientAnchor();
            anchor.Row1 = row1;
            anchor.Col1 = col1;
            IPicture picture = drawing.CreatePicture(anchor, pictureIndex);
            picture.Resize(scaleX, scaleY);
        }

        public void InsertPngImage(int sheetIndex, byte[] imageData, int row1, int col1, double scaleX, double scaleY)
        {
            InsertImage(sheetIndex, imageData, row1, col1, scaleX, scaleY, PictureType.PNG);
        }

        public void InsertJpegImage(int sheetIndex, byte[] imageData, int row1, int col1, double scaleX, double scaleY)
        {
            InsertImage(sheetIndex, imageData, row1, col1, scaleX, scaleY, PictureType.JPEG);
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="imageData"></param>
        /// <param name="anchor"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="pictureType"></param>
        public void InsertImage(int sheetIndex, byte[] imageData,
            NPOIImageAnchor anchor, double scaleX, double scaleY,
            PictureType pictureType)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            int pictureIndex = workbook.AddPicture(imageData, pictureType);
            var drawing = sheet.CreateDrawingPatriarch();
            IClientAnchor clientAnchor = workbook.GetCreationHelper().CreateClientAnchor();
            clientAnchor.Row1 = anchor.Row1;
            clientAnchor.Col1 = anchor.Col1;
            clientAnchor.Row2 = anchor.Row2;
            clientAnchor.Col2 = anchor.Col2;
            clientAnchor.Dx1 = anchor.Dx1;
            clientAnchor.Dy1 = anchor.Dy1;
            clientAnchor.Dx2 = anchor.Dx2;
            clientAnchor.Dy2 = anchor.Dy2;
            clientAnchor.AnchorType = (AnchorType)anchor.AnchorType;
            IPicture picture = drawing.CreatePicture(clientAnchor, pictureIndex);
            picture.Resize(scaleX, scaleY);
        }

        public void InsertJpegImage(int sheetIndex, byte[] imageData,
           NPOIImageAnchor anchor, double scaleX, double scaleY)
        {
            InsertImage(sheetIndex, imageData, anchor, scaleX, scaleY, PictureType.JPEG);
        }

        public void InsertPngImage(int sheetIndex, byte[] imageData,
           NPOIImageAnchor anchor, double scaleX, double scaleY)
        {
            InsertImage(sheetIndex, imageData, anchor, scaleX, scaleY, PictureType.PNG);
        }

        /// <summary>
        /// 清附sheet中所有的图片
        /// </summary>
        /// <param name="sheet"></param>
        public void ClearSheetImages(int sheetIndex)
        {
            if (sheetIndex == -1)
            {
                return;
            }

            ISheet sheet = workbook.GetSheetAt(sheetIndex);

            // 获取工作表中的所有图形容器
            var patriarch = sheet.DrawingPatriarch as HSSFPatriarch;
            if (patriarch != null)
            {
                // 获取所有形状
                int count = patriarch.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    // 为什么这里是0,而不是i?
                    // 例如有2张图片,删除一张图片后,第2张图片的下标已经成了0
                    // 总体类似队列出队的过程
                    var shape = patriarch.Children[0];
                    if (shape is HSSFPicture)
                    {
                        patriarch.RemoveShape(shape);
                    }
                }
            }
        }

        /// <summary>
        /// 模糊查找sheet名称
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public int FindSheetIndex(string sheetName)
        {
            int sheetIndex = -1;
            int count = workbook.NumberOfSheets;
            for (int i = 0; i < count; i++)
            {
                var itemName = workbook.GetSheetName(i);
                if (itemName.Contains(sheetName))
                {
                    sheetIndex = i;
                    break;
                }
            }

            return sheetIndex;
        }


        private NPOICellReference GetCellReference(string col, string row)
        {
            CellReference cellRef = new CellReference(col + row);
            return new NPOICellReference()
            {
                Row = cellRef.Row,
                Col = cellRef.Col,
            };
        }

        public List<NPOICellReference> GatCellReferences(string name)
        {
            List<NPOICellReference> cellReferences = new List<NPOICellReference>();
            // 获取特定名称
            IName rangeName = workbook.GetName(name);
            if (rangeName != null)
            {
                // 获取名称引用的区域
                string refersTo = rangeName.RefersToFormula;
                string sheetName = refersTo.Split('!')[0];
                // 是一个区域
                if (refersTo.Contains(':'))
                {
                    var array = refersTo.Split(':');
                    string startCol = array[0].Split('$')[1];
                    string startRow = array[0].Split('$')[2];
                    NPOICellReference startCellRef = GetCellReference(startCol, startRow);
                    cellReferences.Add(startCellRef);

                    string endCol = array[1].Split('$')[1];
                    string endRow = array[1].Split('$')[2];
                    NPOICellReference endCellRef = GetCellReference(endCol, endRow);
                    cellReferences.Add(endCellRef);
                }
                else
                {
                    string startCol = refersTo.Split('$')[1];
                    string startRow = refersTo.Split('$')[2];
                    NPOICellReference startCellRef = GetCellReference(startCol, startRow);
                    cellReferences.Add(startCellRef);
                }
            }

            return cellReferences;
        }

        public List<NPOIName> GetAllNames()
        {
            List<NPOIName> names = new List<NPOIName>();
            IList<IName> definedNames = workbook.GetAllNames();
            foreach (IName item in definedNames)
            {
                names.Add(new NPOIName()
                {
                    IsFunctionName = item.IsFunctionName,
                    NameName = item.NameName,
                    SheetName = item.SheetName,
                    RefersToFormula = item.RefersToFormula,
                    Comment = item.Comment,
                });
            }
            return names;
        }

        public List<NPOIName> GetNames(string sheetName)
        {
            List<NPOIName> names = new List<NPOIName>();
            IList<IName> definedNames = workbook.GetNames(sheetName);
            foreach (IName item in definedNames)
            {
                names.Add(new NPOIName()
                {
                    IsFunctionName = item.IsFunctionName,
                    NameName = item.NameName,
                    SheetName = item.SheetName,
                    RefersToFormula = item.RefersToFormula,
                    Comment = item.Comment,
                });
            }
            return names;
        }

        public void OnlyShowAnySheet(string sheetName)
        {
            int count = workbook.NumberOfSheets;
            for (int i = 0; i < count; i++)
            {
                var itemName = workbook.GetSheetName(i);
                if (itemName.Contains(sheetName))
                {
                    workbook.SetSheetVisibility(i, SheetVisibility.Visible);
                }
                else
                {
                    workbook.SetSheetVisibility(i, SheetVisibility.Hidden);
                }
            }
        }



        #endregion
        #endregion

    }
}