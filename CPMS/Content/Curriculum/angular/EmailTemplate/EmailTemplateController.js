app.controller("EmailTemplateController", function ($scope, EmailTemplateService, $window, $sce) {
    getallEmail();
    $scope.emails = [];

    function getallEmail() {
        EmailTemplateService.getEmail().success(function (pro) {
            angular.forEach(pro.email, function (value, key) {
                if (value.Noidung != null) {
                    value.Noidung = $sce.trustAsHtml(value.Noidung);
                }              
            })           
            $scope.emails = pro.email;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    $scope.add = function () {
        $scope.Action = 'Thêm'
        $scope.show = true;
        $scope.email = null;
    };

    $scope.edit = function (email) {
        $scope.Action = 'Chỉnh sửa'
        $scope.show = false;
        EmailTemplateService.getDetailEmail(email.MaET).success(function (pro) {
            CKEDITOR.instances['Noidung'].setData(pro.email.Noidung)
            $scope.email = pro.email;
        }).error(function () {
            alert('Có lỗi xảy ra');
        });
    }

    $scope.addEmail = function (email) {
        email.Noidung = CKEDITOR.instances['Noidung'].getData();
        EmailTemplateService.addEmail(email).success(function (msg) {
            angular.forEach(msg.email, function (value, key) {
                if (value.Noidung != null) {
                    value.Noidung = $sce.trustAsHtml(value.Noidung);
                }
            })
            toastr.success(msg.msg);
            $scope.emails = msg.email;
        }, function () {
            alert('Có lỗi xảy ra');
        });
    }

    $scope.editEmail = function (email) {
        email.Noidung = CKEDITOR.instances['Noidung'].getData();
        EmailTemplateService.editEmail(email).success(function (msg) {
            angular.forEach(msg.email, function (value, key) {
                if (value.Noidung != null) {
                    value.Noidung = $sce.trustAsHtml(value.Noidung);
                }
            })
            toastr.success(msg.msg);
            $scope.emails = msg.email;         
        }, function () {
            alert('Có lỗi xảy ra');
        });
    }

    $scope.delete = function (id, index) {
        EmailTemplateService.deleteEmail(id).success(function (msg) {
            toastr.success(msg.msg);
            $scope.emails.splice(index, 1);
        }, function () {
            alert('Có lỗi xảy ra');
        });
    }
});