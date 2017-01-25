using EInvoice.CAdmin.IService;
using EInvoice.CAdmin.Models;
using EInvoice.Core;
using EInvoice.Core.IService;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Web;

namespace EInvoice.CAdmin.ServiceImp
{
    public class ExcelDefaultDataTranService : IDataTranService
    {
        const string ThuocHoaDon = "ThuocHoaDon";
        const string MaKhachHang = "MaKhachHang";
        const string NgayHoaDon = "NgayHoaDon";
        const string TenKhachHang = "TenKhachHang";
        const string DiaChiKhachHang = "DiaChiKhachHang";
        const string MaSoThue = "MaSoThue";
        const string TaiKhoanNganHang = "TaiKhoanNganHang";
        const string DonViTinh = "DonViTinh";
        const string SoLuong = "SoLuong";
        const string DonGia = "DonGia";
        const string TongTien = "TongTien";
        const string TienThue = "TienThue";
        const string GhiChu = "GhiChu";

        ILog log = LogManager.GetLogger(typeof(ExcelDefaultDataTranService));
        public string ParseCustomer(byte[] data, ref string message)
        {
            //throw new NotImplementedException(); 

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

        public string ParseIInvoice(byte[] data, string pattern, string serial, int comID, string accountName, ref string message, ref int failtCount)
        {
            message = "";
            failtCount = 0;
            try
            {
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
                DataTable dataTable = objDataset.Tables[0];
                List<InvoiceFromExcel> listFromExcel = new List<InvoiceFromExcel>();
                foreach (DataRow dr in dataTable.Rows)
                {
                    if (dr != null)
                    {
                        InvoiceFromExcel invoiceFromExcel = new InvoiceFromExcel();
                        if (dr[ThuocHoaDon].ToString().Length > 0)
                            invoiceFromExcel.ThuocHoaDon = Convert.ToInt32(dr[ThuocHoaDon].ToString());
                        invoiceFromExcel.MaKhachHang = dr[MaKhachHang].ToString().Trim();
                        invoiceFromExcel.MaSoThue = dr[MaSoThue].ToString();
                        string cusName = dr[TenKhachHang].ToString();
                        if (!string.IsNullOrWhiteSpace(invoiceFromExcel.MaSoThue))
                            invoiceFromExcel.TenKhachHang = cusName;
                        else
                            invoiceFromExcel.TenNguoiMuaHang = !string.IsNullOrWhiteSpace(cusName) ? cusName : "Người mua không lấy hóa đơn";

                        if (dr[NgayHoaDon].ToString().Length > 0)
                        {
                            string ngayHoaDon = dr[NgayHoaDon].ToString().Split(' ')[0];
                            invoiceFromExcel.NgayHoaDon = DateTime.ParseExact(ngayHoaDon, "dd/MM/yyyy", null);
                        }

                        invoiceFromExcel.DiaChiKhachHang = dr[DiaChiKhachHang].ToString();
                        invoiceFromExcel.DonViTinh = dr[DonViTinh].ToString();
                        if (dr[SoLuong].ToString().Length > 0)
                            invoiceFromExcel.SoLuong = Convert.ToDecimal(dr[SoLuong].ToString().Replace(".", "").Replace(",", "."));
                        if (dr[DonGia].ToString().Length > 0)
                            invoiceFromExcel.DonGia = Convert.ToDecimal(dr[DonGia].ToString().Replace(".", "").Replace(",", "."));
                        if (dr[TongTien].ToString().Length > 0)
                            invoiceFromExcel.TongTien = Convert.ToDecimal(dr[TongTien].ToString().Replace(".", "").Replace(",", "."));
                        if (dr[TienThue].ToString().Length > 0)
                            invoiceFromExcel.TienThue = Convert.ToDecimal(dr[TienThue].ToString().Replace(".", "").Replace(",", "."));
                        invoiceFromExcel.DiaChiKhachHang = dr[DiaChiKhachHang].ToString();
                        invoiceFromExcel.TaiKhoanNganHang = dr[TaiKhoanNganHang].ToString();
                        invoiceFromExcel.GhiChu = dr[GhiChu].ToString();
                        listFromExcel.Add(invoiceFromExcel);
                    }
                }

                //var query = from r in dataTable.AsEnumerable()
                //            select new InvoiceFromExcel()
                //            {                                
                //                NgayHoaDon = r.Field<string>(NgayHoaDon),
                //                TenKhachHang = r.Field<string>(TenKhachHang),
                //                MaSoThue = r.Field<string>(MaSoThue),
                //                TongTien = r.Field<string>(TongTien),
                //                TienThue = r.Field<Double>(TienThue) > 0? r.Field<Double>(TienThue) : 0,
                //                DiaChiKhachHang = r.Field<string>(DiaChiKhachHang),
                //                TaiKhoanNganHang = r.Field<string>(TaiKhoanNganHang),
                //                GhiChu = r.Field<string>(GhiChu)
                //            };                
                return makeStringListInvoice(listFromExcel.OrderBy(p => p.NgayHoaDon).ToList(), out message);
            }
            catch (Exception ex)
            {
                log.Error("ParseIInvoice " + ex);
                message = ex.ToString();
                return null;
            }
        }

        private string makeStringListInvoice(List<InvoiceFromExcel> invoices, out string ErrorMessage)
        {
            try
            {
                ErrorMessage = "";
                StringBuilder xmlStringData = new StringBuilder("<Invoices>");
                xmlStringData.AppendLine("<BillTime></BillTime>");
                var listGroupbySoHD = invoices.GroupBy(u => new { u.NgayHoaDon, u.MaSoThue, u.MaKhachHang, u.ThuocHoaDon });
                foreach (var item in listGroupbySoHD)
                {
                    List<InvoiceFromExcel> lstbyNo = item.ToList();
                    if (lstbyNo.Count == 0) continue;
                    string xmlData = makeStringInvoice(lstbyNo, out ErrorMessage);
                    if (xmlData == null)
                        return null;
                    xmlStringData.AppendLine(xmlData);
                }
                xmlStringData.Append("</Invoices>");
                return xmlStringData.ToString();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private string makeStringInvoice(List<InvoiceFromExcel> invoicesByNo, out string ErrorMessage)
        {
            ErrorMessage = "";
            try
            {
                Decimal totalInvoice = Convert.ToDecimal(invoicesByNo.Sum(p => p.TongTien));
                decimal vatAmount = 0;
                InvoiceFromExcel invoiceFirst = invoicesByNo.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(invoiceFirst.MaKhachHang))
                {
                    ErrorMessage = "Thiếu thông tin: Mã khách hàng";
                    return null;
                }
                if (string.IsNullOrWhiteSpace(invoiceFirst.TenKhachHang) && string.IsNullOrWhiteSpace(invoiceFirst.TenNguoiMuaHang))
                {
                    ErrorMessage = "Thiếu thông tin: Tên khách hàng";
                    return null;
                }

                StringBuilder sbInvoice = new StringBuilder("<Inv>");
                sbInvoice.Append("<key/><Invoice>");
                sbInvoice.AppendFormat("<CusCode>{0}</CusCode>", invoiceFirst.MaKhachHang);
                sbInvoice.AppendFormat("<Buyer>{0}</Buyer>", convertSpecialCharacter(invoiceFirst.TenNguoiMuaHang));
                sbInvoice.AppendFormat("<CusName>{0}</CusName>", convertSpecialCharacter(invoiceFirst.TenKhachHang));
                sbInvoice.AppendFormat("<CusAddress>{0}</CusAddress>", convertSpecialCharacter(invoiceFirst.DiaChiKhachHang));
                sbInvoice.AppendFormat("<CusTaxCode>{0}</CusTaxCode>", invoiceFirst.MaSoThue);
                sbInvoice.Append("<PaymentMethod>TM/CK</PaymentMethod>");

                sbInvoice.Append(makeStringProducts(invoicesByNo, out vatAmount));

                sbInvoice.AppendFormat("<ArisingDate>{0}</ArisingDate>", invoiceFirst.NgayHoaDon.ToString("dd/MM/yyyy"));
                sbInvoice.AppendFormat("<Total>{0}</Total>", totalInvoice);
                sbInvoice.Append("<VATRate>0</VATRate>");
                sbInvoice.AppendFormat("<VATAmount>{0}</VATAmount>", vatAmount);
                long amountInvoice = (long)(totalInvoice + vatAmount);
                sbInvoice.AppendFormat("<Amount>{0}</Amount>", amountInvoice);
                string tienbangchu = NumberToLeter.DocTienBangChu(amountInvoice);
                sbInvoice.AppendFormat("<AmountInWords>{0}</AmountInWords>", tienbangchu);
                sbInvoice.Append("</Invoice></Inv>");
                return sbInvoice.ToString();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ErrorMessage = ex.Message;
                return null;
            }
        }

        private string makeStringProducts(List<InvoiceFromExcel> invoicesByNo, out decimal vatAmount)
        {
            vatAmount = 0;
            StringBuilder strProducts = new StringBuilder("<Products>");
            foreach (var inv in invoicesByNo)
            {
                Decimal tongTien = inv.TongTien;
                Decimal tienThue = inv.TienThue;
                decimal vatRate = tienThue > 0 ? Math.Round(tongTien / tienThue) : 0;
                vatAmount += tienThue;
                strProducts.Append("<Product>");
                strProducts.AppendFormat("<ProdName>{0}</ProdName>", convertSpecialCharacter(inv.GhiChu));
                strProducts.AppendFormat("<ProdUnit>{0}</ProdUnit>", convertSpecialCharacter(inv.DonViTinh));
                strProducts.AppendFormat("<ProdQuantity>{0}</ProdQuantity>", inv.SoLuong);
                strProducts.AppendFormat("<ProdPrice>{0}</ProdPrice>", inv.DonGia);
                strProducts.AppendFormat("<Total>{0}</Total>", tongTien);
                strProducts.AppendFormat("<VATRate>{0}</VATRate>", vatRate);
                strProducts.AppendFormat("<VATAmount>{0}</VATAmount>", tienThue);
                strProducts.AppendFormat("<Amount>{0}</Amount>", tongTien + tienThue);
                strProducts.Append("</Product>");
            }
            strProducts.Append("</Products>");
            return strProducts.ToString();
        }

        private string convertSpecialCharacter(string xmlData)
        {
            return "<![CDATA[" + xmlData + "]]>";
        }
    }
}