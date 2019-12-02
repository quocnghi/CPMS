app.service("MatranService", function ($http) {
    this.getKHDTlist = function () {
        return $http.post("/MaTran/loadKHDTList/");
    }
    this.getreview = function (id) {
        return $http.post("/MaTran/loadReviewInfo/" + id);
    }
});