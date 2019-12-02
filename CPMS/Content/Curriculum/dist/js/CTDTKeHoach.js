document.getElementById('themctdt').addEventListener("click", function (event) {
    $('#the').html('');
    $('#noidung').html('');
    $('#monhoc').html('');
    $('#lop').html('');
});
//loadList();
//function loadList() {
//    $.ajax({
//        async: true,
//        type: 'POST',
//        url: '/CTDTKeHoach/loadList',
//        dataType: "json",
//        contentType: "application/json",
//        cache: false,
//        error: function (json) {
//            alert(json.Message);
//        },
//        success: function (json) {
//            $("#zero_config tbody").append('<tr></tr>');
//            $.each(json.data, function (i, item) {
//                i++;
//                $('#zero_config tr:last').after('<tr><td style="text-align:center">' + i + '</td><td style="text-align:center">' + item.Trinhdo + '</td><td style="text-align:center">' + item.Ghichu + '</td><td style="text-align:center">' + item.Phienban + '</td><td style="text-align:center">' + item.Tinhtrang +'<td style="text-align:center"><a href="javascript:void(0)"><img src="/images/iconfinder_edit_216184.png" style="width:40px;"></a></td></tr>');
//            });
//        }
//    });
//}

var jsonapi;
document.getElementById('btnApply').addEventListener("click", function (event) {
    $('#the').html('');
    $('#noidung').html('<img style="position:inherit; float:left; padding-left:550px" src="/Content/Curriculum/5.gif" />');
    $('#monhoc').html('');
    $('#lop').html('');
    event.preventDefault();
    //get data từ 2 combo box
    var he = $('#he').val();
    var nganh = $('#nganh').val();
    //thêm vào class để gửi json qua controller
    var data = {
        manganh: nganh,
        trinhdo: he,
    };
    //gọi ajax để gửi json qua controller
    $.ajax({
        async: true,
        data: JSON.stringify(data),
        type: 'POST',
        url: '/CTDTKeHoach/Find',
        dataType: "json",
        contentType: "application/json",
        cache: false,
        error: function (json) {
            alert(json.Message);
        },
        success: function (json) {
            console.log(json)
            if (json.Message == undefined) {
                var tdate = null;
                if (json.ctdtao.ngayqd != null) {
                    //Parse dữ liệu sang ngày tháng
                    var date = new Date(parseInt(json.ctdtao.ngayqd.replace('/Date(', '')));
                    tdate = (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
                }     
                //Thêm dữ liệu vào các thẻ div
                var tab = '<ul class="nav nav-pills" role = "tablist" style = "margin-left: 10px;width: 100%;"><li class="nav-item"><a class="nav-link active" data-toggle="tab" href="#noidung">Nội dung CTĐT</a></li><li class="nav-item"><a class="nav-link" data-toggle="tab" href="#monhoc">Môn học</a></li></ul>';
                var str = '<h5 style="position:inherit;float:left ;color: black"><span class= "pull-left"> BỘ GIÁO DỤC VÀ ĐÀO TẠO</span ><br /><b>TRƯỜNG ĐẠI HỌC VĂN LANG</b></h5 ><h5 style="position:absolute; right:0 ; color: black"><b class="pull-right">CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</b><br /><b>Độc Lập - Tự Do - Hạnh Phúc</b></h5><div class="col-md-12"><div class="pull-left" style="color: black; text-align:justify"><div class="pull-left" style="color: black; text-align:center"><b style="font-size: 25px; padding-right:250px">Chương Trình Giáo Dục ' + json.ctdtao.trinhdokhung + '</b></div><hr /><p style="text-align:left ; font-size: 20px"><b>Tên Chương Trình : </b> ' + json.ctdtao.tenct + ' <br /><b>Trình Độ Đào Tạo : </b>' + json.ctdtao.trinhdokhung + '<br /><b>Ngành Đào Tạo : </b>' + json.ctdtao.tenkhoa + ' <br /><b> Mã Số : </b>' + json.ctdtao.madk + ' <br /><b>Loại Hình Đào Tạo : </b>' + json.ctdtao.loaihinhdt + '<br />(Ban hành tại quyết định số ' + json.ctdtao.soqd + '/ VL-ĐT ngày ' + tdate + ' của Hiệu Trưởng trường Đại Học Văn Lang)</p><br /><ol style="font-size: 20px"><li><b>Mục Tiêu Đào Tạo : </b> <br /> ' + json.ctdtao.muctieu + ' </li><li><b>Thời gian đào tạo : </b> ' + json.ctdtao.tgiandt + ' <br /></li><li><b>Khối lượng kiến thức toàn khoá : </b> ' + json.ctdtao.khoiluongkt + ' <br /> </li><li><b>Đối tượng tuyển sinh : </b> ' + json.ctdtao.doituong + ' <br /></li><li><b>Quy trình đào tạo, điều kiện tốt nghiệp : <br /></b> ' + json.ctdtao.quytrinh + '</li><li> <b>Thang điểm : </b>' + json.ctdtao.thangdiem + ' <br /></li><li><b>Cơ sở vật chất phục vụ học tập : </b> <br /> ' + json.ctdtao.csvc + '</li></ol></div></div>';
                var strmh = '<section> <h2 style="text-align:center">Nội Dung Chương Trình Đào Tạo</h2> <div class="card"><div class="card-body"><div class="table-responsive"> <table class="table table-striped table-bordered"><thead> <tr style="text-align:center ; background-color:firebrick; color:white"><th>STT</th> <th>Loại Kiến Thức</th><th>Tên học phần (tên Tiếng Việt, tên Tiếng Anh)</th><th>TC</th><th>LT</th><th>TH</th><th>ĐA</th><th>TT</th> <th>BB/TC</th><th>HK</th></tr></thead><tbody>';
                console.log(tab,str)
                $.each(json.khdaotao, function (i, item) {
                    i++;
                    strmh += '<tr><td style="text-align:center">' + i + '</td><td style="text-align:center">' + item.Mota + '</td><td style="text-align:center">' + item.TenHP + '</td><td style="text-align:center">' + item.SoTC + '</td><td style="text-align:center">' + item.GioLT + '</td><td style="text-align:center">' + item.GioTH + '</td> <td style="text-align:center">' + item.GioDA + '</td><td style="text-align:center">' + item.GioTT + '</td> <td style="text-align:center">' + item.Hinhthuc + '</td><td style="text-align:center">' + item.Hocky + '</td></tr>';
                });
                strmh += '</tbody></table></div></div></div></section>';
                var lop = ' <div class="control-group"><select class="select2 form-control custom-select" id = "khoilop"><option value="" hidden>Chọn khối lớp</option>';
                $.each(json.khoilop, function (i, item) {
                    lop += '<option value="' + item.MaKhoi + '">' + item.TenKhoi + '</option>';
                });
                lop += '</select></div>';
                //Gọi thẻ div và thêm dữ liệu vào
                $('#the').html(tab);
                $('#noidung').html(str);
                $('#monhoc').html(strmh);
                $('#lop').html(lop);
                jsonapi = json;
                //alert(json);
            } else {
                alert(json.Message);
            }

            //location.reload();
        }
    });
});
//Hiện thông báo
function ShowMessage() {
    var result = confirm("Bạn có muốn lưu thông tin?");
    if (result) {
        return true;
    } else {
        return false;
    }
}

document.getElementById('btnAddSubject').addEventListener("click", function (event) {
    var nganh = $('#khoilop').val();

    var data = {
        khoilop: nganh,
        ctdtao: jsonapi.ctdtao,
        khdaotao: jsonapi.khdaotao
    };
    console.log(data);
    $.ajax({
        async: true,
        data: JSON.stringify(data),
        type: 'POST',
        url: '/CTDTKeHoach/Create',
        dataType: "json",
        contentType: "application/json",
        cache: false,
        error: function (json) {
            alert(json.Message);
        },
        success: function (json) {
            if (json.Message == undefined) {
                toastr.success(json.Message);
            } else {               
                $('#addCTDTKHModal').modal('hide');
                $("#zero_config tbody").empty();
                $("#zero_config tbody").append('<tr></tr>');
                toastr.success(json.Message, 'Thông báo');
                $.each(json.data, function (i, item) {
                    i++;
                    $('#zero_config tr:last').after('<tr><td style="text-align:center">' + i + '</td><td style="text-align:center">' + item.Trinhdo + '</td><td style="text-align:center">' + item.Mota + '</td><td style="text-align:center">' + item.TenKhoi + '</td><td style="text-align:center">' + item.Tinhtrang + '<td style="text-align:center"><a href="/CTDTKeHoach/EditCTDTKehoachStep1/' + item.MaCTDT +'"><img src="/images/IconEdit.png" style="width:28px;"></a><a title="Ðánh giá" href="#" data-toggle="modal" data-target="#evaluateCTDT" style="margin:5px;"><img src="/images/IconCheck2.png" style="width:35px;"></a></td></tr>');
                });
            }

        }
    });
});