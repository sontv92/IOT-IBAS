$(function() {
  
  $('#aniimated-thumbnials').lightGallery({
    thumbnail: true,
  });
// Card's slider
  var $carousel = $('.slider-for');

  $carousel
    .slick({
      slidesToShow: 1,
      slidesToScroll: 1,
      arrows: false,
      fade: true,
      adaptiveHeight: true,
      asNavFor: '.slider-nav'
    });
  $('.slider-nav').slick({
    slidesToShow: 2,
    slidesToScroll: 1,
    asNavFor: '.slider-for',
    dots: false,
    centerMode: false,
    focusOnSelect: true,
      variableWidth: true,
     
     
  });


});