app.service("NotificationService", function ($http) {
    this.getMail = function () {
        return $http.post("/Mail/loadMail");
    }

    this.seen = function (id) {
        return $http.post("/Mail/getSeen/" + id);
    }

    this.accept = function (id) {
        return $http.post("/Mail/Accept/" + id);
    }

    this.decline = function (id) {
        return $http.post("/Mail/Decline/" + id);
    }
});