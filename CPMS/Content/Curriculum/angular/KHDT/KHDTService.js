app.service("KHDTService", function ($http) {

    this.getKHDTlist = function () {
        return $http.post("/CourseSyllabus/loadKHDTList/");
    }
    this.getreview = function (id) {
        return $http.post("/CourseSyllabus/loadReviewInfo/" + id);
    }
    this.Adddanhgia = function (data) {
        return $http({
            method: 'POST',
            url: '/CourseSyllabus/Adddanhgia',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }

});