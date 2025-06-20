"use strict"

const dropdownToggle = document.getElementById("dropdownToggle");
const dropdownMenu = document.getElementById("dropdownMenu");

dropdownToggle.addEventListener("click", function (e) {
    e.stopPropagation();
    dropdownMenu.classList.toggle("show");
});

document.addEventListener("click", function () {
    dropdownMenu.classList.remove("show");
});