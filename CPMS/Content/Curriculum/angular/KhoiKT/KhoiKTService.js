app.service("KhoiKTService", function ($http) {
    this.loadKhoiKT = function () {
        return $http.post("/CategoriesManagement/loadKhoiKT");
    }

    this.addKhoiKT = function (data) {
        return $http({
            method: 'POST',
            url: '/CategoriesManagement/addKhoiKT',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.getKhoiKT = function (data) {
        return $http({
            method: 'POST',
            url: '/CategoriesManagement/getKhoiKT',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.editKhoiKT = function (data) {
        return $http({
            method: 'POST',
            url: '/CategoriesManagement/editKhoiKT',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.deleteKhoiKT = function (id) {
        return $http.post("/CategoriesManagement/deleteKhoiKT/" + id);
    }
});