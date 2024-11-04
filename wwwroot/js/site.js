// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$("#navbarDropdown").on("click", function () {
    let d = $('.dropdown-menu').css('display')
    if (d == 'none') {
        $(".dropdown-menu").css("display", "block");
    } else {
        $(".dropdown-menu").css("display", "none");
    }
});