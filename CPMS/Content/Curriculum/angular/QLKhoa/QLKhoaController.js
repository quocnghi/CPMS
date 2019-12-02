app.controller("QLKhoaController", function ($scope, QLKhoaService, $window) {
    loadKhoa();
    $scope.KHOA = [];

    function loadKhoa() {
        QLKhoaService.loadKhoa().success(function (pro) {
            console.log(pro)
            $scope.KHOA = pro.list;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    // Adding khoa
    $scope.addKhoa = function (data) {
        QLKhoaService.addKhoa(data).success(function (msg) {
            toastr.success(msg.msg);
            $scope.KHOA = msg.KHoa;
        }, function () {
            alert('Error in adding record');
        });
    }

    // lay du khoa
    $scope.getKhoa = function (data) {
        QLKhoaService.getKhoa(data).success(function (msg) {
            $scope.sua = msg.KhOa;
        }, function () {
            alert('Error in adding record');
        });
    }

    // Edit khoa
    $scope.editKhoa = function (data) {
        QLKhoaService.editKhoa(data).success(function (msg) {
            toastr.success(msg.msg);
            $scope.KHOA = msg.khoA;
        }, function () {
            alert('Error in adding record');
        });
    }

    // delete khoa
    $scope.deleteKhoa = function (data, index) {
        QLKhoaService.deleteKhoa(data.MaKhoa).success(function (msg) {
            toastr.success(msg.msg);
            $scope.KHOA.splice(index, 1);
        }, function () {
            alert('Error in adding record');
        });
    }

});