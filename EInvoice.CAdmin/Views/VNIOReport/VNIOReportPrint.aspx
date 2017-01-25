<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="errorTitle" ContentPlaceHolderID="TitleContent" runat="server">
     Lập báo cáo theo ngày
</asp:Content>

<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">
        <script src="/Content/js/jquery.PrintArea.js"></script>    
     <script >(function ($) {
    $.fn.extend({
        ReportNumberPagination: function (options) {
            var defaults = {
                number: 10
            };
            var options = $.extend(defaults, options);

            //Creating a reference to the object
            var objContent = $(this);

            // other inner variables
            var fullPages = new Array();
            var subPages = new Array();
            var k = 0;
            var currentPage = 0;
            var pageCount = 1;
            // initialization function
            init = function () {
                objContent.children().slice(0, objContent.children().length).each(function (i) {
                    k++;
                    if (k > options.number) {
                        fullPages.push(subPages);
                        subPages = new Array();
                        subPages.push(this);
                        k = 1;
                        pageCount++;
                        $(this).addClass("p" + pageCount);
                    }

                    //k++;
                    subPages.push(this);
                    $(this).addClass("p" + pageCount);
                });

                if (k > 0) {
                    fullPages.push(subPages);
                }
                if (pageCount > 1) {
                    currentPage = 0;
                    // draw controls
                    showPaginationBar(pageCount);

                    // show first page
                    showPageContent(1);
                }
            };

            // print function
            print4Convert = function () {
                var printContent = "";
               // $(".pagination").css("display", "none");
                tempContent = $(".reportHeader").html();
                for (var i = 1; i <= pageCount; i++) {
                    showPageContent(i);
                    tempContent += $(".reportBody").html();
                    //Sang trang mới 
                    if(i != pageCount)
                    tempContent += "<p style='page-break-before: always'/>"
                }
                printContent += tempContent + $(".reportFooter").html();
                $("#ds").html(printContent);
                var printElement = document.getElementById("ds");
                $(printElement).printArea({
                    mode: "iframe",
                    popWd: 900,
                    popHt: 600,
                    popClose: false
                });
            };

            // show page content function
            showPageContent = function (page) {
                currentPage = 0;
                objContent.children().css("display", "none");
                $(".p" + page).css("display", "");
                $(".number").removeClass("selected");
                $("#countPage").text("Trang " + page + "/" + pageCount);
                currentPage = parseInt(page);
                //th1.chi co mot trang
                if (pageCount == 1) {
                    $("#countPage").text("");
                    $("#countPage").css("display", "none");
                    $(".nextpage").css("display", "");
                }
                //th2.truong hop nhieu trang
                if (pageCount > 1) {

                    //trang thu 1
                    if (page == 1) {
                        var t = fullPages;
                        var tong = 0;
                        $("#countPage").text("");
                        $("#countPage").text("Trang " + page + "/" + pageCount);
                        $(".nextpage").css("display", "none");
                        $(".bgimg").css("display", "none");
                    }
                        //trang tiep theo
                    else if (page > 1 && page < pageCount) {
                        $(".nextpage").css("display", "none");
                        $(".bgimg").css("display", "none");
                    }
                        //trang cuoi cung
                    else if (page == pageCount) {
                        $('.bgimg').css("display", "");
                        $(".nextpage").css("display", "");
                    }
                }
                showPaginationBar(pageCount);
            };
            // show pagination bar function (draw switching numbers)
            showPaginationBar = function (numPages) {
                $('.pagination').empty();
                var pagins = '';
                pagins += '<a href="#" class="number" id="ap0" onclick="showPageContent(1); return false;"><<</a>';
                pagins += "<a href='#' id='prev' class='prevnext' onclick='previewPage()'><</a>";
                pagins += '<a href="#" class="number" id="ap1" onclick="showPageContent(1); return false;">1</a>';
                var limit = 10;
                var start = 2;
                var end = numPages - 1;
                if (numPages > limit) {
                    var middle = Math.ceil(limit / 4);
                    var below = (currentPage - middle);
                    var above = (currentPage + middle);

                    if (below < 4) {
                        above = limit;
                        below = 2;
                    }
                    else if (above > (numPages - 5)) {
                        above = numPages - 1;
                        below = (numPages - limit);
                    }

                    start = below;
                    end = above;
                }
                if (start > 3)
                    pagins += "<span>...</span>";
                for (var i = start; i <= end; i++) {
                    if (i == currentPage || (currentPage <= 0 && i == 0)) {
                        pagins += '<span href="#" class="current number" id="ap' + i + '" onclick="showPageContent(' + i + '); return false;">' + i + '</span>';
                    }
                    else
                        pagins += '<a href="#" class="number" id="ap' + i + '" onclick="showPageContent(' + i + '); return false;">' + i + '</a>';
                }
                if (end < (numPages - 4))
                    pagins += "<span>...</span>";
                pagins += '<a href="#" class="number" id="ap' + numPages + '" onclick="showPageContent(' + numPages + '); return false;">' + numPages + '</a>'
                pagins += '<a href="#" id="next" class="prevnext" onclick="nextPage()">></a>';
                pagins += '<a href="#" class="number" id="ap' + parseInt(numPages + 1) + '" onclick="showPageContent(' + numPages + '); return false;">>></a>';
                $('.pagination').append(pagins);
            };

            // perform initialization
            init();

            // and binding 2 events - on clicking to Prev
            previewPage = function () {
                if (currentPage > 1) {
                    showPageContent(currentPage - 1);
                }
            }
            // and Next
            nextPage = function () {
                if (currentPage < pageCount) {
                    showPageContent(currentPage + 1);
                }
            }
        }
    });
}(jQuery));
</script>   
        <% string html = ViewData["html"].ToString(); %>
    <div id="container">
         <div id="print" style="width:1000px; clear:left;margin:0 auto;"><%: Html.Raw(html.Replace("&lt;br/&gt;","<br />"))  %>
             <div id="ds" style="display:none"></div>
         </div>
   </div>
      <div class="text-center">
        <div class='pagination'></div><br/>
        <button class="btn btn-sm btn-primary" type="button" onclick="print4Convert()"><i class="fa fa-print"></i> In báo cáo</button>                              
    </div> 
    <script type="text/javascript">
        $(document).ready(function () {
            $('.reportsTable .reportsData').ReportNumberPagination({ number: 25 });
        });
        
        function Download_Excel() {
            var Month = $("#Month").val();
            var Year = $("#Year").val();
            document.location = "/ReportsInv/DownloadReportMonth_Excel?month=" + Month + "&year=" + Year;
        }
        function Download_xml() {
            var Month = $("#Month").val();
            var Year = $("#Year").val();
            document.location = "/ReportsInv/DownloadReportMonth_xml?month=" + Month + "&year=" + Year;
        }
    </script>
</asp:Content>
