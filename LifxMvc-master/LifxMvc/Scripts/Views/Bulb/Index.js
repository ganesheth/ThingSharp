var lifxMvcApp = angular.module("lifxMvcApp", ['angularSpectrumColorpicker']);

var indexController = lifxMvcApp.controller("bulbIndexController", ['$scope', '$window', '$log', '$http', function ($scope, $window, $log, $http) {

	$scope.isBusy = false;
	//Server methods
	$scope.getModel = function () {
		$http.get('/Bulb/IndexJson')
			.success(function (result) {
				$scope.isBusy = false;
				$scope.groups = result.Groups;
				var arr = jQuery.makeArray()
				for (var i = 0; i < $scope.groups.length; ++i) {
					var group = $scope.groups[i];
					group.isCollapsed = false;
				}
			})
			.error(function (data, status) {
				$log.error(data);
			})
	};

	$scope.getPalette = function () {
		$http.get('/Bulb/GetKelvinPalette')
			.success(function (result) {
				$scope.palette = result;
			})
			.error(function (data, status) {
				$log.error(data);
			})
	};

	$scope.discoverBulbs = function () {
		$scope.isBusy = true;
		$http.post('/Bulb/DiscoverJson')
			.success(function (result) {
				$scope.groups = result.Groups;
				$scope.isBusy = false;
			})
			.error(function (data, status) {
				$log.error(data);
				$scope.isBusy = false;
			});
	};

	$scope.togglePowerAll = function () {
		$http.post('/Bulb/TogglePowerAllJson')
			.success(function (result) {
				$scope.groups = result.Groups;
			})
			.error(function (data, status) {
				$log.error(data);
			});
	};

	$scope.togglePowerGroup = function (group) {
		$http.post('/Bulb/TogglePowerGroupJson', { name: group.Name })
			.success(function (result) {
				$scope.groups = result.Groups;
			})
			.error(function (data, status) {
				$log.error(data);
			});
	};

	$scope.togglePowerBulb = function (bulbId) {
		$http.post('/Bulb/TogglePowerBulbJson', { bulbId: bulbId })
			.success(function (result) {
				$scope.groups = result.Groups;
			})
			.error(function (data, status) {
				$log.error(data);
			});
	};

	$scope.setColorBulb = function (bulbId, color) {
		$http.post('/Bulb/SetColorBulbJson', { bulbId: bulbId, color: color })
			.success(function (result) {
			})
			.error(function (data, status) {
				$log.error(data);
			});
	};

	//Client methods
	$scope.bulbColorChanged = function (bulb) {
		$scope.setColorBulb(bulb.BulbId, bulb.ColorString);
	};

	$scope.areAnyOn = function () {

		var result = false;
		var arr = jQuery.makeArray($scope.groups)
		result = arr.some($scope.isGroupOn);
		return result;
	};

	$scope.isGroupOn = function (group) {
		var arr = jQuery.makeArray(group.Bulbs)
		var result = arr.some(isBulbOn);
		return result;
	};

	function isBulbOn(bulb) {
		return bulb.IsOn;
	}

	$scope.getButtonBackground = function (color) {
		var result = { 'background-color' : color };
		return result;
	};


	$scope.isBusy = true;
	$scope.getPalette();
	$scope.getModel();

}]);


