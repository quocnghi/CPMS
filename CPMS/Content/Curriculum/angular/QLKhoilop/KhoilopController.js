app.controller("KhoilopController", function ($scope, KhoilopService, $window) {
    loadList();
    $scope.khoilops = [];

    function loadList() {
        KhoilopService.loadList().success(function (pro) {
            console.log(pro)
            $scope.khoilops = pro.list;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    // Adding khoi lop
    $scope.addKhoilop = function (data) {
        KhoilopService.addKhoilop(data).success(function (msg) {
            toastr.success(msg.mg);
            $scope.khoilops = msg.LOP;
        }, function () {
            alert('Error in adding record');
        });
    }

    // lay du khoi lop
    $scope.getKhoilop = function (data) {
        KhoilopService.getKhoilop(data).success(function (msg) {
            $scope.sua = msg.klop;
        }, function () {
            alert('Error in adding record');
        });
    }

    // Edit khoi lop
    $scope.editKhoilop = function (data) {
        KhoilopService.editKhoilop(data).success(function (msg) {
            toastr.success(msg.mg);
            $scope.khoilops = msg.khoi;
        }, function () {
            alert('Error in adding record');
        });
    }

    // delete khoi lop
    $scope.deleteKhoilop = function (data,index) {
        KhoilopService.deleteKhoilop(data.MaKhoi).success(function (msg) {
            toastr.success(msg.mg);
            $scope.khoilops.splice(index,1);
        }, function () {
            alert('Error in adding record');
        });
    }

});