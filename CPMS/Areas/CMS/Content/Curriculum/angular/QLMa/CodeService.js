app.service("CodeService", function ($http) {

    this.getCode = function () {
        return $http.post("/ConfigManagement/loadInfo/");
    }
    this.getDetailCode = function (id) {
        return $http.post("/ConfigManagement/getCode/" + id);
    }
    this.Edit = function (data) {
        return $http({
            method: 'POST',
            url: '/ConfigManagement/Edit',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }

});