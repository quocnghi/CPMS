app.controller("vaitroController", function ($scope, VaitroService) {
    loadListUser();
    $scope.users = [];
    $scope.nhanviens = [];
    $scope.vaitros = [];
    $scope.Id = null;

    function loadListUser() {
        VaitroService.loadListUser().success(function (pro) {
            console.log(pro)
            angular.forEach(pro.list, function (value, key) {
                if (value.Tenvaitro != null) {
                    if (value.Tenvaitro == 'Editor') {
                        value.Tenvaitro = 'Người soạn thảo';
                    } else if (value.Tenvaitro == 'HeadofEditor') {
                        value.Tenvaitro = 'Trưởng ban soạn thảo';
                    } else if (value.Tenvaitro == 'Valuer') {
                        value.Tenvaitro = 'Người đánh giá';
                    }
                }
                if (value.Nhanvien != null) {
                    if (value.Nhanvien == '0') {
                        value.Nhanvien = 'Giảng viên cơ hữu';
                    } else if (value.Nhanvien == '1') {
                        value.Nhanvien = 'Giảng viên thỉnh giảng';
                    } else if (value.Nhanvien == '2') {
                        value.Nhanvien = 'Nhân viên';
                    }
                }
            })
            angular.forEach(pro.listvt, function (value, key) {
                if (value.Name != null) {
                    if (value.Name == 'Editor') {
                        value.Name = 'Người soạn thảo';
                    } else if (value.Name == 'HeadofEditor') {
                        value.Name = 'Trưởng ban soạn thảo';
                    } else if (value.Name == 'Valuer') {
                        value.Name = 'Người đánh giá';
                    }
                }
            })
            $scope.users = pro.list;
            $scope.nhanviens = pro.listnv;
            $scope.vaitros = pro.listvt;         
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    // Adding nhan vien
    $scope.editUser = function (inp) {
        var id = $scope.Id;
        var data = { Id: id, MaNV: inp.MaNV, Vaitro : inp.Vaitro }
        VaitroService.editUser(data).success(function (msg) {
            console.log(msg);
            angular.forEach(msg.list, function (value, key) {
                if (value.Tenvaitro != null) {
                    if (value.Tenvaitro == 'Editor') {
                        value.Tenvaitro = 'Người soạn thảo';
                    } else if (value.Tenvaitro == 'HeadofEditor') {
                        value.Tenvaitro = 'Trưởng ban soạn thảo';
                    } else if (value.Tenvaitro == 'Valuer') {
                        value.Tenvaitro = 'Người đánh giá';
                    }
                }
                if (value.Nhanvien != null) {
                    if (value.Nhanvien == '0') {
                        value.Nhanvien = 'Nhân viên cơ hữu';
                    } else if (value.Nhanvien == '1') {
                        value.Nhanvien = 'Nhân viên thỉnh giảng';
                    } else if (value.Nhanvien == '2') {
                        value.Nhanvien = 'Nhân viên';
                    }
                }
            })
            toastr.success(msg.msg);
            $scope.users = msg.list;
        }, function () {
            alert('Error in adding record');
        });
    }

    // lay du lieu nhan vien
    $scope.edit = function (data) {
        $scope.Id = data.Id;
        $scope.user = data;
        console.log($scope.user);
    }

});