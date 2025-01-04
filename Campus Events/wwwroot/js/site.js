// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    setTimeout(function () {
        var messageElement = document.getElementById("tempMessage");
        if (messageElement) {
            messageElement.style.display = "none";
        }
    }, 5000); // Masque le message après 5 secondes
});
