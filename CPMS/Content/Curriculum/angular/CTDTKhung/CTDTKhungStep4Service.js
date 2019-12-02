app.service("CTDTKhungStep4Service", function ($http) {
    this.getInfo = function () {
        return $http.post("/CTDTKhung/loadInfo");
    }

    this.addGVien = function(data) {
        return $http({
            method: 'POST',
            url: '/CTDTKhung/addGV',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }

    this.saveEmail = function (data) {
        return $http({
            method: 'POST',
            url: '/CTDTKhung/saveEmail',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }

    this.sendEmail = function (id) {
        return $http.post("/CTDTKhung/sendEmail/"+id);
    }

    this.chEmail = function (data) {
        return $http({
            method: 'POST',
            url: '/CTDTKhung/showEmail',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
});