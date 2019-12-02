app.controller("EditCTDTKhungStep4Controller", function ($scope, EditCTDTKhungStep4Service, $window, $location) {
    getallInfo();
    $scope.tenGV = [];
    $scope.emails = [];
    $scope.giangVien = [];
    $scope.nhanVien = []
    $scope.selectedValues = [];
    var slmonhoc = 0;
    var count = 0;

    function getallInfo() {
        EditCTDTKhungStep4Service.getInfo().success(function (pro) {
            $scope.monHoc = pro.monHoc;
            slmonhoc = pro.monHoc.length;
            console.log(pro.monHoc)
            $scope.nhanVien = pro.nhanVien;
            angular.forEach(pro.monHoc, function (value, key) {
                if (value != null) {
                    var tenGV = '';
                    angular.forEach(value.gVien, function (val, k) {
                        if (val != null) {
                            console.log(key)
                            tenGV += val.Ho + ' ' + val.Ten + '-';
                            $scope.tenGV[key] = tenGV;
                            if (count < slmonhoc) {
                                count++;
                            } else if (count >= slmonhoc) {
                                count = slmonhoc
                            }
                        }

                    })
                }

            })

        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    $scope.getValue = function (index) {
        EditCTDTKhungStep4Service.getInfo().success(function (pro) {
            $scope.monHoc = pro.monHoc;
            $scope.selectedValues = []
            console.log($scope.monHoc[index])
            var iD = $scope.monHoc[index].MaKHDT;
            var ten = $scope.monHoc[index].TenMH;
            $scope.mHocId = iD;
            $scope.mHocTen = ten;
            $scope.stt = index;
            $scope.dcnhanvien = [];
            angular.forEach($scope.monHoc[index].gVien, function (val) {
                $scope.selectedValues.push(val.NguoiST.toString());
                $scope.dcnhanvien.push(val.MaQL.toString());
            });
            console.log($scope.selectedValues)
            var model = angular.element('#nvien');
            model.val($scope.selectedValues).change();
            console.log(model)
        }).error(function () {
            alert('Có lỗi xảy ra');
        });


    };

    $scope.addGV = function (mamh, dcuong, index) {
        var data = { MaKHDT: mamh, Id: dcuong, MaQL: $scope.dcnhanvien}
        console.log(data);
        if (dcuong != undefined) {
            EditCTDTKhungStep4Service.editVien(data).success(function (msg) {
                console.log(msg);

                angular.forEach(msg.tenGVPC, function (value, key) {
                    if (value != null) {
                        var tenGV = '';
                        angular.forEach(value.gVien, function (val, k) {
                            if (val != null) {
                                tenGV += val.Ho + ' ' + val.Ten + '-';
                                $scope.tenGV[key] = tenGV;
                            }
                        })
                    }
                })
                nVien = msg.nv;
                var model = angular.element('#addGV');
                model.modal('hide');
                if (count < slmonhoc) {
                    count++;
                } else if (count >= slmonhoc) {
                    count = slmonhoc
                }
                console.log(count);
                toastr.success(msg.msg);
            }, function () {
                alert('Có lỗi xảy ra');
            });
        } else {
            var model = angular.element('#addGV');
            model.modal('hide');
        }
    }

    $scope.saveEmail = function (e) {
        e.Noidung = CKEDITOR.instances['Noidung'].getData();
        EditCTDTKhungStep4Service.saveEmail(e).success(function (ret) {
            toastr.success(ret.msg);
        }, function () {
            alert('Có lỗi xảy ra');
        });
    }

    $scope.finish = function (e) {
        EditCTDTKhungStep4Service.sendEmail(e.MaET).success(function (ret) {
            toastr.success(ret.msg);
            $window.location.href = '/CMS/CTDTKhung/List';
        }, function () {
            alert('Có lỗi xảy ra');
        });
    }

    $scope.chooseEmail = function (date) {
        var data = {NgayHT: date}
        var location = $location.host() + $location.port();;
        console.log(data, location)
        if (count == slmonhoc) {
            var model = angular.element('#chooseEmailModal');
            model.modal('show');
            EditCTDTKhungStep4Service.chEmail(data).success(function (ret) {
                console.log(ret)
                var gVienHP = ret.gVien
                var countGV = 0;
                angular.forEach(ret.gVien, function (val, k) {
                    countGV = 0;
                    angular.forEach(gVienHP, function (value, key) {
                       
                        if (val.NguoiST == value.NguoiST) {
                            countGV++;
                        }
                        if (countGV > 1) {
                            console.log(countGV)
                            gVienHP.splice(key, 1);
                            countGV--;
                        }
                        
                    });
                });
                $scope.giangVien = gVienHP;
                $scope.ngGui = ret.ngGui;
                $scope.emails = ret.email
            }, function () {
                alert('Có lỗi xảy ra');
            });
        } else {
            toastr.warning('Vui lòng thêm giảng viên cho mọi môn học');
        }
    }

    $scope.chooseDetailEmail = function () {
        console.log($scope.email)
        CKEDITOR.instances['Noidung'].setData($scope.email.Noidung)
    }
});