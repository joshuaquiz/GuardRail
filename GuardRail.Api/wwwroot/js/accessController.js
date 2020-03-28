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
                $scope.state = "edit";
                $http.get(
                        `/api/acd/${acd.id}`)
                    .then(response => {
                        $scope.device = response.data;
                        $http.get("/api/doors")
                            .then(doors => {
                                $scope.possibleDoors = {
                                    options: doors.data.filter(
                                        x => $scope.device.doors.indexOf(d => d.id === x.id) === -1),
                                    value: {}
                                };
                            });
                    });
            };
            $scope.addDoor = () => {
                if ($scope.possibleDoors.value === null) {
                    return;
                }
                $scope.device.doors.push($scope.possibleDoors.value);
                const index = $scope.possibleDoors.options.indexOf($scope.possibleDoors.value);
                $scope.possibleDoors.options.splice(index, 1);
            };
            $scope.save = acd => {
                $http.post(
                    `/api/acd/${acd.id}`,
                    acd)
                    .then(() => {
                        $scope.state = "none";
                        setup();
                    });
            };
            setup();
        }
    ]);