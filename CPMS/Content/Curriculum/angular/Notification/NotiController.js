app.controller("NotificationController", function ($scope, NotificationService, $sce) {
    getallEmail();
    $scope.emails = [];
    $scope.count = 0

    function getallEmail() {
        NotificationService.getMail().success(function (pro) {
            angular.forEach(pro.emails, function (value, key) {
                if (value.Ngaytao != null) {
                    var date = new Date(parseInt(value.Ngaytao.replace('/Date(', '')));
                    value.Ngaytao = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
                }
                if (value.DaXem == false) {
                    $scope.count++;
                }              
            })
            $scope.emails = pro.emails
            $scope.show = true;
            $scope.btnshow = true;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    $scope.detail = function (index, id) {
        $scope.count = 0;
        NotificationService.seen(id).success(function (pro) {
            angular.forEach(pro.emails, function (value, key) {
                if (value.Ngaytao != null) {
                    var date = new Date(parseInt(value.Ngaytao.replace('/Date(', '')));
                    value.Ngaytao = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
                }
                if (value.DaXem == false) {
                    $scope.count++;
                } 
            })
            
            $scope.emails = pro.emails
            $scope.ngaytao = $scope.emails[index].Ngaytao
            $scope.Tinhtrang = $scope.emails[index].Tinhtrang
            $scope.MaDC = $scope.emails[index].MaDC
            console.log($scope.MaDC)
            $scope.noidung = $sce.trustAsHtml($scope.emails[index].Thongtin);
            if ($scope.Tinhtrang == null) {
                $scope.btnshow = false
            }
            $scope.show = false;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });       
    };

    $scope.back = function () {
        $scope.show = true;
    };

    $scope.accept = function (maqh) {
        NotificationService.accept(maqh).success(function (pro) {
            angular.forEach(pro.emails, function (value, key) {
                if (value.Ngaytao != null) {
                    var date = new Date(parseInt(value.Ngaytao.replace('/Date(', '')));
                    value.Ngaytao = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
                }
                if (value.DaXem == false) {
                    $scope.count++;
                }
            })
            $scope.emails = pro.emails
            $scope.show = true;
            $scope.btnshow = true;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    };

    $scope.decline = function (maqh) {
        NotificationService.decline(maqh).success(function (pro) {
            angular.forEach(pro.emails, function (value, key) {
                if (value.Ngaytao != null) {
                    var date = new Date(parseInt(value.Ngaytao.replace('/Date(', '')));
                    value.Ngaytao = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
                }
                if (value.DaXem == false) {
                    $scope.count++;
                }
            })
            $scope.emails = pro.emails
            $scope.show = true;
            $scope.btnshow = true;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    };
});