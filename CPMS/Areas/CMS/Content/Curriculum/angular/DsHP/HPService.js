app.service("HPService", function ($http) {
    this.getds = function () {
        return $http.post("/Curriculum/LoadHpList/");
    }
    this.getdshp = function () {
        return $http.post("/MaTran/LoadHpList/");
    }
});