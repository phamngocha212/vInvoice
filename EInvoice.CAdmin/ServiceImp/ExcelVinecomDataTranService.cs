using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EInvoice.Core.IService;
using System.Data.OleDb;
using System.Data;
using FX.Core;
using log4net;
using System.Text.RegularExpressions;
using EInvoice.CAdmin.IService;
using EInvoice.Core;
using EInvoice.Core.Domain;

namespace EInvoice.CAdmin.ServiceImp
{
    public class ExcelVinecomDataTranService : IDataTranService
    {
        ILog log = LogManager.GetLogger(typeof(ExcelVinecomDataTranService));
        private bool cusValidate(Customer cus, ref string message)
        {
            message = "";
            return true;
        }
        public string ParseCustomer(byte[] data, ref string mesage)
        {
            StringBuilder rv = new StringBuilder("<Customers>");
            mesage = "";
            try
            {
                const string MaKhachHang = "MaKhachHang";
                const string TenKhachHang = "TenKhachHang";
                const string DiaChi = "DiaChi";
                const string MaSoThue = "MaSoThue";
                const string DienThoai = "DienThoai";
                const string Email = "Email";
                const string CusType = "LoaiKhachHang";
                const string TenTaiKhoan = "TenTaiKhoan";
                const string BankAccountName = "BankAccountName";
                const string BankName = "BankName";
                const string BankNumber = "BankNumber";
                const string Fax = "Fax";
                const string ContactPerson = "ContactPerson";
                const string RepresentPerson = "RepresentPerson";
                Guid guid = Guid.NewGuid();
                string fileName = AppDomain.CurrentDomain.BaseDirectory + "/Tempfile/" + guid + ".xls";
                using (System.IO.FileStream fs = System.IO.File.Create(fileName))
                {
                    fs.Write(data, 0, data.Length);
                }
                string conn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";
                OleDbConnection objConn = new OleDbConnection(conn);
                objConn.Open();
                OleDbCommand command = new OleDbCommand("SELECT * FROM [Sheet1$]", objConn);
                OleDbDataAdapter objAdapter = new OleDbDataAdapter();
                objAdapter.SelectCommand = command;
                DataSet objDataset = new DataSet();
                objAdapter.Fill(objDataset);
                objConn.Close();
                System.IO.File.Delete(fileName);
                DataTable objTable = objDataset.Tables[0];
                foreach (DataRow dr in objTable.Rows)
                {
                    //if (string.IsNullOrWhiteSpace(dr[MaKhachHang].ToString()))
                    //{
                    //    continue;
                    //}
                    StringBuilder sb = new StringBuilder("<Customer>");
                    string temp;
                    //Customer cus = new Customer();
                    sb.Append("<Name>");
                    temp = dr[TenKhachHang].ToString();
                    if (!string.IsNullOrEmpty(temp))
                        sb.Append(temp.Trim());
                    sb.Append("</Name><Code>");
                    temp = dr[MaKhachHang].ToString();
                    if (!string.IsNullOrEmpty(temp))
                        sb.Append(temp.Trim());
                    sb.Append("</Code><TaxCode>");
                    temp = dr[MaSoThue].ToString();
                    if (!string.IsNullOrEmpty(temp))
                        sb.Append(temp.Trim());
                    sb.Append("</TaxCode><Address>");
                    temp = dr[DiaChi].ToString();
                    if (!string.IsNullOrEmpty(temp))
                        sb.Append(temp.Trim());
                    sb.Append("</Address><Email>");
                    temp = dr[Email].ToString();
                    if (!string.IsNullOrEmpty(temp))
                        sb.Append(temp.Trim());
                    sb.Append("</Email><Phone>");
                    temp = dr[DienThoai].ToString();
                    if (!string.IsNullOrEmpty(temp))
                        sb.Append(temp.Trim());
                    sb.Append("</Phone><CusType>");
                    int custype = 0;        //default 0 -- khach hang ca nhan
                    if (dr.Table.Columns.Contains(CusType))
                    {
                        if (!string.IsNullOrWhiteSpace(dr[CusType].ToString()))
                        {
                            Int32.TryParse(dr[CusType].ToString(), out custype);
                        }
                    }
                    sb.Append(custype.ToString());
                    sb.Append("</CusType><BankAccountName>");
                    if (dr.Table.Columns.Contains(BankAccountName))
                    {
                        temp = dr[BankAccountName].ToString();
                        if (!string.IsNullOrEmpty(temp))
                            sb.Append(temp.Trim());
                    }
                    sb.Append("</BankAccountName><BankName>");
                    if (dr.Table.Columns.Contains(BankName))
                    {
                        temp = dr[BankName].ToString();
                        if (!string.IsNullOrEmpty(temp))
                            sb.Append(temp.Trim());
                    }
                    sb.Append("</BankName><BankNumber>");
                    if (dr.Table.Columns.Contains(BankNumber))
                    {
                        temp = dr[BankNumber].ToString();
                        if (!string.IsNullOrEmpty(temp))
                            sb.Append(temp.Trim());
                    }
                    sb.Append("</BankNumber><Fax>");
                    if (dr.Table.Columns.Contains(Fax))
                    {
                        temp = dr[Fax].ToString();
                        if (!string.IsNullOrEmpty(temp))
                            sb.Append(temp.Trim());
                    }
                    sb.Append("</Fax><ContactPerson>");
                    if (dr.Table.Columns.Contains(ContactPerson))
                    {
                        temp = dr[ContactPerson].ToString();
                        if (!string.IsNullOrEmpty(temp))
                            sb.Append(temp.Trim());
                    }
                    sb.Append("</ContactPerson><RepresentPerson>");
                    if (dr.Table.Columns.Contains(RepresentPerson))
                    {
                        temp = dr[RepresentPerson].ToString();
                        if (!string.IsNullOrEmpty(temp))
                            sb.Append(temp.Trim());
                    }
                    sb.Append("</RepresentPerson></Customer>");
                    rv.AppendLine(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                log.Error("ParseCustomer " + ex.Message);
                mesage = ex.Message;
                return null;
            }
            rv.Append("</Customers>");
            return rv.ToString();
        }

        public string ParseIInvoiceCancel(byte[] data, ref string message)
        {
            StringBuilder rv = new StringBuilder("<Inv>");
            message = "";
            try
            {
                const string Fkey = "key";
                Guid guid = Guid.NewGuid();
                string fileName = AppDomain.CurrentDomain.BaseDirectory + "/Tempfile/" + guid + ".xls";
                using (System.IO.FileStream fs = System.IO.File.Create(fileName))
                {
                    fs.Write(data, 0, data.Length);
                }
                string conn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";
                OleDbConnection objConn = new OleDbConnection(conn);
                objConn.Open();
                OleDbCommand command = new OleDbCommand("SELECT * FROM [Sheet1$]", objConn);
                OleDbDataAdapter objAdapter = new OleDbDataAdapter();
                objAdapter.SelectCommand = command;
                DataSet objDataset = new DataSet();
                objAdapter.Fill(objDataset);
                objConn.Close();
                System.IO.File.Delete(fileName);
                DataTable objTable = objDataset.Tables[0];
                foreach (DataRow dr in objTable.Rows)
                {
                    StringBuilder sb = new StringBuilder("<key>");
                    string temp;
                    temp = dr[Fkey].ToString();
                    if (!string.IsNullOrEmpty(temp))
                        sb.Append(temp.Trim());
                    sb.Append("</key>");
                    rv.AppendLine(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                log.Error("ParseCancelInvoice " + ex.Message);
                message = ex.Message;
                return null;
            }
            rv.Append("</Inv>");
            return rv.ToString();
        }

        public string ParseIInvoice(byte[] data, string pattern, string serial, int comID, string accountName, ref string message, ref int failtCount)
        {
            StringBuilder rv = new StringBuilder("<Invoices>");
            rv.AppendLine("<BillTime></BillTime>");
            message = "";
            failtCount = 0;
            StringBuilder Failed = new StringBuilder("<Invoices>");
            try
            {
                const string Fkey = "Fkey";
                const string OrderNo = "DonDatHang";
                const string OrderDate = "NgayDatHang";
                const string ArisingDate = "NgayHoaDon";
                const string CusName = "TenKhachHang";
                const string CusCode = "MaKhachHang";
                const string CusCom = "TenDonVi";
                const string CusAddress = "DiaChiKhachHang";
                const string CusPhone = "DienThoaiKhachHang";
                const string CusTaxCode = "MaSoThue";
                const string Kind_of_Payment = "HinhThucTT";
                const string ProdCode = "MaSanPham";
                const string ProdName = "SanPham";
                const string ProdUnit = "DonViTinh";
                const string ProdQuantity = "SoLuong";
                const string ProdPrice = "DonGia";
                const string ProdTotal = "ThanhTien";
                const string ProdVATRate = "ThueSuat";
                const string ProdVATAmount = "TienThue";
                const string ProdAmount = "TienBan";
                const string GrossValue = "ThanhTien-1";
                const string GrossValue0 = "ThanhTien0";
                const string GrossValue5 = "ThanhTien5";
                const string VatAmount5 = "Thue5";
                const string GrossValue10 = "ThanhTien10";
                const string VatAmount10 = "Thue10";
                const string Total = "TongThanhTien";
                const string VAT_Amount = "TongThue";
                const string Amount = "TongCong";

                Guid guid = Guid.NewGuid();
                string fileName = AppDomain.CurrentDomain.BaseDirectory + "/Tempfile/" + guid + ".xls";
                using (System.IO.FileStream fs = System.IO.File.Create(fileName))
                {
                    fs.Write(data, 0, data.Length);
                }
                string conn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";
                OleDbConnection objConn = new OleDbConnection(conn);
                objConn.Open();
                OleDbCommand command = new OleDbCommand("SELECT * FROM [Sheet1$]", objConn);
                OleDbDataAdapter objAdapter = new OleDbDataAdapter();
                objAdapter.SelectCommand = command;
                DataSet objDataset = new DataSet();
                objAdapter.Fill(objDataset);
                objConn.Close();
                System.IO.File.Delete(fileName);
                DataTable objTable = objDataset.Tables[0];
                Type temp = InvServiceFactory.GetInvoiceType(pattern, comID);
                string tempString = "";
                int i = 0;
                while (i < objTable.Rows.Count)
                {
                    DataRow dr = objTable.Rows[i];
                    string fkey = "";
                    StringBuilder sb = new StringBuilder("<Inv><key>");
                    if (dr.Table.Columns.Contains(Fkey))
                    {
                        tempString = dr[Fkey].ToString();
                        if (!string.IsNullOrEmpty(tempString))
                        {
                            fkey = tempString.Trim();
                            sb.Append(fkey);
                        }
                    }

                    sb.Append("</key><Invoice><CusCode>");
                    sb.Append(convertSpecialCharacter(dr[CusCode].ToString().Trim()));
                    sb.Append("</CusCode><CusName>");
                    sb.Append(convertSpecialCharacter(dr[CusName].ToString().Trim()));
                    sb.Append("</CusName><CusAddress>");
                    if (dr.Table.Columns.Contains(CusAddress))
                    {
                        sb.Append(convertSpecialCharacter(dr[CusAddress].ToString().Trim()));
                    }
                    sb.Append("</CusAddress><CusPhone>");
                    if (dr.Table.Columns.Contains(CusPhone))
                    {
                        sb.Append(dr[CusPhone].ToString().Trim());
                    }
                    sb.Append("</CusPhone><CusTaxCode>");
                    if (dr.Table.Columns.Contains(CusTaxCode))
                    {
                        sb.Append(dr[CusTaxCode].ToString().Trim());
                    }
                    sb.Append("</CusTaxCode><PaymentMethod>");
                    if (dr.Table.Columns.Contains(Kind_of_Payment))
                    {
                        sb.Append(convertSpecialCharacter(dr[Kind_of_Payment].ToString().Trim()));
                    }
                    sb.Append("</PaymentMethod><CusCom>");
                    if (dr.Table.Columns.Contains(CusCom))
                    {
                        sb.Append(convertSpecialCharacter(dr[CusCom].ToString().Trim()));
                    }
                    sb.Append("</CusCom><OrderNo>");
                    sb.Append(dr[OrderNo].ToString().Trim());
                    sb.Append("</OrderNo><OrderDate>");
                    sb.Append(dr[OrderDate].ToString().Trim());
                    sb.Append("</OrderDate><Products>");

                    int j = i;
                    do
                    {
                        DataRow drProduct = objTable.Rows[j];
                        sb.Append("<Product><Code>");
                        if (dr.Table.Columns.Contains(ProdCode))
                        {
                            sb.Append(convertSpecialCharacter(dr[ProdCode].ToString().Trim()));
                        }
                        sb.Append("</Code><ProdName>");
                        sb.Append(convertSpecialCharacter(drProduct[ProdName].ToString().Trim()));
                        sb.Append("</ProdName><ProdUnit>");
                        sb.Append(convertSpecialCharacter(drProduct[ProdUnit].ToString().Trim()));
                        sb.Append("</ProdUnit><ProdQuantity>");
                        tempString = drProduct[ProdQuantity].ToString().Trim();
                        sb.Append("</ProdQuantity><ProdPrice>");
                        sb.Append(drProduct[ProdPrice].ToString().Trim());
                        sb.Append("</ProdPrice><Amount>");
                        sb.Append(drProduct[ProdAmount].ToString().Trim());
                        sb.Append("</Amount><Total>");
                        sb.Append(drProduct[ProdTotal].ToString().Trim());
                        sb.Append("</Total><VATRate>");
                        sb.Append(drProduct[ProdVATRate].ToString().Trim());
                        sb.Append("</VATRate><VATAmount>");
                        sb.Append(drProduct[ProdVATAmount].ToString().Trim());
                        sb.Append("</VATAmount></Product>");
                        j++;
                    } while (j < objTable.Rows.Count && string.IsNullOrEmpty(objTable.Rows[j][CusName].ToString().Trim()));
                    sb.Append("</Products><Total>");
                    sb.Append(dr[Total].ToString().Trim());
                    sb.Append("</Total><DiscountAmount></DiscountAmount><VATRate></VATRate><VATAmount>");
                    sb.Append(dr[VAT_Amount].ToString().Trim());
                    sb.Append("</VATAmount><Amount>");
                    sb.Append(dr[Amount].ToString().Trim());
                    sb.Append("</Amount><AmountInWords>");
                    Int64 tongtien;
                    string tienBangChu = "";
                    if (Int64.TryParse(dr[Amount].ToString().Trim(), out tongtien))
                        tienBangChu = NumberToLeter.DocTienBangChu(tongtien);
                    sb.Append(tienBangChu);
                    sb.Append("</AmountInWords>");
                    if (dr.Table.Columns.Contains(ArisingDate))
                    {
                        sb.AppendFormat("<ArisingDate>{0}</ArisingDate>", dr[ArisingDate].ToString().Trim());
                    }
                    sb.Append("<GrossValue>");
                    sb.Append(dr[GrossValue].ToString().Trim());
                    sb.Append("</GrossValue><GrossValue0>");
                    sb.Append(dr[GrossValue0].ToString().Trim());
                    sb.Append("</GrossValue0><VatAmount0>0</VatAmount0><GrossValue5>");
                    sb.Append(dr[GrossValue5].ToString().Trim());
                    sb.Append("</GrossValue5><VatAmount5>");
                    sb.Append(dr[VatAmount5].ToString().Trim());
                    sb.Append("</VatAmount5><GrossValue10>");
                    sb.Append(dr[GrossValue10].ToString().Trim());
                    sb.Append("</GrossValue10><VatAmount10>");
                    sb.Append(dr[VatAmount10].ToString().Trim());
                    sb.Append("</VatAmount10>");

                    sb.Append("</Invoice></Inv>");
                    rv.AppendLine(sb.ToString());
                    i = j;
                }
            }
            catch (Exception ex)
            {
                log.Error("ParseIInvoice " + ex);
                message = ex.ToString();
                return null;
            }
            if (failtCount != 0) message = Failed.ToString();
            rv.Append("</Invoices>");
            return rv.ToString();
        }

        private string convertSpecialCharacter(string xmlData)
        {
            return "<![CDATA[" + xmlData + "]]>";
        }
    }
}
