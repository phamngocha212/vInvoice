<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="Type"/>
  <xsl:template match="/">

    <div>
      <div style="border-bottom:none; margin-top:20px;">
        <center >
          <b align="center">
            THỐNG KÊ CHI TIẾT HÓA ĐƠN
          </b>
        </center>
      </div>
      <div>
        <div id ="info">
          <br id ="Pattern" ></br>
          <b> Mẫu số: </b>
          <xsl:value-of select="/TKChiTiet/Mau"/>
          <xsl:if test="TKChiTiet/KyHieu != ''" >
            <br id ="Serial"></br>
            <b>Ký hiệu: </b>
            <xsl:value-of select="/TKChiTiet/KyHieu"/>
          </xsl:if>
          <xsl:if test="TKChiTiet/TT !=''">
            <br id ="Status">   </br>
            <b>Trạng thái: </b>
            <xsl:value-of select="TKChiTiet/TT"/>
          </xsl:if>
          <xsl:if test="/TKChiTiet/TuNgay !=''">
            <br></br>
            <b>Từ ngày: </b>
            <xsl:value-of select="/TKChiTiet/TuNgay"/>
            <b>  Đến ngày: </b>
            <xsl:value-of select="/TKChiTiet/DenNgay"/>
          </xsl:if>
        </div>
        <br></br>
        <table border="1" style="width:100%; min-width:800px; margin-top:10px;"  class="grid">
          <thead>
            <tr style="border: 1px solid #ccc;">
              <th width="50" align="center">
                <b>STT</b>
              </th>
              <th width="130" align="center">
                <b>Mẫu</b>
              </th>
              <th width="120" align="center">
                <b>Ký hiệu</b>
              </th>
              <th width="120" align="center">
                <b>Số</b>
              </th>
              <th width="150" align="center">
                <b>Mã khách hàng</b>
              </th>
              <th align="center">
                <b>Tên khách hàng</b>
              </th>
              <th width="150" align="center">
                <b>Ngày phát hành</b>
              </th>
              <th width="150" align="center">
                <b>Tổng tiền</b>
              </th>
              <th width="200" align="center">
                <b>Trạng thái</b>
              </th>
            </tr>
          </thead>
          <tbody>
            <xsl:for-each select="//Items/Item">
              <tr style="border: 1px solid #ccc;">
                <td align="center" >
                  <xsl:value-of select="position() + //Trang/@Trang * //Trang/@KT"/>
                </td>
                <td align="center" style="border-left: 1px solid #f0f0f0;">
                  <xsl:value-of select="Mau"/>
                </td>
                <td align="center" style="border-left: 1px solid #f0f0f0;">
                  <xsl:value-of select="KyHieu"/>
                </td>
                <td align="center" style="border-left: 1px solid #f0f0f0;">
                  <xsl:if test="$Type = 'xls'">
                    ="<xsl:value-of select="So"/>"
                  </xsl:if>
                  <xsl:if test="$Type != 'xls'">
                    <xsl:value-of select="So"/>
                  </xsl:if>
                </td>
                <td style="border-left: 1px solid #f0f0f0;" class="text">
                  <xsl:value-of select="MaKH"/>
                </td>
                <td style="border-left: 1px solid #f0f0f0;" class="text">
                  <xsl:value-of select="TenKH"/>
                </td>
                <td align="center" style="border-left: 1px solid #f0f0f0;" class="datetime">
                  <xsl:value-of select="NgayPH"/>
                </td>
                <td style="border-left: 1px solid #f0f0f0;" class="text">
                  <xsl:value-of select="TongTien"/>                                   
                </td>
                <td align="center" style="border-left: 1px solid #f0f0f0;">
                  <xsl:value-of select="TT"/>
                </td >
              </tr>
            </xsl:for-each>
          </tbody>
        </table>
      </div>
    </div>
  </xsl:template>

</xsl:stylesheet>
