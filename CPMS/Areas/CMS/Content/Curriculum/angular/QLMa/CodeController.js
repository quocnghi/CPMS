app.controller("CodeController", function ($scope, CodeService, $window, $sce) {
    getCode();
    $scope.emails = [];

    function getCode() {
        CodeService.getCode().success(function (pro) {
            angular.forEach(pro.list, function (value, key) {
                if (value.LoaiMa != null) {
                    if (value.LoaiMa == '1') {
                        value.LoaiMa = 'Phiên bản'
                    } else if (value.LoaiMa == '2') {
                        value.LoaiMa = 'Môn học'
                    } else if (value.LoaiMa == '3') {
                        value.LoaiMa = 'Nhân viên'
                    } else if (value.LoaiMa == '4') {
                        value.LoaiMa = 'Kết quả học tập mong đợi CTĐT'
                    } else if (value.LoaiMa == '5') {
                        value.LoaiMa = 'Kết quả học tập mong đợi học phần'
                    }
                }
            })           
            $scope.codes = pro.list;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    $scope.edit = function (code) {
        CodeService.getDetailCode(code.MaCH).success(function (pro) {
            console.log(pro)
            $scope.code = pro.code;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    $scope.EditCode = function (code) {
        console.log(code)
        CodeService.Edit(code).success(function (msg) {
            angular.forEach(msg.code, function (value, key) {
                if (value.LoaiMa != null) {
                    if (value.LoaiMa == '1') {
                        value.LoaiMa = 'Phiên bản'
                    } else if (value.LoaiMa == '2') {
                        value.LoaiMa = 'Môn học'
                    } else if (value.LoaiMa == '3') {
                        value.LoaiMa = 'Nhân viên'
                    } else if (value.LoaiMa == '4') {
                        value.LoaiMa = 'Kết quả học tập mong đợi CTĐT'
                    } else if (value.LoaiMa == '5') {
                        value.LoaiMa = 'Kết quả học tập mong đợi học phần'
                    }
                }
            })           
            toastr.success(msg.msg);
            $scope.codes = msg.code;
        }, function () {
            alert('Có lỗi xảy ra');
        });
    }
});