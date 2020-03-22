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
    .factory(
        "base64",
        () => {
            var keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
            return {
                encode: input => {
                    var output = "";
                    var chr1, chr2, chr3;
                    var enc1, enc2, enc3, enc4;
                    var i = 0;
                    do {
                        chr1 = input.charCodeAt(i++);
                        chr2 = input.charCodeAt(i++);
                        chr3 = input.charCodeAt(i++);

                        enc1 = chr1 >> 2;
                        enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                        enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                        enc4 = chr3 & 63;

                        if (isNaN(chr2)) {
                            enc3 = enc4 = 64;
                        } else if (isNaN(chr3)) {
                            enc4 = 64;
                        }

                        output = output +
                            keyStr.charAt(enc1) +
                            keyStr.charAt(enc2) +
                            keyStr.charAt(enc3) +
                            keyStr.charAt(enc4);
                        chr1 = chr2 = chr3 = "";
                        enc1 = enc2 = enc3 = enc4 = "";
                    } while (i < input.length);
                    return output;
                },
                decode: input => {
                    var output = "";
                    var chr1, chr2, chr3;
                    var enc1, enc2, enc3, enc4;
                    var i = 0;
                    // remove all characters that are not A-Z, a-z, 0-9, +, /, or =
                    var base64Test = /[^A-Za-z0-9+/=]/g;
                    if (base64Test.exec(input)) {
                        window.alert("There were invalid base64 characters in the input text.\n" +
                            "Valid base64 characters are A-Z, a-z, 0-9, '+', '/',and '='\n" +
                            "Expect errors in decoding.");
                    }
                    input = input.replace(/[^A-Za-z0-9+/=]/g, "");
                    do {
                        enc1 = keyStr.indexOf(input.charAt(i++));
                        enc2 = keyStr.indexOf(input.charAt(i++));
                        enc3 = keyStr.indexOf(input.charAt(i++));
                        enc4 = keyStr.indexOf(input.charAt(i++));

                        chr1 = (enc1 << 2) | (enc2 >> 4);
                        chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                        chr3 = ((enc3 & 3) << 6) | enc4;

                        output = output + String.fromCharCode(chr1);

                        if (enc3 !== 64) {
                            output = output + String.fromCharCode(chr2);
                        }
                        if (enc4 !== 64) {
                            output = output + String.fromCharCode(chr3);
                        }

                        chr1 = chr2 = chr3 = "";
                        enc1 = enc2 = enc3 = enc4 = "";

                    } while (i < input.length);
                    return output;
                }
            };
        })
    .factory(
        "authenticationService",
        [
            "base64",
            "$http",
            "$rootScope",
            function (base64, $http, $rootScope) {
                var service = {
                    TryConfigure: (pass, fail) => {
                        var value = sessionStorage.getItem("globals");
                        if (value) {
                            const authData = base64.decode(JSON.parse(value).currentUser.authData).split(":");
                            service.SetCredentials(authData[0], authData[1]);
                            pass();
                        } else {
                            fail();
                        }
                    },
                    Login: (username, password, callback) =>
                        $http.post(
                            "/api/login",
                            {
                                username: username,
                                password: password
                            })
                        .then(callback),
                    SetCredentials: (username, password) => {
                        var authData = base64.encode(`${username}:${password}`);
                        $rootScope.globals = {
                            currentUser: {
                                username: username,
                                authData: authData
                            }
                        };
                        $http.defaults.headers.common["Authorization"] = `Basic ${authData}`;
                        sessionStorage.setItem("globals", JSON.stringify($rootScope.globals));
                    },
                    ClearCredentials: () => {
                        $rootScope.globals = {};
                        sessionStorage.removeItem("globals");
                        $http.defaults.headers.common.Authorization = "Basic ";
                    }
                };
                return service;
            }])
    .config(
        [
            "$routeProvider",
            $routeProvider => {
                $routeProvider
                    .when(
                        "/login",
                        {
                            templateUrl: "/pages/login.html",
                            controller: "loginController"
                        })
                    .when(
                        "/home",
                        {
                            templateUrl: "/pages/home.html",
                            controller: "homeController"
                        })
                    .when(
                        "/users",
                        {
                            templateUrl: "/pages/users.html",
                            controller: "usersController"
                        })
                    .otherwise(
                        {
                            redirectTo: "/home"
                        });
            }
        ])
    .controller(
        "mainController",
        [
            "$rootScope",
            "$scope",
            "$location",
            "connection",
            "authenticationService",
            ($rootScope, $scope, $location, connection, authenticationService) => {
                connection();
                $rootScope.$on("$locationChangeStart", (event, next, current) => {
                    $scope.showMenus = $location.path() !== "/login";
                    if ($location.path() !== "/login" && $rootScope.globals === undefined && $rootScope.globals.currentUser === undefined) {
                        $location.path("/login");
                    }
                });
                authenticationService.TryConfigure(
                    () => {
                        $location.path("/home");
                    },
                    () => {
                        $location.path("/login");
                    });
            }
        ]);