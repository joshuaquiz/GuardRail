"use strict";

app.controller(
    "doorsController",
    [
        "$rootScope",
        "$scope",
        "$http",
        function ($rootScope, $scope, $http) {
            $rootScope.$on("message", d => console.log(d));
            const setup = () => {
                $http.get("/api/doors")
                    .then(response => {
                        $scope.doors = response.data;
                    });
            };
            $scope.lockDoor = door => {
                $http.post(
                    `/api/doors/${door.id}/lock`)
                    .then(() => {
                        door.lockedStatus = 2;
                        alert("locked!");
                    });
            };
            $scope.unlockDoor = door => {
                $http.post(
                    `/api/doors/${door.id}/unlock`)
                    .then(() => {
                        door.lockedStatus = 1;
                        alert("unlocked!");
                    });
            };
            setup();
        }
    ]);