app.controller("QLHenganhController", function ($scope, QLHanganhService, $window) {
    loadInfo();
    $scope.henganhs = [];
    $scope.KHOA = [];

    function loadInfo() {
        QLHanganhService.loadInfo().success(function (pro) {
            console.log(pro)
            $scope.henganhs = pro.list;
            $scope.KHOA = pro.listkhoa;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    // Adding he nganh
    $scope.addHenganh = function (data) {
        QLHanganhService.addHenganh(data).success(function (msg) {
            toastr.success(msg.msg);
            $scope.henganhs = msg.heNganh;
        }, function () {
            alert('Error in adding record');
        });
    }

    // lay du lieu he nganh
    $scope.gethenganh = function (He) {
        QLHanganhService.gethenganh(He).success(function (msg) {
            $scope.editHn = msg.He;
            
        }, function () {
            alert('Error in adding record');
        });
    }

    // Edit he nganh
    $scope.edithenganh = function (data) {
        QLHanganhService.edithenganh(data).success(function (msg) {
            toastr.success(msg.msg);
            console.log(msg)
            $scope.henganhs = msg.HNganh;
        }, function () {
            alert('Error in adding record');
        });
    }

    // delete he nganh
    $scope.deletehenganh = function (hhe, index) {
        QLHanganhService.deletehenganh(hhe.MaHN).success(function (msg) {
            toastr.success(msg.msg);
            $scope.henganhs.splice(index, 1);
        }, function () {
            alert('Error in adding record');
        });
    }
    
});