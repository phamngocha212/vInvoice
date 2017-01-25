using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EInvoice.CAdmin.Models;
using System.Data.OleDb;

namespace EInvoice.CAdmin
{
    public class UtilExcel
    {
        #region Excel
        public static void CreateSpreadsheetWorkbook(string filepath)
        {
            //string filePath = AppDomain.CurrentDomain.BaseDirectory + "MyExcel.xlsx";
            //string templatePath = AppDomain.CurrentDomain.BaseDirectory + "Temp.xlsx";
            //Excel.CreateSpreadsheetWorkbook(filePath);
            //Excel.GetDataResults(reader, filePath, templatePath);
            //Delete document file
            if (File.Exists(filepath)) File.Delete(filepath);

            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            //This section may be needed at a later time, if the users want to style
            //The XLSX file that is created by the application
            //Dim workbookStylesPart As WorkbookStylesPart = workbookpart.AddNewPart(Of WorkbookStylesPart)()
            //workbookStylesPart.Stylesheet = CreateStylesheet()
            //workbookStylesPart.Stylesheet.Save()

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet();
            sheet.Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart);
            sheet.SheetId = 1;
            sheet.Name = "ExportData";

            sheets.Append(sheet);

            workbookpart.Workbook.Save();

            // Close the document.
            spreadsheetDocument.Close();
        }
        #region report Adjust
        public static void GetDataResultsAdjust(IList<RecordAdjust> lstRecord, string filePath, string templatePath, string status, string fromdate, string todate)
        {
            if ((!File.Exists(templatePath)))
            {
                //If we do not have a template file, we need to create one
                CreateSpreadsheetWorkbook(templatePath);
            }
            //Copy the template file to the output file location
            File.Copy(templatePath, filePath, true);
            //Open the copied file
            using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(filePath, true))
            {
                //Navigate to the workbook part
                WorkbookPart workbookPart = myDoc.WorkbookPart;
                //open the first worksheet
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                //Get the id of this sheet. We need this because we are going to add a new 
                //worksheet to this workbook, then we are going to delete this worksheet
                //This ID will tell us which one to delete
                string origninalSheetId = workbookPart.GetIdOfPart(worksheetPart);
                //Add the new worksheet
                WorksheetPart replacementPart = workbookPart.AddNewPart<WorksheetPart>();
                //This is the ID of the new worksheet
                string replacementPartId = workbookPart.GetIdOfPart(replacementPart);
                //We are going to read from the original worksheet to get the 
                //templated items that we added to the worksheet in the traditional way
                OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);
                //We are goint to copy the items from the original worksheet by using
                //an XML writer, this overcomes the memory limitations of having
                //an extremely large dataset.
                OpenXmlWriter writer = OpenXmlWriter.Create(replacementPart);
                //The template does not have any data so we will be creating new rows and cells
                //Then writing them using XML.
                Row r = new Row();
                Cell c = new Cell();
                while ((reader.Read()))
                {
                    //This iterates through the sheet data and copies it
                    if ((object.ReferenceEquals(reader.ElementType, typeof(SheetData))))
                    {
                        //Exit the loop if we hit a sheetdata end element
                        if ((reader.IsEndElement))
                            break; // TODO: might not be correct. Was : Exit While
                        //We create a new sheetdata element (basically this is the reoot container for a sheet)
                        writer.WriteStartElement(new SheetData());
                        if (status == "0")
                        {
                            int rowIndex2 = 2;
                            r.RowIndex = (UInt32)rowIndex2;
                            writer.WriteStartElement(r);
                            c = CreateCell(5, "BÁO CÁO HÓA ĐƠN THAY THẾ", rowIndex2);
                            writer.WriteElement(c);
                            writer.WriteEndElement();
                        }
                        else if (status == "1")
                        {
                            int rowIndex2 = 2;
                            r.RowIndex = (UInt32)rowIndex2;
                            writer.WriteStartElement(r);
                            c = CreateCell(5, "BÁO CÁO HÓA ĐƠN ĐIỀU CHỈNH", rowIndex2);
                            writer.WriteElement(c);
                            writer.WriteEndElement();
                        }
                        int rowIndextieudea = 3;
                        r.RowIndex = (UInt32)rowIndextieudea;
                        writer.WriteStartElement(r);
                        c = CreateCell(0, "Từ ngày:" + fromdate, rowIndextieudea);
                        writer.WriteElement(c);
                        writer.WriteEndElement();

                        int rowIndextieudeb = 4;
                        r.RowIndex = (UInt32)rowIndextieudeb;
                        writer.WriteStartElement(r);
                        c = CreateCell(0, "Đến ngày:" + todate, rowIndextieudeb);
                        writer.WriteElement(c);
                        writer.WriteEndElement();

                        int rowIndextieudec = 5;
                        r.RowIndex = (UInt32)rowIndextieudec;
                        writer.WriteStartElement(r);
                        c = CreateCell(0, "Tổng số hóa đơn:" + lstRecord.Count(), rowIndextieudec);
                        writer.WriteElement(c);
                        writer.WriteEndElement();

                        int rowIndex = 6;
                        r.RowIndex = (UInt32)rowIndex;
                        //Start the first row.
                        writer.WriteStartElement(r);
                        //Iterate through the columns to write a row conaining the column names

                        c = CreateCell(0, "STT", rowIndex);
                        writer.WriteElement(c);
                        c = CreateCell(1, "Ký hiệu mẫu", rowIndex);
                        writer.WriteElement(c);
                        c = CreateCell(2, "Ký hiệu hóa đơn", rowIndex);
                        writer.WriteElement(c);
                        c = CreateCell(3, "Số hóa đơn", rowIndex);
                        writer.WriteElement(c);
                        c = CreateCell(4, "Khách hàng", rowIndex);
                        writer.WriteElement(c);

                        c = CreateCell(5, "Ký hiệu mẫu", rowIndex);
                        writer.WriteElement(c);
                        c = CreateCell(6, "Ký hiệu hóa đơn", rowIndex);
                        writer.WriteElement(c);
                        c = CreateCell(7, "Số hóa đơn", rowIndex);
                        writer.WriteElement(c);
                        c = CreateCell(8, "Khách hàng", rowIndex);
                        writer.WriteElement(c);

                        c = CreateCell(9, "Người thực hiện", rowIndex);
                        writer.WriteElement(c);

                        c = CreateCell(10, "Ngày thực hiện", rowIndex);
                        writer.WriteElement(c);
                        c = CreateCell(11, "Trạng thái", rowIndex);
                        writer.WriteElement(c);

                        //End first row
                        writer.WriteEndElement();
                        foreach (var item in lstRecord)
                        {
                            if (item == null) continue;
                            rowIndex = rowIndex + 1;
                            r.RowIndex = (UInt32)rowIndex;
                            writer.WriteStartElement(r);
                            //iterate through the columns to write their data

                            c = CreateCell(0, item.stt.ToString(), rowIndex); writer.WriteElement(c);
                            c = CreateCell(1, item.patternOlder.ToString(), rowIndex); writer.WriteElement(c);
                            c = CreateCell(2, item.serialOlder.ToString(), rowIndex); writer.WriteElement(c);
                            c = CreateCell(3, string.Format("{0:0000000}", Convert.ToInt32(item.noOlder)), rowIndex); writer.WriteElement(c);
                            //c = CreateCell(4, item.cusnameOlder.ToString(), rowIndex); writer.WriteElement(c);

                            c = CreateCell(5, item.patternNew.ToString(), rowIndex); writer.WriteElement(c);
                            c = CreateCell(6, item.serialNew.ToString(), rowIndex); writer.WriteElement(c);
                            c = CreateCell(7, string.Format("{0:0000000}", Convert.ToInt32(item.noNew)), rowIndex); writer.WriteElement(c);
                            c = CreateCell(8, item.cusnameNew.ToString(), rowIndex); writer.WriteElement(c);

                            c = CreateCell(9, item.username.ToString(), rowIndex); writer.WriteElement(c);
                            c = CreateCell(10, String.Format("{0:dd/MM/yyyy}", DateTime.Parse(item.proccessdate)), rowIndex); writer.WriteElement(c);
                            if (item.status == 1) { c = CreateCell(11, "Lập hóa đơn thay thế", rowIndex); writer.WriteElement(c); }
                            else if (item.status == 2) { c = CreateCell(11, "Lập hóa đơn điều chỉnh", rowIndex); writer.WriteElement(c); }
                            //write the end of the row
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                    else if ((reader.IsStartElement))
                    {
                        //Start elements are directly copied
                        writer.WriteStartElement(reader);
                    }
                    else if ((reader.IsEndElement))
                    {
                        //End elements are directly copied
                        writer.WriteEndElement();
                    }
                }

                //Close the reader and writer
                reader.Close();
                writer.Close();

                //Get the newly created sheet (same id as the old sheet, but it is the first one)
                Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Id.Value.Equals(origninalSheetId)).First();
                //Assign it the new sheet id
                sheet.Id.Value = replacementPartId;
                //remove the old sheet
                workbookPart.DeletePart(worksheetPart);
                //Done
                myDoc.Close();

                //Delete template file
                if (File.Exists(templatePath)) File.Delete(templatePath);
            }
        }
        #endregion end report Adjust
        #region report CancelInv
        public static void GetDataResultsInvCancel(IList<RecordInvCancel> lstRecord, string filePath, string templatePath, string fromdate, string todate)
        {
            if ((!File.Exists(templatePath)))
            {
                //If we do not have a template file, we need to create one
                CreateSpreadsheetWorkbook(templatePath);
            }
            //Copy the template file to the output file location
            File.Copy(templatePath, filePath, true);
            //Open the copied file
            using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(filePath, true))
            {
                //Navigate to the workbook part
                WorkbookPart workbookPart = myDoc.WorkbookPart;
                //open the first worksheet
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                //Get the id of this sheet. We need this because we are going to add a new 
                //worksheet to this workbook, then we are going to delete this worksheet
                //This ID will tell us which one to delete
                string origninalSheetId = workbookPart.GetIdOfPart(worksheetPart);
                //Add the new worksheet
                WorksheetPart replacementPart = workbookPart.AddNewPart<WorksheetPart>();
                //This is the ID of the new worksheet
                string replacementPartId = workbookPart.GetIdOfPart(replacementPart);
                //We are going to read from the original worksheet to get the 
                //templated items that we added to the worksheet in the traditional way
                OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);
                //We are goint to copy the items from the original worksheet by using
                //an XML writer, this overcomes the memory limitations of having
                //an extremely large dataset.
                OpenXmlWriter writer = OpenXmlWriter.Create(replacementPart);
                //The template does not have any data so we will be creating new rows and cells
                //Then writing them using XML.
                Row r = new Row();
                Cell c = new Cell();
                while ((reader.Read()))
                {
                    //This iterates through the sheet data and copies it
                    if ((object.ReferenceEquals(reader.ElementType, typeof(SheetData))))
                    {
                        //Exit the loop if we hit a sheetdata end element
                        if ((reader.IsEndElement))
                            break; // TODO: might not be correct. Was : Exit While
                        //We create a new sheetdata element (basically this is the reoot container for a sheet)
                        writer.WriteStartElement(new SheetData());

                        int rowIndex1 = 1;
                        r.RowIndex = (UInt32)rowIndex1;
                        writer.WriteStartElement(r);
                        c = CreateCell(3, "BÁO CÁO HÓA ĐƠN HỦY", rowIndex1);
                        writer.WriteElement(c);
                        writer.WriteEndElement();

                        int rowIndex2 = 3;
                        r.RowIndex = (UInt32)rowIndex2;
                        writer.WriteStartElement(r);
                        c = CreateCell(0, "Từ ngày:" + fromdate, rowIndex2);
                        writer.WriteElement(c);
                        writer.WriteEndElement();

                        int rowIndex3 = 4;
                        r.RowIndex = (UInt32)rowIndex3;
                        writer.WriteStartElement(r);
                        c = CreateCell(0, "Đến ngày:" + todate, rowIndex3);
                        writer.WriteElement(c);
                        writer.WriteEndElement();

                        int rowIndex4 = 5;
                        r.RowIndex = (UInt32)rowIndex4;
                        writer.WriteStartElement(r);
                        c = CreateCell(0, "Tổng số hóa đơn:" + lstRecord.Count(), rowIndex4);
                        writer.WriteElement(c);
                        writer.WriteEndElement();

                        int rowIndex = 6;
                        r.RowIndex = (UInt32)rowIndex;
                        //Start the first row.
                        writer.WriteStartElement(r);
                        //Iterate through the columns to write a row conaining the column names

                        c = CreateCell(0, "STT", rowIndex);
                        writer.WriteElement(c);
                        c = CreateCell(1, "Người thực hiện", rowIndex);
                        writer.WriteElement(c);
                        c = CreateCell(2, "Ký hiệu mẫu", rowIndex);
                        writer.WriteElement(c);
                        c = CreateCell(3, "Ký hiệu hóa đơn", rowIndex);
                        writer.WriteElement(c);
                        c = CreateCell(4, "Số hóa đơn", rowIndex);
                        writer.WriteElement(c);

                        c = CreateCell(5, "Ngày thực hiện", rowIndex);
                        writer.WriteElement(c);
                        //End first row
                        writer.WriteEndElement();
                        foreach (var item in lstRecord)
                        {
                            if (item == null) continue;
                            rowIndex = rowIndex + 1;
                            r.RowIndex = (UInt32)rowIndex;
                            writer.WriteStartElement(r);
                            //iterate through the columns to write their data

                            c = CreateCell(0, item.stt.ToString(), rowIndex); writer.WriteElement(c);
                            c = CreateCell(1, item.Username.ToString(), rowIndex); writer.WriteElement(c);
                            c = CreateCell(2, item.pattern.ToString(), rowIndex); writer.WriteElement(c);
                            c = CreateCell(3, item.serial.ToString(), rowIndex); writer.WriteElement(c);
                            c = CreateCell(4, Convert.ToDecimal(item.no).ToString("0000000"), rowIndex); writer.WriteElement(c);
                            c = CreateCell(5, String.Format("{0:dd/MM/yyyy}", DateTime.Parse(item.DayCancelInv)), rowIndex); writer.WriteElement(c);

                            //write the end of the row
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                    else if ((reader.IsStartElement))
                    {
                        //Start elements are directly copied
                        writer.WriteStartElement(reader);
                    }
                    else if ((reader.IsEndElement))
                    {
                        //End elements are directly copied
                        writer.WriteEndElement();
                    }
                }

                //Close the reader and writer
                reader.Close();
                writer.Close();

                //Get the newly created sheet (same id as the old sheet, but it is the first one)
                Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Id.Value.Equals(origninalSheetId)).First();
                //Assign it the new sheet id
                sheet.Id.Value = replacementPartId;
                //remove the old sheet
                workbookPart.DeletePart(worksheetPart);
                //Done
                myDoc.Close();

                //Delete template file
                if (File.Exists(templatePath)) File.Delete(templatePath);
            }
        }
        #endregion end report CancelInv

        private static string GetExcelColumnName(int columnNumber)
        {
            //This gets the column name (ie. A1, B4, AA21, etc...)
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo = 0;

            while (dividend >= 0)
            {
                //twenty-six letters in the alphabet
                modulo = (dividend) % 26;
                //Append letters until we have accounted for all digits
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                //Get the remainder
                dividend = Convert.ToInt32((dividend - modulo) / 26) - 1;
            }

            return columnName;
        }

        private static Cell CreateCell(int columnNumber, string value, int index)
        {
            Cell formattedCell = new Cell();
            formattedCell.DataType = CellValues.InlineString;
            formattedCell.CellReference = GetExcelColumnName(columnNumber) + index;
            //formattedCell.StyleIndex = 4
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = value;
            inlineString.AppendChild(t);
            formattedCell.AppendChild(inlineString);
            //End If

            return formattedCell;
        }
        #endregion

        #region read excel to DataTable
        /// <summary>
        /// Doc file excel (.xls, .xlsx) ra dataTable
        /// </summary>
        /// <param name="filePath">Full file path</param>
        /// <param name="sheetName">name of sheet to read</param>
        /// <param name="isHeader">true: row 1 is header of column</param>
        /// <returns></returns>
        public static DataTable readExcelFile(string filePath, string sheetName, bool isHeader)
        {
            string conn = "";
            if (isHeader)
            {
                conn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";
            }
            else
            {
                conn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=No;IMEX=1'";
            }
            OleDbConnection objConn = new OleDbConnection(conn);
            objConn.Open();
            OleDbCommand command = new OleDbCommand("SELECT * FROM [" + sheetName + "]", objConn);
            OleDbDataAdapter objAdapter = new OleDbDataAdapter();
            objAdapter.SelectCommand = command;
            DataSet objDataset = new DataSet();
            objAdapter.Fill(objDataset);
            objConn.Close();
            DataTable objTable = objDataset.Tables[0];
            return objTable;
        }
        #endregion
    }
}