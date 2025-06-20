"use strict"


const sliderRow = document.getElementById("slider-row");
const leftArrow = document.querySelector(".left-arrow");
const rightArrow = document.querySelector(".right-arrow");

let currentIndex = 0;
const totalItems = sliderRow.querySelectorAll(".image-item").length;
const visibleItems = 2;

function updateSlider() {
    const itemWidth = sliderRow.querySelector(".image-item").offsetWidth;
    sliderRow.style.transform = `translateX(-${currentIndex * itemWidth}px)`;
}

rightArrow.addEventListener("click", () => {
    if (currentIndex < totalItems - visibleItems) {
        currentIndex++;
        updateSlider();
    }
});

leftArrow.addEventListener("click", () => {
    if (currentIndex > 0) {
        currentIndex--;
        updateSlider();
    }
});





const brandsRow = document.getElementById("brands-row");
const leftArrowBrand = document.querySelector("#brands .left-arrow");
const rightArrowBrand = document.querySelector("#brands .right-arrow");

let brandIndex = 0;
const brandItems = brandsRow.querySelectorAll(".image-item");
const totalBrandItems = brandItems.length;

function updateBrandSlider() {
    const itemWidth = brandItems[0].offsetWidth;
    brandsRow.style.transform = `translateX(-${brandIndex * itemWidth}px)`;
}

rightArrowBrand.addEventListener("click", () => {
    if (brandIndex < totalBrandItems - 6) {
        brandIndex++;
        updateBrandSlider();
    }
});

leftArrowBrand.addEventListener("click", () => {
    if (brandIndex > 0) {
        brandIndex--;
        updateBrandSlider();
    }
});


const navItems = document.querySelectorAll('.second-navbar .nav-item');

navItems.forEach(item => {
    item.addEventListener('click', () => {
        navItems.forEach(i => i.classList.remove('active'));
        item.classList.add('active');
    });
});
