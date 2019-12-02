app.service("VaitroService", function ($http) {
    this.loadListUser = function () {
        return $http.post("/QuanLyUsers/loadListUser");
    }

    this.editUser = function (data) {
        return $http({
            method: 'POST',
            url: '/QuanLyUsers/editUser',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
});