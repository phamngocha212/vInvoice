<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="Type"/>
  <xsl:template match="/">

    <div>
      <div style="border-bottom:none; margin-top:20px">
        <center>
          <b align="center">THỐNG KÊ TẠO LẬP, PHÁT HÀNH HÓA ĐƠN</b>
        </center>
      </div>
      <div>
        <div id="info" class="datetime">
          <br></br>
          <b>Mẫu số: </b>
          <xsl:value-of select="/TKTao_PH/Mau"/>
          <xsl:if test="TKTao_PH/KyHieu != ''" >
            <br></br>
            <b>Ký hiệu: </b>
            <xsl:value-of select="/TKTao_PH/KyHieu"/>
          </xsl:if>
          <xsl:if test="TKTao_PH/NguoiTao !=''">
            <br></br>
            <b>Người tạo: </b>
            <xsl:value-of select="TKTao_PH/NguoiTao"/>
          </xsl:if>
          <xsl:if test="/TKTao_PH/NguoiPH !=''">
            <br></br>
            <b>Người phát hành: </b>
            <xsl:value-of select="/TKTao_PH/NguoiPH"/>
          </xsl:if>
          <xsl:if test="/TKTao_PH/NgayPH !=''">
            <br></br>
            <b>Ngày phát hành: </b>
            <xsl:value-of select="/TKTao_PH/NgayPH"/>
          </xsl:if>
        </div>
        <br></br>
        <table border="1" style="width:100%; min-width:800px; margin-top:10px " class="grid" >
          <thead>
            <tr style="border: 1px solid #ccc;">
              <th width="50" align="center">
                <b>STT </b>
              </th>
              <th width="120" align="center">
                <b>Số hóa đơn</b>
              </th>
              <th width="150" align="center">
                <b>Mẫu số </b>
              </th>
              <th width="120" align="center">
                <b>Ký hiệu</b>
              </th>
              <th width="120" align="center">
                <b>Ngày tạo</b>
              </th>
              <th width="250" align="center">
                <b>Người tạo</b>
              </th>
              <th width="120" align="center">
                <b>Ngày phát hành</b>
              </th>
              <th width="250" align="center">
                <b>Người phát hành</b>
              </th>
            </tr>
          </thead>
          <tbody>
            <xsl:for-each select="//Items/Item">
              <tr style="border: 1px solid #ccc;">
                <td align="center">
                  <xsl:value-of select="position()+//Trang/@Trang * //Trang/@KT"/>


                  <td align="center" style="border-left: 1px solid #f0f0f0;">
                    <xsl:if test="$Type = 'xls'">
                      ="<xsl:value-of select="So"/>"
                    </xsl:if>
                    <xsl:if test="$Type != 'xls'">
                      <xsl:value-of select="So"/>
                    </xsl:if>
                  </td>


                </td>
                <td align="center" style="border-left: 1px solid #f0f0f0;">
                  <xsl:value-of select="Mau"/>
                </td>
                <td align="center" style="border-left: 1px solid #f0f0f0;">
                  <xsl:value-of select="KyHieu"/>
                </td>
                <td align="center" style="border-left: 1px solid #f0f0f0;" class="datetime">
                  <xsl:value-of select="NgayTao"/>
                </td>
                <td style="border-left: 1px solid #f0f0f0;" class="text">
                  <xsl:value-of select="NguoiTao"/>
                </td>
                <td align="center" style="border-left: 1px solid #f0f0f0;" class="datetime">
                  <xsl:value-of select="NgayPH"/>
                </td >
                <td style="border-left: 1px solid #f0f0f0;" class="text">
                  <xsl:value-of select="NguoiPH"/>
                </td >
              </tr>
            </xsl:for-each>
          </tbody>
        </table>
      </div>
    </div>
  </xsl:template>
</xsl:stylesheet>
