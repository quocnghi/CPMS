app.service("NotificationService", function ($http) {
    this.getMail = function () {
        return $http.post("/CMS/Mail/loadMail/");
    }

    this.seen = function (id) {
        return $http.post("/CMS/Mail/getSeen/" + id);
    }

    this.accept = function (id) {
        return $http.post("/CMS/Mail/Accept/" + id);
    }

    this.decline = function (id) {
        return $http.post("/CMS/Mail/Decline/" + id);
    }
});