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

			<div class="reportHeader">
				<style>
					table{
					font-size:16px !important;
					}
					table tr td:first-child{
					text-align:center;

					}
					table tbody tr td:nth-child(4), table thead tr th{text-align:center}
					table tbody tr td:last-child{
					text-align:right;
					}
					table tr td, th{
					padding:5px !important;
					}
					table{
					border-collapse:collapse;
					}
					table thead{
					background:#dddddd;
					}
				</style>
				<div style="width:250px;text-align:center;float:left;font-size:16px;padding-top:20px;">
					BỘ Y TẾ <br/>  <br/>
					BỆNH VIỆN MẮT TRUNG ƯƠNG <br/>
					----------oOo----------
				</div>
				<div style="width:350px;text-align:center;float:right;">
					<h3>
						<xsl:value-of select="/BaoCao/TenBaoCao"/>
					</h3>
					<p>
						Ngày <xsl:value-of select="substring(/BaoCao/NgayBaoCao,1,2)"/>
						tháng <xsl:value-of select="substring(/BaoCao/NgayBaoCao,4,2)"/>
						năm <xsl:value-of select="substring(/BaoCao/NgayBaoCao,7,9)"/>
					</p>
				</div>
			</div>
			<div class="reportBody" style="width:100%;clear:left;">
				<table class="reportsTable" width="100%" border="1" cellspacing="0" cellpadding="0" style=" border:1px solid #000">
					<thead style="background:#dddddd;">
						<tr>
							<th width="20px">STT</th>
							<th width="150px">Mẫu số</th>
							<th width="150px">Ký hiệu</th>
							<th width="150px">Số hóa đơn</th>
							<th width="300px">Tên bệnh nhân</th>
							<th width="300px">Thành tiền</th>
						</tr>
					</thead>
					<tbody class="reportsData">
						<xsl:for-each select="/BaoCao/DanhSachBenhNhan/BenhNhan">
							<tr>
								<td>
									<xsl:value-of select="STT"/>
								</td>
								<td>
									<xsl:value-of select="MauHoaDon"/>
								</td>
								<td>
									<xsl:value-of select="KyHieu"/>
								</td>
								<td>
									<xsl:value-of select="SoHoaDon"/>
								</td>
								<td>
									<xsl:value-of select="TenBenhNhan"/>
								</td>
								<td>
									<xsl:value-of select="SoTien"/>
								</td>
							</tr>
						</xsl:for-each>
					</tbody>
				</table>
				<div>
					<p style="text-align:right" id="countPage">
					</p>
				</div>
			</div>
			<div class="reportFooter">
				<div >
				 
					<p style="font-size:16px">
						Tổng : <xsl:value-of select="/BaoCao/TongTien"/>
					</p>
					<p style="font-size:16px">
						Bằng chữ : <xsl:value-of select="/BaoCao/SoTienBangChu"/>
					</p>
				</div>
				<table width="100%" border="0" cellspacing="0" cellpadding="0" class="report-used-bottom">
					<tr>
						<td>
							<center>
								<strong>Người lập biểu</strong>
							</center>
						</td>
						<td>
							<center>
								<strong>Người soát hóa đơn</strong>

							</center>
						</td>
						<td >
							<center>

								<strong>Thủ quỹ</strong>

							</center>
						</td>
					</tr>
				</table>
				<br/>
				<br/>
				<br/>
				<br/>
			</div>
		</div>


	</xsl:template>
</xsl:stylesheet>
