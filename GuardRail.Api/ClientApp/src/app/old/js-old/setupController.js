"use strict";

app.controller(
    "loginController",
    [
        "$scope",
        "$location",
        "$http",
        "authenticationService",
        function ($scope, $location, $http, authenticationService) {
            $scope.createAccount = () => {
                $scope.dataLoading = true;
                $http.post(
                        "/api/setup",
                        {
                            username: $scope.username,
                            password: $scope.password,
                            firstName: $scope.firstName,
                            lastName: $scope.lastName
                        })
                    .then(
                        response => {
                            if (response.data) {
                                authenticationService.SetCredentials(
                                    $scope.username,
                                    $scope.password);
                                $location.path("/home");
                            } else {
                                $scope.error = response.message;
                                $scope.dataLoading = false;
                            }
                        });
            };
        }
    ]);