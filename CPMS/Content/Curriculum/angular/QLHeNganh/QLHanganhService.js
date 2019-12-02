app.service("QLHanganhService", function ($http) {
    this.loadInfo = function () {
        return $http.post("/CategoriesManagement/loadInfo");
    }

    this.addHenganh = function (data) {
        return $http({
            method: 'POST',
            url: '/CategoriesManagement/addHenganh',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.gethenganh = function (data) {
        return $http({
            method: 'POST',
            url: '/CategoriesManagement/gethenganh',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.edithenganh = function (data) {
        return $http({
            method: 'POST',
            url: '/CategoriesManagement/edithenganh',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.deletehenganh = function (id) {
        return $http.post("/CategoriesManagement/deletehenganh/" + id);
    }
});