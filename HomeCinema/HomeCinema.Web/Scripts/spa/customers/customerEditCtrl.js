(function (app) {
    'use strict';

    app.controller('customerEditCtrl', customerEditCtrl);

    customerEditCtrl.$inject = ['$scope', '$uibModalInstance','$timeout', 'apiService', 'notificationService'];

    function customerEditCtrl($scope, $uibModalInstance, $timeout, apiService, notificationService) {
        $scope.cancelEdit = cancelEdit;
        $scope.updateCustomer = updateCustomer;

        $scope.openDatePicker = openDatePicker;
        $scope.dateOptions = { formatYear: 'yy', startingDay: 1 };
        $scope.datepicker = {};
        $scope.inputFormats = ['yyyy-MM-dd', 'yyyy-MM-dd HH:mm:ss', 'yyyy-MM-ddTHH:mm:ss', 'EEE MMM dd yyyy', 'yyyy-MMMM-dd', 'dd-MMMM-yyyy'];
        $scope.format = $scope.inputFormats[0];

        function updateCustomer() {
            console.log($scope.EditedCustomer);
            apiService.post('/api/customers/update/', $scope.EditedCustomer, updateCustomerCompleted, updateCustomerLoadFailed);
        }

        function updateCustomerCompleted(response) {
            notificationService.displaySuccess($scope.EditedCustomer.FirstName + ' ' + $scope.EditedCustomer.LastName + ' has been updated');
            $scope.EditedCustomer = {};
            $uibModalInstance.dismiss();
        }

        function updateCustomerLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function cancelEdit() {
            $scope.isEnabled = false;
            $uibModalInstance.dismiss();
        }

        function openDatePicker($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $timeout(function () {
                $scope.datepicker.opened = true;
            });

            $timeout(function () {
                $('.uib-datepicker-popup.dropdown-menu').css('z-index', '10000');
            }, 100);
        };
    }

})(angular.module('homeCinema'));
