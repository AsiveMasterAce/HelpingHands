// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const mobileScreen = window.matchMedia("(max-width: 990px )");
$(document).ready(function () {
    $(".dashboard-nav-dropdown-toggle").click(function () {
        $(this).closest(".dashboard-nav-dropdown")
            .toggleClass("show")
            .find(".dashboard-nav-dropdown")
            .removeClass("show");
        $(this).parent()
            .siblings()
            .removeClass("show");
    });
    $(".menu-toggle").click(function () {
        if (mobileScreen.matches) {
            $(".dashboard-nav").toggleClass("mobile-show");
        } else {
            $(".dashboard").toggleClass("dashboard-compact");
        }
    });

    //var url = window.location.href;

    //// iterate over all nav links
    //$('.dashboard-nav .dashboard-nav-list .dashboard-nav-item').each(function () {
    //    var link = $(this).attr('href');

    //    // check if the link is in the current URL
    //    if (url.includes(link)) {
    //        // add active class
    //        $(this).addClass('active');
    //    }
    //});
});



