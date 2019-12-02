app.controller("QLHenganhController", function ($scope, QLHanganhService, $window) {
    loadInfo();
    $scope.henganhs = [];
    $scope.nganhs = [];
    $scope.KHOA = [];

    function loadInfo() {
        QLHanganhService.loadInfo().success(function (pro) {
            console.log(pro)
            $scope.henganhs = pro.list;
            $scope.nganhs = pro.listhnganh;
            $scope.KHOA = pro.listkhoa;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    // Adding he nganh
    $scope.addHenganh = function (hn, tenhe) {      
        var data = { Henganh: hn, TenHe: tenhe }
        console.log(data)
        QLHanganhService.addHenganh(data).success(function (msg) {
            toastr.success(msg.msg);
            $scope.nganhs = msg.heNganh;
        }, function () {
            Console.log(data);
        });
    }
    // Adding he
    $scope.addHe = function (data) {
        console.log(data)
        QLHanganhService.addHe(data).success(function (msg) {
            toastr.success(msg.msg);
            $scope.nganhs = msg.he;
        }, function () {
            Console.log(data);
        }
        );
    }

    // lay du lieu he nganh
    $scope.gethenganh = function (data) {
        console.log(data)
        QLHanganhService.gethenganh(data).success(function (msg) {
            $scope.editHn = msg.He;
            
        }, function () {
            alert('Error in adding record');
        });
    }

    // Edit he nganh
    $scope.editHenganh = function (data) {
        console.log(data)
        QLHanganhService.editHenganh(data).success(function (msg) {
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