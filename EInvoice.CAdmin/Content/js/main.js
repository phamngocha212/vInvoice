(function ($) {
    $.fn.extend({
        ContentPagination: function (options) {
            var defaults = {
                height: 400
            };
            var options = $.extend(defaults, options);

            //Creating a reference to the object
            var objContent = $(this);

            // other inner variables
            var fullPages = new Array();
            var subPages = new Array();
            var height = 0;
            var activePage = 1;
            var pageCount = 1;
            // initialization function
            init = function () {
                objContent.children().each(function (i) {
                    if (height + this.clientHeight > options.height) {
                        fullPages.push(subPages);
                        subPages = new Array();
                        height = 0;
                        pageCount++;
                    }

                    height += this.clientHeight;
                    subPages.push(this);
                    $(this).addClass("p" + pageCount);
                });

                if (height > 0) {
                    fullPages.push(subPages);
                }

                // draw controls
                showPaginationBar(pageCount);

                // show first page
                showPageContent(1);
            };

            // show page content function
            showPageContent = function (page) {
                objContent.children().css("display", "none");
                $(".p" + page).css("display", "");
                $(".number").removeClass("selected");
                $("#countPage").text("tiep theo trang truoc - trang " + page + "/" + pageCount);
                activePage = page;
                //th1.chi co mot trang
                if (pageCount == 1) {
                    $("#countPage").text("");
                    $("#countPage").css("display", "none");
                    $(".nextpage").css("display", "line");
                }
                //th2.truong hop nhieu trang
                if (pageCount > 1) {

                    //trang thu 1
                    if (page == 1) {
                        var t = fullPages;
                        var tong = 0;
                        $("#countPage").text("");
                        $("#countPage").text("trang " + page + "/" + pageCount);
                        $(".nextpage").css("display", "none");
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
                pagins += "<a href='#' id='prev' class='prevnext'><</a>";
                var limit = 10;
                var start = 1;
                var end = numPages;
                if (numPages > limit) {
                    var middle = Math.ceil(limit / 4);
                    var below = (activePage - middle);
                    var above = (activePage + middle);

                    if (below < 4) {
                        above = limit;
                        below = 1;
                    }
                    else if (above > (numPages - 4)) {
                        above = numPages;
                        below = (numPages - limit);
                    }

                    start = below;
                    end = above;
                }
                if (start > 3)
                    pagins += "<span>...</span>";
                for (var i = start; i <= end; i++) {
                    if (i == activePage || (activePage <= 0 && i == 0)) {
                        pagins += '<span href="#" class="current number" id="ap' + i + '" onclick="showPageContent(' + i + '); return false;">' + i + '</span>';
                    }
                    else
                        pagins += '<a href="#" class="number" id="ap' + i + '" onclick="showPageContent(' + i + '); return false;">' + i + '</a>';
                }
                if (end < (pageCount - 3))
                    pagins += "<span>...</span>";
                pagins += "<a href='#' id='next' class='prevnext'>></a>";
                pagins += '<a href="#" class="number" id="ap' + numPages + '" onclick="showPageContent(' + numPages + '); return false;">>></a>'
                $('.pagination').append(pagins);
            };

            // perform initialization
            init();

            // and binding 2 events - on clicking to Prev
            $('.pagination #prev').click(function () {
                if (activePage > 1)
                    showPageContent(activePage - 1);
            });
            // and Next
            $('.pagination #next').click(function () {
                if (activePage < pageCount)
                    showPageContent(activePage + 1);
            });

            // print function
            print4Convert = function () {
                var printContent = "";
                $(".pagination").css("display", "none");
                for (var i = 1; i <= pageCount; i++) {
                    showPageContent(i);
                    tempContent = $("#container").html();
                    printContent += tempContent + "<p style='page-break-before: always'/>";
                }
                $("#ds").html(printContent);
                var printElement = document.getElementById("ds");
                $(printElement).printArea({
                    mode: "iframe",
                    popWd: 900,
                    popHt: 600,
                    popClose: false
                });
            };
        },
        ProductNumberPagination: function (options) {
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
                $(".pagination").css("display", "none");
                for (var i = 1; i <= pageCount; i++) {
                    showPageContent(i);
                    tempContent = $("#container").html();
                    printContent += tempContent + "<p style='page-break-before: always'/>";
                }
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
                $("#countPage").text("tiep theo trang truoc - trang " + page + "/" + pageCount);
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
                        $("#countPage").text("trang " + page + "/" + pageCount);
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
            previewPage = function(){
                if (currentPage > 1) {
                    showPageContent(currentPage - 1);
                }
            }
            // and Next
            nextPage= function () {
                if (currentPage < pageCount) {
                    showPageContent(currentPage + 1);
                }
            }
        }
    });
}(jQuery));
