(() => {
    "use strict";
    angular
        .module("app")
        .controller("homeController", homeController);

    homeController.$inject = ["$location"];

    function homeController($location) {
        const vm = this;
        vm.title = "homeController";

        activate();

        function activate() {

        }
    }
})();
