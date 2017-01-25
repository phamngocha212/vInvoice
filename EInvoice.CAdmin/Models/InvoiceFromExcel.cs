using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EInvoice.CAdmin.Models
{
    public class InvoiceFromExcel
    {
        public int ThuocHoaDon { get; set; }
        public string TaiKhoanNganHang { get; set; }
        public string MaKhachHang { get; set; }
        public DateTime NgayHoaDon { get; set; }
        public string TenNguoiMuaHang { get; set; }
        public string TenKhachHang { get; set; }
        public string MaSoThue { get; set; }
        public string DonViTinh { get; set; }
        public Decimal SoLuong { get; set; }
        public Decimal DonGia { get; set; }
        public Decimal TongTien { get; set; }
        public Decimal TienThue { get; set; }
        public string DiaChiKhachHang { get; set; }
        public string GhiChu { get; set; }
    }
}