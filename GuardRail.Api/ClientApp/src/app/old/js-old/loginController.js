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
                        if (response.data === "true") {
                            authenticationService.SetCredentials(
                                $scope.username,
                                $scope.password);
                            $location.path("/home");
                        } else if (response.data === "false") {
                            $scope.error = response.message;
                            $scope.dataLoading = false;
                        } else if (response.data === "none") {
                            $location.path("/setup");
                        }
                    });
            };
        }
    ]);