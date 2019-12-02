app.service("KhoilopService", function ($http) {
    this.loadList = function () {
        return $http.post("/CategoriesManagement/loadList");
    }

    this.addKhoilop = function (data) {
        return $http({
            method: 'POST',
            url: '/CategoriesManagement/addKhoilop',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.getKhoilop = function (data) {
        return $http({
            method: 'POST',
            url: '/CategoriesManagement/getKhoilop',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.editKhoilop = function (data) {
        return $http({
            method: 'POST',
            url: '/CategoriesManagement/editKhoilop',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.deleteKhoilop = function (id) {
        return $http.post("/CategoriesManagement/deleteKhoilop/" + id);
    }
});