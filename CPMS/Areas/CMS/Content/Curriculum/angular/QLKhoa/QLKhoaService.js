app.service("QLKhoaService", function ($http) {
    this.loadKhoa = function () {
        return $http.post("/CategoriesManagement/loadKhoa");
    }

    this.addKhoa = function (data) {
        return $http({
            method: 'POST',
            url: '/CategoriesManagement/addKhoa',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.getKhoa = function (data) {
        return $http({
            method: 'POST',
            url: '/CategoriesManagement/getKhoa',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.editKhoa = function (data) {
        return $http({
            method: 'POST',
            url: '/CategoriesManagement/editKhoa',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.deleteKhoa = function (id) {
        return $http.post("/CategoriesManagement/deleteKhoa/" + id);
    }
});