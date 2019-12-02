app.service("NhanvienService", function ($http) {
    this.loadListNV = function () {
        return $http.post("/QuanLyUsers/loadListNhavien");
    }

    this.addNhanvien = function (data) {
        return $http({
            method: 'POST',
            url: '/QuanLyUsers/addNhanvien',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.getNhanvien = function (data) {
        return $http({
            method: 'POST',
            url: '/QuanLyUsers/getNhanvien',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.editNhanvien = function (data) {
        return $http({
            method: 'POST',
            url: '/QuanLyUsers/editNhanvien',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.deleteNhanvien = function (id) {
        return $http.post("/QuanLyUsers/deleteNhanvien/" + id);
    }
});