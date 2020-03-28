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
                    $scope.logs.splice(0, 0, p);
                    $scope.$apply();
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