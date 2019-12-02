app.controller("ReviewController", function ($scope, ReviewService, $window) {
    $scope.khdts = [];
    getKHDTlist();

    function getKHDTlist(id) {
        ReviewService.getKHDTlist(id).success(function (pro) {
            console.log(pro)
            $scope.khdts = pro.khdt;
        }).error(function (e) {
            alert('Có lỗi xảy ra' + e);
        });
    }

    // review noi dung
    $scope.getreview = function (id) {
        ReviewService.getreview(id).success(function (pro) {
            console.log(pro)
            angular.forEach(pro.cdrhp, function (value, key) {
                if (value.MaHT1.length > 0) {
                    var maELO = "";
                    angular.forEach(value.MaHT1, function (valuer, key) {
                        if (valuer.MaELO != null) {
                            maELO += valuer.MaELO + '-';
                            valuer.MaELO = maELO;
                        }

                    })
                } else {
                    value.MaHT1 = null;
                }

            })
            angular.forEach(pro.ndhp, function (value, key) {
                if (value.Mota.length > 0) {
                    var maCELO = "";
                    angular.forEach(value.Mota, function (valuer, key) {
                        if (valuer.MaHT != null) {
                            maCELO += valuer.MaHT + '-';
                            valuer.MaHT = maCELO;
                        }

                    })
                } else {
                    value.Mota = null;
                }

            })
            $scope.khdt = pro.mhoc;
            if (pro.mhoc.TrangthaiDC == '3') {
                $scope.checkPD = true;
            } else if (pro.mhoc.TrangthaiDC == '4') {
                $scope.checkTC = true;
            }
            $scope.LoaiPH = pro.loaiPH;
            $scope.KKienThuc = pro.KKT;
            $scope.CDRCTDT = pro.CDR;
            $scope.cdrmonhocs = pro.cdrhp;
            $scope.matrans = pro.mtcdr;
            $scope.noidunggds = pro.ndung;
            angular.forEach(pro.tHP, function (value, key) {
                if (value.NamXB != null) {
                    var date = new Date(parseInt(value.NamXB.replace('/Date(', '')));
                    value.NamXB = date.getFullYear();
                }
            })
            $scope.tailieuthamkhaos = pro.tHP;

        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }
   
});