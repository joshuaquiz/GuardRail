"use strict";

app.controller(
    "loginController",
    [
        "$scope",
        "$rootScope",
        "$location",
        "authenticationService",
        function ($scope, $rootScope, $location, authenticationService) {
            $rootScope.hideMenus = $routeParams.hideMenus;
            authenticationService.ClearCredentials();
            $scope.login = () => {
                $scope.dataLoading = true;
                authenticationService.Login(
                    $scope.email,
                    $scope.password,
                    response => {
                        if (response.success) {
                            authenticationService.SetCredentials(
                                $scope.email,
                                $scope.password);
                            $location.path("/");
                        } else {
                            $scope.error = response.message;
                            $scope.dataLoading = false;
                        }
                    });
            };
        }
    ]);