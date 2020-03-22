"use strict";

var app = angular.module(
    "app",
    [
        "ngRoute"
    ])
    .factory(
        "connection",
        [
            "$rootScope",
            $rootScope => {
                var connection = new signalR.HubConnectionBuilder().withUrl("/guardRailHub").build();
                connection.on("ReceiveMessage", params => $rootScope.$broadcast("message", params));
                connection.start().catch(err => console.error(err.toString()));
                return () => { };
            }
        ])
    .config(
        [
            "$routeProvider",
            $routeProvider => {
                $routeProvider
                    .when(
                        "/home",
                        {
                            templateUrl: "/pages/home.html",
                            controller: "homeController",
                            controllerAs: "home"
                        })
                    .when(
                        "/users",
                        {
                            templateUrl: "/pages/users.html",
                            controller: "usersController",
                            controllerAs: "users"
                        })
                    .otherwise(
                        {
                            templateUrl: "/pages/home.html",
                            controller: "homeController",
                            controllerAs: "home"
                        });
            }
        ])
    .controller(
        "mainController",
        [
            "$rootScope",
            "$scope",
            "$route",
            "$location",
            "$routeParams",
            "connection",
            ($rootScope, $scope, $route, $location, $routeParams, connection) => {
                connection();
                $rootScope.$on("message", d => console.log(d));
                $scope.$route = $route;
                $scope.$location = $location;
                $scope.$routeParams = $routeParams;
            }
        ]);