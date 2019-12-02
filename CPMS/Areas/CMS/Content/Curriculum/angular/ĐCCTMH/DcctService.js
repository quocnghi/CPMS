app.service("DcctService", function ($http) {
    this.getInfo = function () {
        return $http.post("/CourseSyllabus/loadInfo/");
    }

    this.getReviewInfo = function (id) {
        return $http.post("/CourseSyllabus/loadReviewInfo/" + id);
    }

    this.getKHDTlist = function () {
        return $http.post("/CourseSyllabus/loadKHDTList/" );
    }

    this.addCdr = function (data) {
        return $http({
            method: 'POST',
            url: '/CourseSyllabus/addCdr',
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: 'json'
        });
    }
    this.getCdr = function (data) {
        return $http({
            method: 'POST',
            url: '/CourseSyllabus/getCdr',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.editCdr = function (data) {
        return $http({
            method: 'POST',
            url: '/CourseSyllabus/editCdr',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.deleteCdr = function (id) {
        return $http.post("/CourseSyllabus/deleteCdr/" + id);
    }

    this.addND = function (data) {
        return $http({
            method: 'POST',
            url: '/CourseSyllabus/addND',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.getND = function (data) {
        return $http({
            method: 'POST',
            url: '/CourseSyllabus/getND',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.editND = function (data) {
        return $http({
            method: 'POST',
            url: '/CourseSyllabus/editND',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }

    this.deleteND = function (id) {
        return $http.post("/CourseSyllabus/deleteND/" + id);
    }

    this.getreview = function (data) {
        return $http({
            method: 'POST',
            url: '/CourseSyllabus/getreview',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }

    this.addTailieu = function (data) {
        return $http({
            method: 'POST',
            url: '/CourseSyllabus/addTailieu',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.getTailieu = function (data) {
        return $http({
            method: 'POST',
            url: '/CourseSyllabus/getTailieu',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.editTailieu = function (data) {
        return $http({
            method: 'POST',
            url: '/CourseSyllabus/editTailieu',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
    this.deleteTailieu = function (id) {
        return $http.post("/CourseSyllabus/deleteTailieu/" + id);
    }

    this.hoanthanh = function (data) {
        return $http({
            method: 'POST',
            url: '/CourseSyllabus/hoanthanh',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }

    this.luunhap = function (data) {
        return $http({
            method: 'POST',
            url: '/CourseSyllabus/luunhap',
            data: JSON.stringify(data),
            dataType: 'json'
        });
    }
});