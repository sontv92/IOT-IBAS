$(document).on('ready', function() {
    $(".center").slick({
        autoplay: true,
    autoplaySpeed: 2000,
    infinite: true,
    centerMode: false,
    slidesToShow: 5,
    slidesToScroll: 3,
    responsive: [
        {
          breakpoint: 1024,
          settings: {
            slidesToShow: 3,
            slidesToScroll: 1,
            infinite: true,
          }
        },
        {
          breakpoint: 600,
          settings: {
            slidesToShow: 2,
            slidesToScroll: 1
          }
        },
        {
          breakpoint: 480,
          settings: {
            slidesToShow: 1,
            slidesToScroll: 1
          }
        }
        // You can unslick at a given breakpoint now by adding:
        // settings: "unslick"
        // instead of a settings object
      ]
  });
  $(".regular").slick({
        dots: false,
        infinite: true,
        slidesToShow: 5,
        // autoplay: true,
        // autoplaySpeed: 2000,
        slidesToScroll: 1,
        prevArrow: '<div class="slick-prev-spdx"></div>',
        nextArrow: '<div class="slick-next-spdx"></div>', 
        responsive: [
        {
          breakpoint: 1024,
          settings: {
            slidesToShow: 3,
            slidesToScroll: 1,
            infinite: true,
          }
        },
        {
          breakpoint: 600,
          settings: {
            slidesToShow: 2,
            slidesToScroll: 1
          }
        },
        {
          breakpoint: 480,
          settings: {
            slidesToShow: 1,
            slidesToScroll: 1
          }
        }
        // You can unslick at a given breakpoint now by adding:
        // settings: "unslick"
        // instead of a settings object
      ]
      });
  // Main/Product image slider for product page
  $('#detail .main-img-slider').slick({
    slidesToShow: 1,
    slidesToScroll: 1,
    infinite: true,
    arrows: true,
    fade:true,
    autoplay: true,
    autoplaySpeed: 4000,
    speed: 300,
    lazyLoad: 'ondemand',
    asNavFor: '.thumb-nav',
    prevArrow: false,
    nextArrow: false
  });
  // Thumbnail/alternates slider for product page
  $('.thumb-nav').slick({
    slidesToShow: 4,
    slidesToScroll: 1,
    infinite: true,
    centerPadding: '0px',
    asNavFor: '.main-img-slider',
    dots: false,
    centerMode: false,
    draggable: true,
    speed:200,
    focusOnSelect: true,
    prevArrow: '<div class="slick-prev"></div>',
    nextArrow: '<div class="slick-next"></div>'  
  });


  //keeps thumbnails active when changing main image, via mouse/touch drag/swipe
  $('.main-img-slider').on('afterChange', function(event, slick, currentSlide, nextSlide){
    //remove all active class
    $('.thumb-nav .slick-slide').removeClass('slick-current');
    //set active class for current slide
    $('.thumb-nav .slick-slide:not(.slick-cloned)').eq(currentSlide).addClass('slick-current');  
  });
});