"use strict";

app.controller(
    "usersController",
    [
        "$rootScope",
        "$scope",
        "$http",
        function ($rootScope, $scope, $http) {
            $rootScope.$on("message", d => console.log(d));
            const setup = () => {
                $http.get("/api/users")
                    .then(response => {
                        $scope.users = response.data;
                    });
            };
            setup();
        }
    ]);