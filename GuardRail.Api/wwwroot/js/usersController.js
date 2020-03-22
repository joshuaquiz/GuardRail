"use strict";

app.controller(
    "usersController",
    [
        "$rootScope",
        "$http",
        ($rootScope, $http) => {
            $rootScope.$on("message", d => console.log(d));
            const setup = () => {
                $http.get("/api/users")
                    .then(response => {
                        console.log(response);
                    });
            };
            setup();
        }
    ]);