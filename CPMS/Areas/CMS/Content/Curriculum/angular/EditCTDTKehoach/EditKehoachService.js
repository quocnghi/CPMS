app.service("EditCTDTKHStep4Service", function ($http) {
    this.getInfo = function () {
        return $http.post("/CTDTKeHoach/loadEditInfo");
    }

    this.editVien = function (data) {
        return $http({
            method: 'POST',
            url: '/CTDTKeHoach/editGV',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }

    this.saveEmail = function (data) {
        return $http({
            method: 'POST',
            url: '/CTDTKeHoach/saveEmail',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }

    this.sendEmail = function (id) {
        return $http.post("/CTDTKeHoach/EditsendEmail/" + id);
    }

    this.chEmail = function (data) {
        return $http({
            method: 'POST',
            url: '/CTDTKeHoach/EditshowEmail',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
});