$(document).ready(function () {
	var PowerCall = function (type, id, name, action) {
		var choiceTxt = "This action will " + action + " " + type + " " + name + "!  ";
		choiceTxt += "Do you wish to proceed?";
		var choice = confirm(choiceTxt);

		if (choice == true) {
			$("#" + type + "-" + action + "-" + id + "-form").submit();
		}
	}

	$(".instance-power-up").click(function (event) {
		PowerCall(
			"instance",
			$(event.target).data("id"),
			$(event.target).data("name"),
			"start"
		);
	});

	$(".instance-power-down").click(function (event) {
		PowerCall(
			"instance",
			$(event.target).data("id"),
			$(event.target).data("name"),
			"stop"
		);
	});

	$(".stack-power-up").click(function (event) {
		event.preventDefault();

		PowerCall(
			"stack",
			$(event.target).data("id"),
			$(event.target).data("name"),
			"start"
		);
	});

	$(".stack-power-down").click(function (event) {
		event.preventDefault();

		PowerCall(
			"stack",
			$(event.target).data("id"),
			$(event.target).data("name"),
			"stop"
		);
	});
});