"use strict"

let shownCount = 3;
const step = 3;
document.getElementById("show-more-btn")?.addEventListener("click", function () {
    const hiddenComments = document.querySelectorAll(".hidden-comment");
    for (let i = 0; i < step && i < hiddenComments.length; i++) {
        hiddenComments[i].classList.remove("hidden-comment");
    }
    if (document.querySelectorAll(".hidden-comment").length === 0) {
        this.style.display = "none";
    }
});

const filters = document.querySelectorAll(".category-filter");
const categoryBlocks = document.querySelectorAll(".category-block");
const specialBlock = document.querySelector(".special-offers-block");

filters.forEach(filter => {
    filter.addEventListener("click", function (e) {
        e.preventDefault();
        const selectedId = this.dataset.categoryId;

        filters.forEach(f => f.classList.remove("active"));
        this.classList.add("active");

        if (selectedId === "all") {
            specialBlock.style.display = "block";
            categoryBlocks.forEach(block => block.style.display = "block");
        } else if (selectedId === "special") {
            specialBlock.style.display = "block";
            categoryBlocks.forEach(block => block.style.display = "none");
        } else {
            specialBlock.style.display = "none";
            categoryBlocks.forEach(block => {
                block.style.display = block.dataset.categoryId === selectedId ? "block" : "none";
            });
        }
    });
});

document.querySelector('[data-category-id="all"]').click();