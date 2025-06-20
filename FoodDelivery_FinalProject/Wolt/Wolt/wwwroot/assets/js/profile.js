"use strict"

document.addEventListener("DOMContentLoaded", function () {
    const links = document.querySelectorAll(".sidebar ul li");
    const sections = document.querySelectorAll(".content-section");

    links.forEach((link, index) => {
        link.addEventListener("click", function () {
            links.forEach(l => l.classList.remove("active"));
            this.classList.add("active");

            sections.forEach(s => s.style.display = "none");
            sections[index].style.display = "block";
        });
    });

    @* Pass server - side messages as JS variables * @
            var successMessage = '@(TempData["Success"] ?? "")';
    var errorMessage = '@(TempData["Error"] ?? "")';

    if (successMessage) {
        alert(successMessage);
    }
    else if (errorMessage) {
        alert(errorMessage);
    }
});


