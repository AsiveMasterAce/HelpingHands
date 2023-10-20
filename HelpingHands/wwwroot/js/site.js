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


    var sectionLinks = $('a.dashboard-nav-item, a.dashboard-nav-dropdown-item');

    sectionLinks.each(function () {
        if ($(this).prop('href') == window.location.href) {
            $(this).addClass('active');
        }
    });

    window.onscroll = function () {
        var activeSection = document.elementFromPoint(window.innerWidth / 2, window.innerHeight / 2);
        sectionLinks.each(function () {
            if ($(this).prop('href') == '#' + activeSection.id) {
                $(this).addClass('active');
            } else {
                $(this).removeClass('active');
            }
        });
    };
});



