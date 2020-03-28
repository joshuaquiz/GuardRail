"use strict";

app.controller(
    "accessController",
    [
        "$rootScope",
        "$scope",
        "$http",
        function ($rootScope, $scope, $http) {
            const setup = () => {
                $http.get("/api/acd")
                    .then(response => {
                        $scope.acds = response.data;
                    });
            };
            $scope.edit = acd => {
                $http.get(
                    `/api/acd/${acd.id}`)
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