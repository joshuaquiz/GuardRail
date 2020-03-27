"use strict";

app.controller(
    "homeController",
    [
        "$rootScope",
        "$scope",
        "$http",
        function ($rootScope, $scope, $http) {
            $rootScope.$on(
                "NewLog",
                (e, p) => {
                    console.log(e, p);
                    $scope.logs.push(p);
                });
            const setup = () => {
                $http.get("/api/logs/latest")
                    .then(response => {
                        $scope.logs = response.data;
                    });
            };
            setup();
        }
    ]);