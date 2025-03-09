jQuery(document).ready(function($){
    $(document).ready(function () {

        //************** JS FAQ *****************//
        $('.question .title').click(function (event) {
            $(this).next('.answer').slideToggle('500');
            $(this).toggleClass('display_icon');
        });
		//************** END FAQ *****************//


		if ($(window).width() < 480) {
		   $('#masthead').addClass('mobile_head');
		}
        $('.pageCust-88 .ot-vm-click').addClass('active');
		$('#mega-menu-title').click(function() {
			$('.ot-vm-click').toggleClass('active');
		});
		//$('.box_show_product .gl_box_home_product').click(function() {
		//	$(this).next('.tol_box_home_product').children('.slider_fix_1.list_cat').toggle(500);
  //      });
        if ($(window).width() < 480) {
				$('.box_show_product .gl_box_home_product').click(function() {
				$(this).next('.tol_box_home_product').children('.slider_fix_1.list_cat').slideToggle(300);
			});
		}
		$('.icon_menu').click(function(event) {
			$('.mobile-sidebar.mfp-wrap').toggleClass('show_menu_mobile');
		});
		$('.icon_menu').click(function(event){
			$('.main-menu-overlay').toggleClass('active_over');
		});
		$('.close_menu').click(function(event) {
			$('.mobile-sidebar.mfp-wrap').removeClass('show_menu_mobile');
		});
		$('.close_menu').click(function(event) {
			$('.main-menu-overlay').removeClass('active_over');
        });


        //$('.btn_read_more_exc').click(function () {
        //    $('.product-short-description').toggleClass('active_read');
        //    if ($('.btn_read_more_exc').text() == "Xem thêm") {
        //        $(this).text("Thu gọn");
        //    } else {
        //        $(this).text("Xem thêm");
        //    }
        //});
        //var chieucao = $('.product-short-description').outerHeight();
        //if (chieucao <= 45) {
        //    $('.btn_read_more_exc').remove();
        //}
        //$('.btn_read_more_exc').click(function () {
        //    $(this).toggleClass('less_more');
        //});

        //$('.read_more_sale_sum').click(function () {
        //    $('.infomation_sale').toggleClass('active_read_sale');
        //    if ($('.read_more_sale_sum').text() == "Xem tất cả") {
        //        $(this).text("Thu gọn");
        //    } else {
        //        $(this).text("Xem tất cả");
        //    }
        //});
        //var chieucaosale = $('.infomation_sale').outerHeight();
        //if (chieucaosale <= 140) {
        //    $('.read_more_sale_sum').remove();
        //}
        //$('.read_more_sale_sum').click(function () {
        //    $(this).toggleClass('less_more_sale');
        //});

        //$('.btn_read_more_content_sing_product span.asp').click(function () {
        //    $('.editorial_hotspots').toggleClass('active_read_editorial');
        //    if ($('.btn_read_more_content_sing_product span.asp').text() == "Xem thêm nội dung") {
        //        $(this).text("Thu gọn");
        //    } else {
        //        $(this).text("Xem thêm nội dung");
        //    }
        //});
        //var chieucaosale = $('.editorial_hotspots').outerHeight();
        //if (chieucaosale <= 600) {
        //    $('.btn_read_more_content_sing_product').remove();
        //}
        //$('.btn_read_more_content_sing_product').click(function () {
        //    $(this).toggleClass('less_more_editorial');
        //});
 


        //load more
        //if ($('.product-short-description').length > 0) {
        //    var wrap = $('.product-short-description');
        //    var current_height = wrap.height();
        //    alert(current_height);
        //    var your_height = 45;
        //    if (current_height > your_height) {
        //        wrap.css('height', your_height + 'px');
        //        wrap.append(function () {
        //            return '<div class="nhantranvn_readmore nhantranvn_readmore_show"><a title="Đọc thêm" href="javascript:void(0);">Đọc thêm</a></div>';
        //        });
        //        wrap.append(function () {
        //            return '<div class="nhantranvn nhantranvn_readmore_less" style="display: none"><a title="Thu gọn" href="javascript:void(0);">Thu gọn</a></div>';
        //        });
        //        $('body').on('click', '.nhantranvn_readmore_show', function () {
        //            wrap.removeAttr('style');
        //            $('body .nhantranvn_readmore_show').hide();
        //            $('body .nhantranvn_readmore_less').show();
        //        });
        //        $('body').on('click', '.nhantranvn_readmore_less', function () {
        //            wrap.css('height', your_height + 'px');
        //            $('body .nhantranvn_readmore_show').show();
        //            $('body .nhantranvn_readmore_less').hide();
        //        });
        //    }
        //}
        //end more

		


        //var chieucaosale = $(".editorial_hotspots").outerHeight();
        
	    //if (chieucaosale <= 600){
	    //	$('.btn_read_more_content_sing_product').remove();
	    //}
	    //$('.btn_read_more_content_sing_product').click(function() {
	    //	$(this).toggleClass('less_more_editorial');
	    //});


	    if ($(window).width() < 900) {
		   	$(".ulfilter_side .lilevel1 > span").click(function(){
		   		$(this).next("ul").toggleClass('showulfilter');
		   	});
		}
		$('.item_checkout:first-child').addClass('active_method');
		$('.item_checkout').click(function(event) {
			$(this).addClass('active_method');
			$(".item_checkout").not(this).removeClass('active_method');
		});
		$('.btn_add_diachi span').click(function(event) {
			$('.show_dckhac').slideToggle("slow");
		});
	});
});

function openCity(evt, cityName) {
  	var i, tabcontent, tablinks;
  	tabcontent = document.getElementsByClassName("tabcontent");
  	for (i = 0; i < tabcontent.length; i++) {
    	tabcontent[i].style.display = "none";
  	}
  	tablinks = document.getElementsByClassName("tablinks");
  	for (i = 0; i < tablinks.length; i++) {
    	tablinks[i].className = tablinks[i].className.replace(" active", "");
  	}
  	document.getElementById(cityName).style.display = "block";
  	evt.currentTarget.className += " active";
}

