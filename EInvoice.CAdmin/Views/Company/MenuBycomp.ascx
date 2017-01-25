<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<MenuModels>>" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<section class="sidebar">

    <ul class="sidebar-menu">
        <!-- Sidebar user panel -->
        <%foreach (MenuModels it in Model)
          {          
        %>
        <li class='treeview treeviewlv1' data-submenu-id="submenu-<%=it.MenuBase.Id %>">
            <%if (EInvoice.CAdmin.DataHelper.IsAcitve(it.MenuBase))
              {%>
            <a href="<%=it.MenuBase.NavigateUrl%>"><i class="fa <%=it.MenuBase.Css %> "></i><span><%=Html.Encode(it.MenuBase.Name.ToUpper())%></span><i class="fa fa-angle-left pull-right"></i></a>
            <ul class="treeview-menu treeview-menu1" id="submenu-<%=it.MenuBase.Id %>">
                <%foreach (var c in it.Items)
                  {
                      int i = c.Items.Count();                              
                %>
                <%if (EInvoice.CAdmin.DataHelper.IsAcitve(c.MenuBase))
                  {%>
                <li class="treeview">
                    <a href="<%=c.MenuBase.NavigateUrl%>"><i class="fa fa-circle-o"></i><span><%=Html.Encode(c.MenuBase.Name)%></span>
                        <%=i>0 ? "<i class='fa fa-angle-left pull-right'></i>" : ""%></a>
                    <%if (i > 0)
                      {%>
                    <ul class="treeview-menu treeview-menu2">
                        <%foreach (var cc in c.Items)
                          {%>
                        <%if (EInvoice.CAdmin.DataHelper.IsAcitve(cc.MenuBase))
                          {%>
                        <li><a href="<%=cc.MenuBase.NavigateUrl%>"><i class="fa fa-circle"></i><span><%=Html.Encode(cc.MenuBase.Name)%></span></a></li>
                        <%}
                          } %>
                    </ul>
                    <%} %>
                </li>
                <%}
                  } %>
            </ul>
            <%} %>
        </li>
        <%} %>
    </ul>

    <script>
        var menu = $(".sidebar-menu");

        // jQuery-menu-aim: <meaningful part of the example>
        // Hook up events to be fired on menu row activation.
        menu.menuAim({
            activate: activateSubmenu,
            deactivate: deactivateSubmenu
        });
        // jQuery-menu-aim: </meaningful part of the example>

        // jQuery-menu-aim: the following JS is used to show and hide the submenu
        // contents. Again, this can be done in any number of ways. jQuery-menu-aim
        // doesn't care how you do this, it just fires the activate and deactivate
        // events at the right times so you know when to show and hide your submenus.

        function activateSubmenu(row) {
            var row = $(row),
                submenuId = row.data("submenuId"),
                submenu = $("#" + submenuId),
                height = menu.outerHeight(),
                width = menu.outerWidth();
            // Show the submenu              
            submenu.addClass("activeSubMenu");
        }

        function deactivateSubmenu(row) {
            var row = $(row),
                submenuId = row.data("submenu-id");
            submenu = $("#" + submenuId);

            submenu.removeClass("activeSubMenu");
        }

        // Bootstrap's dropdown menus immediately close on document click.
        // Don't let this event close the menu if a submenu is being clicked.
        // This event propagation control doesn't belong in the menu-aim plugin
        // itself because the plugin is agnostic to bootstrap.

        $(document).ready(function () {
            $(".content-wrapper").hover(function () {
                if ($(".treeview-menu1").hasClass("activeSubMenu")) {
                    $(".treeview-menu1").addClass("hideSub");
                }
            });
            $(".treeview").hover(function () {
                if ($(this).find(".treeview-menu1").hasClass("hideSub")) {
                    $(this).find(".treeview-menu1").removeClass("hideSub");
                }
            });
            $(".treeview-menu1").hover(
            function () {
                $(this).removeClass("hideSub");
                $(this).addClass("activeSubMenu");
                // Called when the mouse enters the element
            },
            function () {
                $(this).removeClass("activeSubMenu");
                $(this).addClass("hideSub");
                // Called when the mouse leaves the element
            }
           );
        });
    </script>
</section>
