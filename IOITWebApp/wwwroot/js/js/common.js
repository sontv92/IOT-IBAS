
//Js gửi đăng ký nhận bản tin
function ReceiveNews() {
    var email = $("#EmailFooter");
    var msg = "";
    if (email.val() == undefined || email.val() == "") {
        msg = "Vui lòng nhập Email!";
    }
    else {
        var regex = /^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;
        if (!regex.test(String(email.val()).toLowerCase())) {
            msg = "Email không hợp lệ!"
        }
    }
    
    if (msg == "") {
        $("#error-receive-news").text("");
        console.log(JSON.stringify({ Email: email.val() }));
        $('#formSubmitted').empty();

        var formData = new FormData();
        formData.append("Email", email.val());
        $.ajax({
            type: "POST",
            url: "/Home/ReceiveNews",
            cache: false,
            contentType: false,
            processData: false,
            data: formData,
            success: function (result) {
            },
            error: function (req, status, error) {
            },
            statusCode: {
                200: function () {
                    email.val("");
                    var msgSuccess = '<div class="alert alert-success alert-dismissible bg-thanhcong" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong></strong> Đăng ký nhận bản tin thành công!</div>';
                    $('#formSubmitted').append(msgSuccess);
                },
                212: function () {
                    var msgSuccess = '<div class="alert alert-warning alert-dismissible bg-thongbao" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong></strong> Email đã tồn tại trong hệ thống!</div>';
                    $('#formSubmitted').append(msgSuccess);
                },
                500: function () {
                    var msgSuccess = '<div class="alert alert-danger alert-dismissible bg-thatbai" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong></strong> Xảy ra lỗi. Vui lòng thử lại sau!</div>';
                    $('#formSubmitted').append(msgSuccess);
                }
            }
        });

        setTimeout(
            function () {
                $('#formSubmitted').empty();
            }, 5000);
    }
    else {
        $("#error-receive-news").text(msg);
    }
    

}