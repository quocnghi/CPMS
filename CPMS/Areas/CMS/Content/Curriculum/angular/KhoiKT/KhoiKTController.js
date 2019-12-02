app.controller("KhoiKTController", function ($scope, KhoiKTService, $window) {
    loadKhoiKT();
    $scope.KHOIKT = [];
    $scope.LoaiKT = [];

    function loadKhoiKT() {
        KhoiKTService.loadKhoiKT().success(function (pro) {
            console.log(pro)
            $scope.KHOIKT = pro.ls;
            $scope.LoaiKT = pro.list;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    // Adding khoa
    $scope.addKhoiKT = function (tenkt, loai) {
        data = { khoikienthuc: tenkt, TenLoai: loai };
        console.log(data)
        KhoiKTService.addKhoiKT(data).success(function (msg) {
            toastr.success(msg.msg);
            $scope.KHOIKT = msg.KHoi;
        }, function () {
            alert('Error in adding record');
        });
    }
    $scope.addKKT = function (tenkt, loai) {
        data = { TEN: tenkt, LOAI: loai };
        console.log(data)
        KhoiKTService.addKKT(data).success(function (msg) {
            toastr.success(msg.msg);
            $scope.KHOIKT = msg.kienthuc;
        }, function () {
            alert('Error in adding record');
        });
    }
    $scope.addLoai = function (data) {
        console.log(data)
        KhoiKTService.addLoai(data).success(function (msg) {
            toastr.success(msg.msg);
            $scope.KHOIKT = msg.loaikt;
        }, function () {
            alert('Error in adding record');
        });
    }
    // lay du khoa
    $scope.getKhoiKT = function (data) {
        KhoiKTService.getKhoiKT(data).success(function (msg) {
            $scope.sua = msg.KhOi;
        }, function () {
            alert('Error in adding record');
        });
    }

    // Edit khoa
    $scope.editKhoiKT = function (data) {
        KhoiKTService.editKhoiKT(data).success(function (msg) {
            toastr.success(msg.msg);
            $scope.KHOIKT = msg.kthuc;
        }, function () {
            alert('Error in adding record');
        });
    }

    // delete khoa
    $scope.deleteKhoiKT = function (data, index) {
        KhoiKTService.deleteKhoiKT(data.MaKhoiKT).success(function (msg) {
            toastr.success(msg.mg);
            $scope.KHOIKT.splice(index, 1);
        }, function () {
            alert('Error in adding record');
        });
    }

});