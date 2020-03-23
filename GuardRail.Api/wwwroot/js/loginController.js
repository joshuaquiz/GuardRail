"use strict";

app.controller(
    "loginController",
    [
        "$scope",
        "$location",
        "authenticationService",
        function ($scope, $location, authenticationService) {
            authenticationService.ClearCredentials();
            $scope.login = () => {
                $scope.dataLoading = true;
                authenticationService.Login(
                    $scope.username,
                    $scope.password,
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