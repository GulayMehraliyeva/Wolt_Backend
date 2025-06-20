"use strict"


document.addEventListener("DOMContentLoaded", function () {
    const pageButtons = document.querySelectorAll(".page");
    const leftArrow = document.querySelector(".left-arrow");
    const rightArrow = document.querySelector(".right-arrow");

    let currentPageIndex = 0;

    function updateActivePage(index) {
        if (index < 0 || index >= pageButtons.length) return;

        pageButtons.forEach(btn => btn.classList.remove("active"));
        pageButtons[index].classList.add("active");
        currentPageIndex = index;
    }

    pageButtons.forEach((btn, index) => {
        btn.addEventListener("click", () => {
            updateActivePage(index);
        });
    });

    document.querySelectorAll(".arrow").forEach(button => {
        button.addEventListener("click", (e) => {
            e.preventDefault();

            const isLeft = button.classList.contains("left-arrow");
            const newIndex = isLeft ? currentPageIndex - 1 : currentPageIndex + 1;
            updateActivePage(newIndex);
        });
    });

    updateActivePage(0);


   
});

