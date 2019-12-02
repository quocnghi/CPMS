app.controller("HPController", function ($scope, HPService, $window) {
    getds();
    getdshp();
    $scope.DSHP = [];

    function getds() {
        HPService.getds().success(function (pro) {
            console.log(pro)
            angular.forEach(pro.dsHp, function (value, key) {
                if (value.MaCDR.length > 0) {
                    var maHT = "";
                    angular.forEach(value.MaCDR, function (valuer, key) {
                        if (valuer.MaHT != null) {
                            maHT += valuer.MaHT + ';';
                            valuer.MaHT = maHT;
                        }

                    })
                } else {
                    value.MaCDR = null;
                }

            })
            $scope.DSHP = pro.dsHp;
        }).error(function (e) {
            alert('Có lỗi xảy ra' + e);
        });
    }
    function getdshp() {
        HPService.getds().success(function (pro) {
            console.log(pro)
            angular.forEach(pro.dsHp, function (value, key) {
                if (value.MaCDR.length > 0) {
                    var maHT = "";
                    angular.forEach(value.MaCDR, function (valuer, key) {
                        if (valuer.MaHT != null) {
                            maHT += valuer.MaHT + ';';
                            valuer.MaHT = maHT;
                        }

                    })
                } else {
                    value.MaCDR = null;
                }

            })
            $scope.DSHP = pro.dsHp;
        }).error(function (e) {
            //alert('Có lỗi xảy ra' + e);
        });
    }
});