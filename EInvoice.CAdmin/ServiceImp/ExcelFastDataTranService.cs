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
    public class ExcelFastDataTranService : IDataTranService
    {
        ILog log = LogManager.GetLogger(typeof(ExcelFastDataTranService));
        private bool cusValidate(Customer cus, ref string message)
        {
            message = "";
            return true;
        }
        public string ParseCustomer(byte[] data, ref string message)
        {
            StringBuilder rv = new StringBuilder("<Customers>");
            message = "";
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
                message = ex.Message;
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

        public string ParseIInvoice(byte[] data, string pattern, string serial, int comID, string accountName, ref string mesage, ref int failtCount)
        {
            StringBuilder rv = new StringBuilder("<Invoices>");
            rv.AppendLine("<BillTime></BillTime>");
            mesage = "";
            failtCount = 0;
            StringBuilder Failed = new StringBuilder("<Invoices>");
            try
            {
                const string Fkey = "Fkey";
                const string ArasingDate = "NgayHoaDon";
                const string TenKhachHang = "TenKhachHang";
                const string MaKhachHang = "MaKhachHang";
                const string DiaChiKhachHang = "DiaChiKhachHang";
                const string DienThoaiKhachHang = "DienThoaiKhachHang";
                const string MaSoThue = "MaSoThue";
                const string HinhThucTT = "HinhThucTT";
                const string KyHoaDon = "KyHoaDon";
                const string SanPham = "SanPham";
                const string DonViTinh = "DonViTinh";
                const string SoLuong = "SoLuong";
                const string DonGia = "DonGia";
                const string ThanhTien = "ThanhTien";
                const string TienBan = "TienBan";
                const string ThueSuat = "ThueSuat";
                const string TienThue = "TienThue";
                const string TongCong = "TongCong";
                const string SoSanPham = "SoSanPham";
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
                    string cusCode = dr[MaKhachHang].ToString().Trim();
                    sb.Append(convertSpecialCharacter(cusCode));
                    sb.Append("</CusCode><CusName>");
                    sb.Append(convertSpecialCharacter(dr[TenKhachHang].ToString().Trim()));
                    sb.Append("</CusName><CusAddress>");
                    if (dr.Table.Columns.Contains(DiaChiKhachHang))
                    {
                        sb.Append(convertSpecialCharacter(dr[DiaChiKhachHang].ToString().Trim()));
                    }
                    sb.Append("</CusAddress><CusPhone>");
                    if (dr.Table.Columns.Contains(DienThoaiKhachHang))
                    {
                        sb.Append(dr[DienThoaiKhachHang].ToString().Trim());
                    }
                    sb.Append("</CusPhone><CusTaxCode>");
                    sb.Append(dr[MaSoThue].ToString().Trim());
                    sb.Append("</CusTaxCode><PaymentMethod>");
                    if (dr.Table.Columns.Contains(HinhThucTT))
                    {
                        sb.Append(convertSpecialCharacter(dr[HinhThucTT].ToString().Trim()));
                    }
                    sb.Append("</PaymentMethod><KindOfService>");
                    if (dr.Table.Columns.Contains(KyHoaDon))
                    {
                        sb.Append(convertSpecialCharacter(dr[KyHoaDon].ToString().Trim()));
                    }
                    sb.Append("</KindOfService><Products>");
                    int j = i;
                    do
                    {
                        DataRow drProduct = objTable.Rows[j];
                        sb.Append("<Product><ProdName>");
                        sb.Append(convertSpecialCharacter(drProduct[SanPham].ToString().Trim()));
                        sb.Append("</ProdName><ProdUnit>");
                        sb.Append(convertSpecialCharacter(drProduct[DonViTinh].ToString().Trim()));
                        sb.Append("</ProdUnit><ProdQuantity>");
                        tempString = drProduct[SoLuong].ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(tempString))
                        {
                            sb.Append(tempString.Replace(".", "").Replace(",", "."));
                        }
                        sb.Append("</ProdQuantity><ProdPrice>");
                        sb.Append(drProduct[DonGia].ToString().Trim());
                        sb.Append("</ProdPrice><Amount>");
                        sb.Append(drProduct[ThanhTien].ToString().Trim());
                        sb.Append("</Amount></Product>");
                        j++;
                    } while (j < objTable.Rows.Count && string.IsNullOrEmpty(objTable.Rows[j][TenKhachHang].ToString().Trim()));
                    sb.Append("</Products><Total>");
                    sb.Append(dr[TienBan].ToString().Trim());
                    sb.Append("</Total><DiscountAmount></DiscountAmount><VATRate>");
                    sb.Append(dr[ThueSuat].ToString().Trim());
                    sb.Append("</VATRate><VATAmount>");
                    sb.Append(dr[TienThue].ToString().Trim());
                    sb.Append("</VATAmount><Amount>");
                    sb.Append(dr[TongCong].ToString().Trim());
                    sb.Append("</Amount><AmountInWords>");
                    Int64 tongtien;
                    string tienBangChu = "";                    
                    if (Int64.TryParse(dr[TongCong].ToString().Trim(), out tongtien))
                        tienBangChu = NumberToLeter.DocTienBangChu(tongtien);
                    sb.Append(tienBangChu);
                    sb.Append("</AmountInWords>");
                    if (dr.Table.Columns.Contains(ArasingDate))
                    {
                        sb.AppendFormat("<ArisingDate>{0}</ArisingDate>", dr[ArasingDate].ToString().Trim());
                    }
                    sb.Append("</Invoice></Inv>");
                    rv.AppendLine(sb.ToString());
                    i=j;
                }
            }
            catch (Exception ex)
            {
                log.Error("ParseIInvoice " + ex);
                mesage = ex.ToString();
                return null;
            }
            if (failtCount != 0) mesage = Failed.ToString();
            rv.Append("</Invoices>");
            return rv.ToString();
        }

        private string convertSpecialCharacter(string xmlData)
        {
            return "<![CDATA[" + xmlData + "]]>";
        }
    }
}
