"use strict";

app.controller(
    "devicesController",
    [
        "$rootScope",
        "$scope",
        "$http",
        function ($rootScope, $scope, $http) {
            const setup = () => {
                $http.get("/api/devices")
                    .then(response => {
                        $scope.devices = response.data;
                    });
            };
            $scope.edit = device => {
                $scope.state = "edit";
                $http.get(
                    `/api/devices/${device.id}`)
                    .then(response => {
                        $scope.device = response.data;
                        $http.get("/api/users")
                            .then(users => {
                                $scope.possibleUsers = {
                                    options: $scope.device.user === null
                                        ? users.data
                                        : users.data.filter(
                                            x => x.id !== $scope.device.user.id),
                                    value: {}
                                };
                            });
                    });
            };
            $scope.save = device => {
                device.user = $scope.possibleUsers.value;
                $http.post(
                    `/api/devices/${device.id}`,
                    device)
                    .then(() => {
                        $scope.state = "none";
                        setup();
                    });
            };
            setup();
        }
    ]);