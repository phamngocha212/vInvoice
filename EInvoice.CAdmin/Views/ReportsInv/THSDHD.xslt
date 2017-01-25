<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template name="replace-string">
    <xsl:param name="text"/>
    <xsl:param name="replace"/>
    <xsl:param name="with"/>
    <xsl:choose>
      <xsl:when test="contains($text,$replace)">
        <xsl:value-of select="substring-before($text,$replace)"/>
        <xsl:value-of select="$with"/>
        <xsl:call-template name="replace-string">
          <xsl:with-param name="text" select="substring-after($text,$replace)"/>
          <xsl:with-param name="replace" select="$replace"/>
          <xsl:with-param name="with" select="$with"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$text"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="/">
    <div class="header-l" style="font-size:14px">
      <div class="national"  style="clear:left;width:100%;position:relative; ">
        
            <div style="font-size:17px;font-weight:bold;width:366px;text-align:center;clear:left;margin:0 auto;">
            
                CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM<br/>
                Độc lập - Tự do - Hạnh phúc<br/>
                -------------------------
              </div>

            <div   style="color:#006633;width:150px;text-align:center;position:absolute;top:30px;right:0">
              Mẫu số: BC26/AC <br/>
              (Ban hành kèm theo Thông tư số  
              39/2014/TT-BTC ngày 31/3/2014 của Bộ Tài chính)
          </div>
      </div>
      <div style="clear:left;width:400px;margin:0 auto">
        <br/>
        
      <br/>
        <center style="font-size:17px;font-weight:bold">
          BÁO CÁO TÌNH HÌNH SỬ DỤNG HÓA ĐƠN<br/>
          <i style="font-size:14px;font-weight:normal">
            <xsl:choose>
              <xsl:when test="/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/KyKKhaiThue/kieuKy ='M'">
                Kỳ tính thuế:Tháng <xsl:value-of select="substring(/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/KyKKhaiThue/kyKKhai,1,2)"/> năm <xsl:value-of select="substring(/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/KyKKhaiThue/kyKKhai,4,7)"/>
              </xsl:when>
              <xsl:otherwise>
                Kỳ tính thuế:Quý <xsl:value-of select="substring(/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/KyKKhaiThue/kyKKhai,1,1)"/> năm <xsl:value-of select="substring(/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/KyKKhaiThue/kyKKhai,3,7)"/>
              </xsl:otherwise>
            </xsl:choose>
          </i>
        </center>
      </div>
       
      <br/>
      <br/>
      <br/>
      <div  style="font-weight: bold" >
        1.Tên tổ chức (cá nhân):<xsl:value-of select="/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/NNT/tenNNT"></xsl:value-of>
        <br/>
        2. Mã số thuế:<xsl:value-of select="/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/NNT/mst"/>
        <br/>
        3. Địa chỉ:<xsl:value-of select="/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/NNT/dchiNNT"/>
      </div>
      <table>
        <tr>
          <td colspan="3">Kỳ báo cáo cuối cùng[]</td>
          <td colspan="3">Chuyển địa điểm[]</td>
        </tr>
        <tr>
          <td colspan="3">
            Ngày đầu kỳ báo cáo: <xsl:value-of select="/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/KyKKhaiThue/kyKKhaiTuNgay"/>,
          </td>
            <td colspan="3">
            Ngày cuối kỳ báo cáo: <xsl:value-of select="/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/KyKKhaiThue/kyKKhaiDenNgay"/>
          </td>
        </tr>
      </table>
      <table width="100%">
        <tr>
          <th colspan="22">
            <p style=" text-align: right;font-weight:normal">Đơn vị tính: số</p>
          </th>
        </tr>
      </table>
      <table width="100%" border="1" cellspacing="0" cellpadding="0" style=" border:1px solid #000">
        <thead>
          <tr>
            <th rowspan="4" width="10px">STT</th>
            <th rowspan="4" width="70px">Tên loại hóa đơn</th>
            <th rowspan="4" width="30px">Ký hiệu mẫu hóa đơn</th>
            <th rowspan="4" width="30px">Ký hiệu Hóa đơn</th>
            <th colspan="5" width="150px">Số tồn đầu kỳ, mua / phát hành trong kỳ</th>
            <th colspan="10">Số sử dụng, xóa bỏ, mất, hủy trong kỳ</th>
            <th colspan="3" rowspan="3" width="100px">Tồn cuối kỳ</th>
          </tr>
          <tr>
            <th rowspan="3" width="50px">Tổng số</th>
            <th rowspan="2" colspan="2" width="50px">Số tồn đầu kỳ</th>

            <th rowspan="2" colspan="2" width="100px">Số mua/ phát hành trong kỳ</th>
            <th rowspan="2" colspan="3">Tổng số sử dụng, xóa bỏ, mất, hủy</th>
            <th colspan="7">Trong đó</th>
          </tr>
          <tr>
            <th rowspan="2">
              Số lượng đã<br/> sử dụng
            </th>
            <th colspan="2">Xóa bỏ</th>
            <th colspan="2">Mất</th>
            <th colspan="2">Hủy</th>
          </tr>
          <tr>
            <th>Từ số</th>
            <th >Đến số</th>
            <th >Từ số</th>
            <th >Đến số</th>
            <th >Từ số</th>
            <th >Đến số</th>
            <th >Cộng</th>
            <th >Số lượng</th>
            <th class="textnowrap">Số</th>
            <th>Số lượng</th>
            <th >Số</th>
            <th>Số lượng</th>
            <th>Số</th>
            <th>Từ số</th>
            <th>Đến số</th>
            <th>Số lượng</th>
          </tr>
          <tr>
            <td style="text-align:center">1</td>
            <td style="width:150px;text-align:center">2</td>
            <td style="text-align:center">3</td>
            <td style="width:60px;text-align:center">4</td>
            <td style="width:60px; text-align:center">5</td>
            <td style="width:60px;text-align:center">6</td>
            <td style="width:60px;text-align:center">7</td>
            <td style="width:60px;text-align:center">8</td>
            <td style="width:60px;text-align:center">9</td>
            <td style="width:60px;text-align:center">10</td>
            <td style="width:60px;text-align:center">11</td>
            <td style="width:60px;text-align:center">12</td>
            <td style="width:60px;text-align:center">13</td>
            <td style="width:60px;text-align:center">14</td>
            <td style="width:60px;text-align:center">15</td>
            <td style="width:60px;text-align:center">16</td>
            <td style="width:60px;text-align:center">17</td>
            <td style="width:60px;text-align:center">18</td>
            <td style="width:60px;text-align:center">19</td>
            <td style="width:60px;text-align:center">20</td>
            <td style="width:60px;text-align:center">21</td>
            <td style="width:60px;text-align:center">22</td>
          </tr>
        </thead>
        <tbody>
          <xsl:for-each select="/HSoThueDTu/HSoKhaiThue/CTieuTKhaiChinh/HoaDon/ChiTiet">
            <tr  border="1" class="text">
              <td  style="text-align:center">
                <xsl:value-of select="position()"/>
              </td>
              <td  style="vertical-align: text-top" class="text">
                <xsl:value-of select="tenHDon"/>
              </td>
              <td  style="text-align:center;vertical-align: text-top">
                <xsl:value-of select="kHieuMauHDon"/>
              </td>
              <td  style="text-align:center;vertical-align: text-top">
                <xsl:value-of select="kHieuHDon"/>
              </td>
              <td  style="text-align:center;vertical-align: text-top">                
                <xsl:value-of select="translate(translate(translate(format-number(soTonMuaTrKy_tongSo, '###,###.##'),',','?'),'.',','),'?','.')"/>
              </td>
              <td id ="TonTuSo"  style="text-align:right;vertical-align: text-top">
                <xsl:choose>
                  <xsl:when test="soTonDauKy_tuSo=''">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="soTonDauKy_tuSo"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td  style="text-align:right;vertical-align: text-top">
                <xsl:choose>
                  <xsl:when test="soTonDauKy_denSo=''">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="soTonDauKy_denSo"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td  style="text-align:right;vertical-align: text-top">
                <xsl:choose>
                  <xsl:when test="muaTrongKy_tuSo = ''">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="muaTrongKy_tuSo"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td  style="text-align:right;vertical-align: text-top">
                <xsl:choose>
                  <xsl:when test="muaTrongKy_denSo = ''">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="muaTrongKy_denSo"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td  style="text-align:right;vertical-align: text-top">
                <xsl:choose>
                  <xsl:when test="tongSoSuDung_tuSo = ''">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="tongSoSuDung_tuSo"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td  style="text-align:right;vertical-align: text-top">
                <xsl:choose>
                  <xsl:when test="tongSoSuDung_denSo = ''">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="tongSoSuDung_denSo"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td  style="text-align:right;vertical-align: text-top">                
                <xsl:value-of select="translate(translate(translate(format-number(tongSoSuDung_cong, '###,###.##'),',','?'),'.',','),'?','.')"/>
              </td>
              <td  style="text-align:right;vertical-align: text-top">                
                <xsl:value-of select="translate(translate(translate(format-number(soDaSDung, '###,###.##'),',','?'),'.',','),'?','.')"/>
              </td>
              <td  style="text-align:right;vertical-align:text-top">
                <xsl:value-of select="xoaBo_soLuong"/>
              </td>
              <!--<td class="textnowrap">
                <xsl:choose>
                  <xsl:when test="DaDung/ChiTiet/Xoa/@So = ''">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="translate(DaDung/ChiTiet/Xoa/@So,',',' ')"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>-->
              <td class="textnowrap" style="text-align:right; word-break: keep-all; width:140px">
                <xsl:value-of select="xoaBo_so"/> 
              </td>
              <td  style="text-align:right;vertical-align: text-top">
                <xsl:value-of select="DaDung/ChiTiet/Mat/@SoLuong"/>
              </td>
              <td style="text-align:right">
                <xsl:choose>
                  <xsl:when test="DaDung/ChiTiet/Mat/@So = ''">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="DaDung/ChiTiet/Mat/@So"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td  style="width:20px;text-align:right;vertical-align: text-top" class="text">
                <xsl:value-of select="huy_soLuong"/>
              </td>
              <td  style="width:20px;text-align:right;vertical-align: text-top" class="text">
                <xsl:choose>
                  <xsl:when test="huy_so = ''">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="huy_so"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td  style="width:20px;text-align:right;vertical-align: text-top" class="text">
                <xsl:choose>
                  <xsl:when test="tonCuoiKy_tuSo = ''">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>                    
                    <xsl:value-of select="tonCuoiKy_tuSo"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td  style="width:60px;text-align:right;vertical-align: text-top" class="text">
                <xsl:choose>
                  <xsl:when test="tonCuoiKy_denSo = ''">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="tonCuoiKy_denSo"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td  style="width:20px;text-align:right;vertical-align: text-top" class="text">
                
                <xsl:value-of select="translate(translate(translate(format-number(tonCuoiKy_soLuong, '###,###.##'),',','?'),'.',','),'?','.')"/>&#160;
              </td>
            </tr>
          </xsl:for-each>
        </tbody>
      </table>
      <p style=" margin:10px 0 0 15px; text-align: left;">Cam kết báo cáo tình hình sử dụng hóa đơn trên đây là đúng sự thật, nếu có gì sai trái, đơn vị chịu hoàn toàn trách nhiệm trước pháp luật</p>
      <div>
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="report-used-bottom">
          <tr>
            <td></td>
            <td></td>
            <td></td>
          </tr>
          <tr>
            <td></td>
            <td>
              <center>
                <strong>Người lập biểu</strong>
                <br/>
                <i>(Ký, ghi rõ họ, tên)</i>
              </center>
            </td>
            <td></td>
            <td></td>
            <!--<td style="width:50%;" colspan="10">
              <center>
                <strong>Kế toán trưởng</strong>
                <br/>
                <i>(Ký, ghi rõ họ, tên)</i>
              </center>
            </td>-->
            <td colspan="10">
              <center>
                <span>
                  <i>
                    Ngày <xsl:value-of select="substring(/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/ngayLapTKhai,9,2)"/> tháng <xsl:value-of select="substring(/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/ngayLapTKhai,6,2)"/> năm <xsl:value-of select="substring(/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/ngayLapTKhai,1,4)"/>
                  </i>
                </span>
                <br />
                <strong>NGƯỜI ĐẠI DIỆN THEO PHÁP LUẬT</strong>
                <br/>
                <i>(Ký, ghi rõ họ, tên và đóng dấu)</i>
              </center>
            </td>
          </tr>
        </table >
      </div>
    </div>

  </xsl:template>

</xsl:stylesheet>
