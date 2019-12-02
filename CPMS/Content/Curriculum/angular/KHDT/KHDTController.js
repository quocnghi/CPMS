app.controller("khdtController", function ($scope, KHDTService, $window) {
    getKHDTlist();
    $scope.khdts = [];
    

    $scope.toCourse = function (id) {
        $window.location.href = '/CourseSyllabus/Course/' + id;
    }

    function getKHDTlist(id) {
        KHDTService.getKHDTlist(id).success(function (pro) {
          
            if (pro.khdt != null) {
                angular.forEach(pro.khdt, function (value, key) {

                    if (value.NgayTao != null) {
                        var date = new Date(parseInt(value.NgayTao.replace('/Date(', '')));
                        value.NgayTao = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
                    }
                    if (value.NgayHT != null) {
                        var date = new Date(parseInt(value.NgayHT.replace('/Date(', '')));
                        value.NgayHT = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
                    }
                    if (value.NgayCN != null) {
                        var date = new Date(parseInt(value.NgayCN.replace('/Date(', '')));
                        value.NgayCN = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
                    }
                    var tenGV = '';
                    var tinhtrang = '';
                    if (value.GVien != null) {
                        angular.forEach(value.GVien, function (val, key) {
                            if (val.Trangthai == null) {
                                tinhtrang = 'Chưa trả lời';
                            } else if (val.Trangthai == true) {
                                tinhtrang = 'Đồng ý';
                            } else if (val.Trangthai == false) {
                                tinhtrang = 'Từ chối'
                            }
                            tenGV = tenGV + val.hoGV + ' ' + val.tenGV + ': ' + tinhtrang +'-';
                            val.hoGV = tenGV;
                        })
                    }
                })
                console.log(pro.khdt)
                $scope.khdts = pro.khdt;
            }          
        }).error(function (e) {
            alert('Có lỗi xảy ra' + e);
        });
    }

     // review noi dung
    $scope.getreview = function (id) {
        KHDTService.getreview(id).success(function (pro) {
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

    $scope.changePD = function () {
        $scope.TC = null;
    }

    $scope.changeTC = function () {
        $scope.PD = null;
    }

    //luu đánh giá
    $scope.Adddanhgia = function (dg,tc,pd) {
        console.log(dg, tc, pd);
        if (pd == null) {
            dg.TrangthaiDC = '3';
            dg.GhichuDG = '';
            KHDTService.Adddanhgia(dg).success(function (msg) {
                toastr.success(msg.mg);
                }, function () {
                    alert('Error in adding record');
                });

        } else if (tc == null) {
            dg.TrangthaiDC = '4';
            KHDTService.Adddanhgia(dg).success(function (msg) {
                toastr.success(msg.mg);
                }, function () {
                    alert('Error in adding record');
                });
        }
    }
});