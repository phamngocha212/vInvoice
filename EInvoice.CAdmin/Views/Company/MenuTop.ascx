<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<EInvoice.CAdmin.Models.MenuModels>>" %>
<!-- Header Navbar: style can be found in header.less -->
<nav class="navbar navbar-static-top" role="navigation">
    <!-- Sidebar toggle button-->

    <div class="navbar-header">
        <a href="/" class="logo">
            <!-- mini logo for sidebar mini 50x50 pixels -->
            <img src="/Content/images/logo.png" alt="Alternate Text" />
        </a><a href="#" class="sidebar-toggle" data-toggle="offcanvas" role="button">
            <span class="sr-only">Toggle navigation</span>
        </a>
        <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#navbar-collapse">
            <i class="fa fa-bars"></i>
        </button>
    </div>

    <div class="collapse navbar-collapse pull-left" id="navbar-collapse">
        <ul class="nav navbar-nav">
            <%foreach (var it in Model)
              {
                  int i = it.Items.Count();                  
            %>
            <li class="dropdown menulv-1"><a href="<%=it.MenuBase.NavigateUrl%>"><%=Html.Encode(it.MenuBase.Name.ToUpper())%>
                <%=i > 0 ? "<span class='caret'></span>" : ""%></a>
                <%if (i > 0)
                  {%>
                <ul class="dropdown-menu menulv-2" role="menu">
                    <%foreach (var c in it.Items)
                      {                                                
                    %>
                    <%if (EInvoice.CAdmin.DataHelper.IsAcitve(c.MenuBase))
                      {%>
                    <li><a href="<%=c.MenuBase.NavigateUrl%>"><i class="fa fa-asterisk"></i><%=Html.Encode(c.MenuBase.Name)%></a></li>
                    <%}
                      }%>
                </ul>
                <%}%>              
            </li>
            <%
              }%>
        </ul>

    </div>
    <!-- /.navbar-collapse -->
    <div class="navbar-custom-menu">
        <ul class="nav navbar-nav">
            <li class="dropdown tool tool-menu">
                <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                    <i class="fa fa-suitcase"></i>Ứng dụng ký số
                </a>
                <ul class="dropdown-menu">

                    <!-- Menu Footer-->
                    <li class="user-footer">
                        <a href="/Data/setup.exe"><i class="fa fa-download"></i>Ứng dụng ký số</a>
                    </li>
                    <li>
                        <a href="https://chrome.google.com/webstore/detail/invoice-signing/dmoaiianpoajgjaigblfebiblfjdbgeh" target="_blank"><i class="fa fa-asterisk"></i>Phần mềm ký trên chrome</a>
                    </li>
                </ul>
            </li>
            <!-- User Account: style can be found in dropdown.less -->
            <li class="dropdown user user-menu">
                <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                    <i class="fa fa-user"></i>
                    <span class=""><%=HttpContext.Current.User.Identity.Name%>
                    </span>
                </a>
                <ul class="dropdown-menu">

                    <!-- Menu Footer-->
                    <li class="user-footer">
                        <div class="pull-left">
                            <a href="/Account/Changepassword" class="btn btn-default btn-flat">Đổi mật khẩu</a>
                        </div>
                        <div class="pull-right">
                            <a href="/Account/Logout" class="btn btn-default btn-flat">Đăng xuất</a>
                        </div>
                    </li>
                </ul>
            </li>
            <!-- Control Sidebar Toggle Button -->
        </ul>
    </div>
</nav>
