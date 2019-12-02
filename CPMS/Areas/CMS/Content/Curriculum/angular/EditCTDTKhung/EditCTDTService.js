app.service("EditCTDTKhungStep4Service", function ($http) {
    this.getInfo = function () {
        return $http.post("/CTDTKhung/loadEditInfo");
    }

    this.editVien = function (data) {
        return $http({
            method: 'POST',
            url: '/CTDTKhung/editGV',
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
        return $http.post("/CTDTKhung/EditsendEmail/" + id);
    }

    this.chEmail = function (data) {
        return $http({
            method: 'POST',
            url: '/CTDTKhung/EditshowEmail',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }

    //this.chEmail = function (id) {
    //    return $http.post("/CTDTKhung/EditshowEmail/" + id);
    //}
});