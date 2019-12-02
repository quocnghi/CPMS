app.service("ReviewService", function ($http) {
    this.getKHDTlist = function () {
        return $http.post("/MaTran/loadKHDTList/");
    }
    this.getreview = function (id) {
        return $http.post("/CTDTKhung/loadReviewInfo/" + id);
    }

});