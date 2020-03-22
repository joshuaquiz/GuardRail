"use strict";

app.controller(
    "homeController",
    [
        "$rootScope",
        "$scope",
        "$http",
        ($rootScope, $scope, $http) => {
            $rootScope.$on("message", d => console.log(d));
            const setup = () => {
                $http.get("/api/logs/latest")
                    .then(response => {
                        $scope.logs = response.data;
                    });
            };
            setup();
        }
    ]);