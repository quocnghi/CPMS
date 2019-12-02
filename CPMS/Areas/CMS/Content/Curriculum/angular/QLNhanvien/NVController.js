app.controller("nhanvienController", function ($scope, NhanvienService) {
    loadListNV();
    $scope.nhanviens = [];

    function loadListNV() {
        NhanvienService.loadListNV().success(function (pro) {
            console.log(pro)
            angular.forEach(pro.list, function (value, key) {
                if (value.Gioitinh != null) {
                    if (value.Gioitinh == '0') {
                        value.Gioitinh = 'Nam';
                    } else if (value.Gioitinh == '1') {
                        value.Gioitinh = 'Nữ';
                    }
                }
                if (value.LoaiGV != null) {
                    if (value.LoaiGV == '0') {
                        value.LoaiGV = 'Giảng viên cơ hữu';
                    } else if (value.LoaiGV == '1') {
                        value.LoaiGV = 'Giảng viên thỉnh giảng';
                    } else if (value.LoaiGV == '2') {
                        value.LoaiGV = 'Nhân viên';
                    }
                }
                if (value.Ngaysinh != null) {
                    var date = new Date(parseInt(value.Ngaysinh.replace('/Date(', '')));
                    value.Ngaysinh = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
                }
            })
            $scope.nhanviens = pro.list;
            $scope.manv = pro.ma;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    $scope.addNV = function () {
        $scope.Action = 'Thêm';
        $scope.show = true;
        $scope.nhanvien = null;
        $scope.ma = $scope.manv.TenMa + '-' + $scope.manv.Phienban;
        if ($scope.nhanviens[$scope.nhanviens.length - 1].MaQL == $scope.ma) {
            var pb = parseInt($scope.manv.Phienban);
            pb++;
            if (pb < 10) {
                $scope.manv.Phienban = "00" + pb
            } else if (pb >= 10 && pb < 100) {
                $scope.manv.Phienban = "0" + pb
            } else if (pb >= 100) {
                $scope.manv.Phienban = "" + pb
            }
            $scope.ma = $scope.manv.TenMa + '-' + $scope.manv.Phienban;
        }
    }

    // Adding nhan vien
    $scope.addNhanvien = function (ma, data) {
        data.MaQL = ma
        console.log(data)
        NhanvienService.addNhanvien(data).success(function (msg) {
            angular.forEach(msg.list, function (value, key) {
                if (value.Gioitinh != null) {
                    if (value.Gioitinh == '0') {
                        value.Gioitinh = 'Nam';
                    } else if (value.Gioitinh == '1') {
                        value.Gioitinh = 'Nữ';
                    }
                }
                if (value.LoaiGV != null) {
                    if (value.LoaiGV == '0') {
                        value.LoaiGV = 'Giảng viên cơ hữu';
                    } else if (value.LoaiGV == '1') {
                        value.LoaiGV = 'Giảng viên thỉnh giảng';
                    } else if (value.LoaiGV == '2') {
                        value.LoaiGV = 'Nhân viên';
                    }
                }
                if (value.Ngaysinh != null) {
                    var date = new Date(parseInt(value.Ngaysinh.replace('/Date(', '')));
                    value.Ngaysinh = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
                }
            })
            console.log(msg);
            toastr.success(msg.msg);
            $scope.nhanviens = msg.list;
        }, function () {
            alert('Error in adding record');
        });
    }

    // lay du lieu nhan vien
    $scope.getNhanvien = function (data) {
        NhanvienService.getNhanvien(data).success(function (msg) {
            $scope.Action = 'Chỉnh sửa';
            $scope.show = false;
            if (msg.nv.Ngaysinh != null) {
                var date = new Date(parseInt(msg.nv.Ngaysinh.replace('/Date(', '')));
                msg.nv.Ngaysinh = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
            }
            $scope.nhanvien = msg.nv;
        }, function () {
            alert('Error in adding record');
        });
    }

    // Edit nhân viên
    $scope.editNhanvien = function (data) {
        NhanvienService.editNhanvien(data).success(function (msg) {
            angular.forEach(msg.list, function (value, key) {
                if (value.Gioitinh != null) {
                    if (value.Gioitinh == '0') {
                        value.Gioitinh = 'Nam';
                    } else if (value.Gioitinh == '1') {
                        value.Gioitinh = 'Nữ';
                    }
                }
                if (value.LoaiGV != null) {
                    if (value.LoaiGV == '0') {
                        value.LoaiGV = 'Giảng viên cơ hữu';
                    } else if (value.LoaiGV == '1') {
                        value.LoaiGV = 'Giảng viên thỉnh giảng';
                    } else if (value.LoaiGV == '2') {
                        value.LoaiGV = 'Nhân viên';
                    }
                }
                if (value.Ngaysinh != null) {
                    var date = new Date(parseInt(value.Ngaysinh.replace('/Date(', '')));
                    value.Ngaysinh = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
                }
            })
            toastr.success(msg.msg);
            $scope.nhanviens = msg.list;
        }, function () {
            alert('Error in adding record');
        });
    }

    // delete nhân viên
    $scope.deleteNhanvien = function (data, index) {
        NhanvienService.deleteNhanvien(data.MaNV).success(function (msg) {
            toastr.success(msg.msg);
            $scope.nhanviens.splice(index, 1);
        }, function () {
            alert('Error in adding record');
        });
    }

});