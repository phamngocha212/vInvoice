<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
      <div class="report-used-vat" style =" width:1000px;margin 0px auto;font-family:Arial;font-size:13px">
          <ul style="list-style: none;width:904px;text-align: center;margin-left:222px">
            <li style="float:left;line-height: 20px;">
              <center>
                <b>PHỤ LỤC</b>
                <br/>
                <b>BẢNG KÊ HOÁ ĐƠN, CHỨNG TỪ HÀNG HOÁ, DỊCH VỤ BÁN RA</b>
                <br/>
                (Kèm theo tờ khai thuế GTGT theo mẫu số 01/GTGT)
                <br/>
                <b>[1]</b> Kỳ tính thuế: tháng <xsl:value-of select="substring(//HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/KyKKhaiThue/kyKKhai,1,2)"/> năm <xsl:value-of select="substring(HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/KyKKhaiThue/kyKKhai,4,6)"/>
                <br/>
              </center>
            </li>
            <li style="line-height: 20px;">
              Mẫu số:<b> 01- 1/GTGT</b><br/>
              <i>
                (Ban hành kèm theo Thông tư số<br/>
                119/2014/TT-BTC ngày 25/8/2014 <br/>
                của Bộ Tài chính)
              </i>
            </li>
          </ul>
          <div style="margin-top:20px;line-height: 24px;">
            <b>[02] Tên người nộp thuế:</b><xsl:value-of select="//HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/NNT/tenNNT"/>
            <br/>
            <b>[03]</b> Mã số thuế: <xsl:value-of select="//HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/NNT/mst"/>
            <br/>
            <b>[04] Tên đại lý thuế(nếu có):</b>
            <br/>
            <b>[05]</b> Mã số thuế
          </div>
      <div style="width:100%;text-align:right" >
        <p>Đơn vị tiền: đồng Việt Nam</p>
      </div>
      <table width="100%" border="0" cellspacing="0" cellpadding="0" class="report-used-list" style=" border:thin solid #000;">
        <thead>
          <tr style=" border:thin solid #000;">
            <th rowspan="2">STT</th>
            <th colspan="2">Hóa đơn, chứng từ bán</th>
            <th rowspan="2">Tên người mua</th>
            <th rowspan="2">Mã số thuế người mua</th>
            <th rowspan="2">Doanh số bán chưa có thuế</th>
            <th rowspan="2">Thuế GTGT</th>
            <th rowspan="2">Ghi chú</th>
          </tr>
          <tr style=" border:thin solid #000;">
            <th>Số hóa đơn</th>
            <th>Ngày, tháng, năm phát hành</th>
          </tr>
        </thead>
        <tbody>
          <tr style=" border:thin solid #000;">
            <td style ="text-align:center; width: 50px">
              <i >(1)</i>
            </td>
            <td style ="text-align:center; width: 70px">
              <i >(2)</i>
            </td>
            <td style ="text-align:center; width: 80px">
              <i >(3)</i>
            </td>
            <td style ="text-align:center; width: 200px">
              <i >(4)</i>
            </td>
            <td style ="text-align:center; width: 110px">
              <i >(5)</i>
            </td>
            <td style ="text-align:center; width: 200px">
              <i >(6)</i>
            </td>
            <td style ="text-align:center; width: 100px">
              <i >(7)</i>
            </td>
            <td style ="text-align:center; width: 100px">
              <i >(8)</i>
            </td>
          </tr>
          <!--không chịu thuế-->
          <tr style=" border:thin solid #000;">
            <td colspan ="8">
              <i>1. Hàng hoá, dịch vụ không chịu thuế giá trị gia tăng (GTGT):</i>
            </td>
          </tr>
          <xsl:for-each select="//HSoThueDTu/HSoKhaiThue/PLuc/PL01_1_GTGT/HHDVKChiuThue/ChiTietHHDVKChiuThue/HDonBRa">
            <tr style=" border:thin solid #000;">
              <td style ="text-align:center;">
                <xsl:value-of select="position()"/>
              </td>
              <td style ="text-align:center;" class="text">
                <xsl:value-of select="soHDon"/>
              </td>
              <td style ="text-align:center;" class="datetime">
                <xsl:value-of select="ngayPHanh"/>
              </td>
              <td style ="text-align:left;">
                <xsl:value-of select="tenNMUA"/>
              </td>
              <td style ="text-align:left;" class="text">
                <xsl:choose>
                  <xsl:when test="mstNMUA ='' ">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="mstNMUA"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td style="text-align: right;" class="text">
                <xsl:value-of select="dsoBanChuaThue"/>
              </td>
              <td style ="text-align: right" class="text">
                <xsl:value-of select="thueGTGT"/>
              </td>
              <td style ="text-align:left;">
                <xsl:value-of select="ghiChu"/>
              </td>
            </tr>
          </xsl:for-each>
          <tr id ="Sum" style=" border:thin solid #000;">
            <td colspan ="5">
              <b>Tổng Cộng</b>
            </td>
            <td style ="text-align: right" class="text">
              <b>
                <xsl:value-of select="translate(translate(translate(format-number(//tongDThuBRaKChiuThue, '###,###.##'),',','?'),'.',','),'?','.')"/>
              </b>
            </td>
            <td style ="text-align: right" class="text">
              <!--<b>
                  <xsl:value-of select="//HD0VAT/@TongThue"/>
                </b>-->
            </td>
            <td>&#160;</td>
          </tr>
          <!--Thuế 0%-->
          <tr style=" border:thin solid #000;">
            <td colspan ="8">
              <i>2. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 0%:</i>
            </td>
          </tr>
          <xsl:for-each select="/HSoThueDTu/HSoKhaiThue/PLuc/PL01_1_GTGT/HHDVThue0/ChiTietHHDVThue0/HDonBRa">
            <tr style=" border:thin solid #000;">
              <td style ="text-align: center">
                <xsl:value-of select="position()"/>
              </td>
              <td style ="text-align:center" class="text">
                <xsl:value-of select="soHDon"/>
              </td>
              <td style ="text-align:center" class="datetime">
                <xsl:value-of select="ngayPHanh"/>
              </td>
              <td style ="text-align:left">
                <xsl:value-of select="tenNMUA"/>
              </td>
              <td style ="text-align:left" class="text">
                <xsl:choose>
                  <xsl:when test="mstNMUA ='' ">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="mstNMUA"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td style ="text-align: right" class="text">
                <xsl:value-of select="dsoBanChuaThue"/>
              </td>
              <td style ="text-align: right" class="text">
                <xsl:value-of select="thueGTGT"/>
              </td>
              <td>
                <xsl:choose>
                  <xsl:when test="ghiChu ='' ">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="ghiChu"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
            </tr>
          </xsl:for-each>
          <tr id ="Sum" style=" border:thin solid #000;">
            <td colspan ="5">
              <b>Tổng Cộng</b>
            </td>
            <td style ="text-align: right" class="text">
              <b>
                <xsl:value-of select="translate(translate(translate(format-number(//tongDThuBRaThue0, '###,###.##'),',','?'),'.',','),'?','.')"/>
              </b>
            </td>
            <td style ="text-align: right" class="text">
              <b>
                <xsl:value-of select="translate(translate(translate(format-number(//tongThueBRaThue0, '###,###.##'),',','?'),'.',','),'?','.')"/>
              </b>
            </td>
            <td>&#160;</td>
          </tr>
          <!--Thuế 5%-->
          <tr style=" border:thin solid #000;">
            <td colspan ="8">
              <i>3. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 5%:</i>
            </td>
          </tr>
          <xsl:for-each select="/HSoThueDTu/HSoKhaiThue/PLuc/PL01_1_GTGT/HHDVThue5/ChiTietHHDVThue5/HDonBRa">
            <tr style=" border:thin solid #000;">
              <td style ="text-align: center">
                <xsl:value-of select="position()"/>
              </td>
              <td style ="text-align:center" class="text">
                <xsl:value-of select="soHDon"/>
              </td>
              <td style ="text-align:center" class="datetime">
                <xsl:value-of select="ngayPHanh"/>
              </td>
              <td style ="text-align:left">
                <xsl:value-of select="tenNMUA"/>
              </td>
              <td style ="text-align:left" class="text">
                <xsl:choose>
                  <xsl:when test="mstNMUA ='' ">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="mstNMUA"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td style ="text-align: right" class="text">
                <xsl:value-of select="dsoBanChuaThue"/>
              </td>
              <td style ="text-align: right" class="text">
                <xsl:value-of select="thueGTGT"/>
              </td>
              <td>
                <xsl:choose>
                  <xsl:when test="ghiChu ='' ">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="ghiChu"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
            </tr>
          </xsl:for-each>
          <tr id ="Sum" style=" border:thin solid #000;">
            <td colspan ="5">
              <b>Tổng Cộng</b>
            </td>
            <td style ="text-align: right" class="text">
              <b>
                <xsl:value-of select="translate(translate(translate(format-number(//tongDThuBRaThue5, '###,###.##'),',','?'),'.',','),'?','.')"/>
              </b>
            </td>
            <td style ="text-align: right" class="text">
              <b>
                <xsl:value-of select="translate(translate(translate(format-number(//tongThueBRaThue5, '###,###.##'),',','?'),'.',','),'?','.')"/>
              </b>
            </td>
            <td>&#160;</td>
          </tr>
          <!--Thuế 10%-->
          <tr style=" border:thin solid #000;">
            <td colspan ="8">
              <i>4. Hàng hoá, dịch vụ chịu thuế suất thuế GTGT 10%:</i>
            </td>
          </tr>
          <xsl:for-each select="/HSoThueDTu/HSoKhaiThue/PLuc/PL01_1_GTGT/HHDVThue10/ChiTietHHDVThue10/HDonBRa">
            <tr style=" border:thin solid #000;">
              <td style ="text-align: center">
                <xsl:value-of select="position()"/>
              </td>
              <td style ="text-align:center" class="text">
                <xsl:value-of select="soHDon"/>
              </td>
              <td style ="text-align:center" class="datetime">
                <xsl:value-of select="ngayPHanh"/>
              </td>
              <td style ="text-align:left">
                <xsl:value-of select="tenNMUA"/>
              </td>
              <td style ="text-align:left" class="text">
                <xsl:choose>
                  <xsl:when test="mstNMUA ='' ">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="mstNMUA"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td style ="text-align: right" class="text">
                <xsl:value-of select="dsoBanChuaThue"/>
              </td>
              <td style ="text-align: right" class="text">
                <xsl:value-of select="thueGTGT"/>
              </td>
              <td>
                <xsl:choose>
                  <xsl:when test="ghiChu ='' ">
                    &#160;
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="ghiChu"/>
                  </xsl:otherwise>
                </xsl:choose>
              </td>
            </tr>
          </xsl:for-each>
          <tr id ="Sum" style=" border:thin solid #000;">
            <td colspan ="5">
              <b>Tổng Cộng</b>
            </td>
            <td style ="text-align: right" class="text">
              <b>
                <xsl:value-of select="translate(translate(translate(format-number(//tongDThuBRaThue10, '###,###.##'),',','?'),'.',','),'?','.')"/>
              </b>
            </td>
            <td style ="text-align: right" class="text">
              <b>
                <xsl:value-of select="translate(translate(translate(format-number(//tongThueBRaThue10, '###,###.##'),',','?'),'.',','),'?','.')"/>
              </b>
            </td>
            <td>&#160;</td>
          </tr>
        </tbody>
      </table>
      <p>
       <b>Tổng doanh thu hàng hoá, dịch vụ bán ra chịu thuế GTGT (*):</b>&#160;&#160;
        <b>
          <xsl:value-of select="translate(translate(translate(format-number(//HSoThueDTu/HSoKhaiThue/PLuc/PL01_1_GTGT/tongDThuBRa, '###,###.##'),',','?'),'.',','),'?','.')"/>
        </b>
      </p>
      <p>
       <b> Tổng số thuế GTGT của hàng hóa, dịch vụ bán ra(**):</b>&#160;&#160; <b>
         <xsl:value-of select="translate(translate(translate(format-number(//HSoThueDTu/HSoKhaiThue/PLuc/PL01_1_GTGT/tongThueBRa, '###,###.##'),',','?'),'.',','),'?','.')"/>
        </b>
      </p>
      <p>Tôi cam đoan số liệu khai trên là đúng và chịu trách nhiệm trước pháp luật về những số liệu đã khai./... </p>
        <ul style="list-style:none">
           <li style="float:left;padding-right:280px">
             <b>NHÂN VIÊN ĐẠI LÝ THUẾ</b>
             <br/>Họ và tên:
             <br/>Chứng chỉ hành nghề số:
           </li>
           <li style="text-align:center">
               <span>
                 Hà nội,Ngày <xsl:value-of select="substring(/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/ngayLapTKhai,9,2)"/> tháng <xsl:value-of select="substring(/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/ngayLapTKhai,6,2)"/> năm <xsl:value-of select="substring(/HSoThueDTu/HSoKhaiThue/TTinChung/TTinTKhaiThue/TKhaiThue/ngayLapTKhai,1,4)"/>
               </span>
               <BR/>
               <strong>
                 NGƯỜI NỘP THUẾ hoặc <br/>
                 ĐẠI DIỆN HỢP PHÁP CỦA NGƯỜI NỘP THUẾ
               </strong>
               <br/>
               <span style="padding-left:450px">Ký, ghi rõ họ tên, chức vụ và đóng dấu (nếu có)</span>
           </li>
        </ul>
        <br/>
        <br/>
        <strong style=" text-decoration:underline">Ghi chú:</strong>
        <p>(*) Tổng doanh thu hàng hóa, dịch vụ bán ra chịu thuế GTGT là tổng cộng số liệu tại cột 6 của dòng tổng của các chỉ tiêu 2, 3, 4.</p>
        <p>(**) Tổng số thuế GTGT của hàng hóa, dịch vụ bán ra là tổng cộng số liệu tại cột 7 của dòng tổng của các chỉ tiêu 2, 3, 4.</p>
    </div>
  </xsl:template>
</xsl:stylesheet>
