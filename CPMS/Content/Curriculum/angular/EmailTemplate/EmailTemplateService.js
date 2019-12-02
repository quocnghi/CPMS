app.service("EmailTemplateService", function ($http) {
    this.getEmail = function () {
        return $http.post("/EmailTemplate/loadEmailList");
    }

    this.getDetailEmail = function (id) {
        return $http.post("/EmailTemplate/getDetailEmail/" + id);
    }

    this.addEmail = function (data) {
        return $http({
            method: 'POST',
            url: '/EmailTemplate/addEmail',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }

    this.editEmail = function (data) {
        return $http({
            method: 'POST',
            url: '/EmailTemplate/editEmail',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }

    this.deleteEmail = function (id) {
        return $http.post("/EmailTemplate/deleteEmail/" + id);
    }
});