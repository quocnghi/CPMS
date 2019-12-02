app.controller("MatranController", function ($scope, MatranService, $window) {
    getKHDTlist();
    $scope.khdts = [];

    function getKHDTlist(id) {
        MatranService.getKHDTlist(id).success(function (pro) {
            console.log(pro)
                $scope.khdts = pro.khdt;
        }).error(function (e) {
            alert('Có lỗi xảy ra' + e);
        });
    }

    // review noi dung
    $scope.getreview = function (id) {
        MatranService.getreview(id).success(function (pro) {
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

            $scope.TenHP = pro.mhoc.TenHP;
            $scope.MaHP = pro.mhoc.MaHP;
            $scope.TenCTDT = pro.mhoc.TenCTDT;
            $scope.SoTC = pro.mhoc.SoTC;
            $scope.GioLT = pro.mhoc.GioLT;
            $scope.GioTH = pro.mhoc.GioTH;
            $scope.GioDA = pro.mhoc.GioDA;
            $scope.GioTT = pro.mhoc.GioTT;
            $scope.Hocky = pro.mhoc.Hocky;
            $scope.NgonnguGD = pro.mhoc.NgonnguGD;
            $scope.Mota = pro.mhoc.Mota;
            $scope.MonTQ = pro.mhoc.MonTQ;
            $scope.MonHT = pro.mhoc.MonHT;
            $scope.NoidungCN = pro.mhoc.NoidungCN;
            $scope.MuctieuHP = pro.mhoc.MuctieuHP;
            $scope.MotaKienthuc = pro.mhoc.MotaKienthuc;
            $scope.PPGD = pro.mhoc.PPGD;
            $scope.PPHT = pro.mhoc.PPHT;
            $scope.NhiemvuSV = pro.mhoc.NhiemvuSV;
            $scope.TailieuHT = pro.mhoc.TailieuHT;
            $scope.Thangdiem = pro.mhoc.Thangdiem;
            $scope.PhuongtienGD = pro.mhoc.PhuongtienGD;
            $scope.PhuongtienThi = pro.mhoc.PhuongtienThi;
            $scope.NoidungCN = pro.mhoc.NoidungCN;
            $scope.KKienThuc = pro.KKT;
            $scope.Hinhthuc = pro.mhoc.Hinhthuc;
            $scope.CDRCTDT = pro.CDR;
            $scope.cdrmonhocs = pro.cdrhp;
            $scope.matrans = pro.mtcdr;
            $scope.noidunggds = pro.ndhp;
            $scope.LoaiTL = pro.tHP.LoaiTL;
            $scope.TenTL = pro.tHP.TenTL;
            $scope.Tacgia = pro.tHP.Tacgia;
            $scope.NhaXB = pro.tHP.NhaXB;
            $scope.NamXB = pro.tHP.NamXB;
            $scope.TenHT = pro.ndhp.TenHT;
            $scope.Noidung = pro.ndhp.Noidung;
            $scope.Phanloai = pro.ndhp.Phanloai;
            $scope.Mota = pro.ndhp.Mota;



        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }
});