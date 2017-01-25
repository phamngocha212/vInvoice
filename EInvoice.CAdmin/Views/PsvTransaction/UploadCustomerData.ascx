<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PsvUploadModel>" %>
<%@ Import Namespace="EInvoice.PSVExtends.Models" %>
<form method="post" action="/PsvTransaction/UploadCustomerData" id="adminForm" name="adminForm" enctype="multipart/form-data">
    <div class="box-body">
        <div class="alert alert-danger text-center">Chọn file .zip của file .xml hoặc .xls Upload lên hệ thống, dung lượng file không vượt quá 100MB!</div>
        <div class="row">
            <div class="col-xs-12 form-horizontal">
                <%=Html.Hidden("TypeLabel",Model.TypeLabel) %>
                <%=Html.Hidden("TypeTrans",Model.TypeTrans) %>
                <div class="form-group">
                    <label class="col-sm-4">
                        Chọn file:<span style="color: red">(*)</span>
                    </label>
                    <div class="col-sm-8">
                        <input type="file" name="FilePath" id="FilePath" accept=".zip,.ZIP,.xls,.XLS,.xlsx,.XLSX" class="required form-control">
                        <input type="text" id="_txtFile" readonly="readonly" placeholder="Nhấn chuột vào đây để đính kèm file" class="required form-control" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="box-footer">
        <div class="element-right">
            <button class="btn btn-sm btn-primary" id="uploadButton" type="submit" onclick="SaveUpload()" style="margin-left: 10px;" name="phathanh"><i class="fa fa-upload"></i>UPLOAD DỮ LIỆU</button>
            <button class="btn btn-sm btn-default" type="button" onclick="document.location ='/Transaction/Index?TypeTran=<%=Model.TypeTrans%>'">
                <i class="fa fa-backward"></i>QUAY LẠI</button>
        </div>
    </div>
</form>
