const signUpButton = document.getElementById('signUp');
const signInButton = document.getElementById('signIn');
const container = document.getElementById('container_signup_signin');

function signUpValidateForm() {
	x = document.forms["sign-up-form"]["username"].value;
	if (x == "") {
		//   alert("'Email' can not be empty!!");
		asAlertMsg({
			type: "error",
			title: "Bạn chưa nhập thông tin",
			message: "'Username' không được để trống!!",

			button: {
				title: "Đóng",
				bg: "Cancel Button"
			}
		});
		return false;
	}
	var x = document.forms["sign-up-form"]["ho_ten"].value;
	if (x == "") {
		//   alert("'Name' can not be empty!!");
		asAlertMsg({
			type: "error",
			title: "Bạn chưa nhập thông tin",
			message: "'Họ Tên' không được để trống!!",

			button: {
				title: "Đóng",
				bg: "Cancel Button"
			}
		});
		return false;
	}
	var x = document.forms["sign-up-form"]["dia_chi"].value;
	if (x == "") {
		//   alert("'Name' can not be empty!!");
		asAlertMsg({
			type: "error",
			title: "Bạn chưa nhập thông tin",
			message: "'Địa chỉ' không được để trống!!",

			button: {
				title: "Đóng",
				bg: "Cancel Button"
			}
		});
		return false;
	}
	var x = document.forms["sign-up-form"]["sdt"].value;
	if (x == "") {
		//   alert("'Name' can not be empty!!");
		asAlertMsg({
			type: "error",
			title: "Bạn chưa nhập thông tin",
			message: "'Số điện thoại' không được để trống!!",

			button: {
				title: "Đóng",
				bg: "Cancel Button"
			}
		});
		return false;
	}
	var x = document.forms["sign-up-form"]["ngay_sinh"].value;
	if (x == "") {
		//   alert("'Name' can not be empty!!");
		asAlertMsg({
			type: "error",
			title: "Bạn chưa nhập thông tin",
			message: "'Ngày sinh' không được để trống!!",

			button: {
				title: "Đóng",
				bg: "Cancel Button"
			}
		});
		return false;
	}
	var x = document.forms["sign-up-form"]["cmnd"].value;
	if (x == "") {
		//   alert("'Name' can not be empty!!");
		asAlertMsg({
			type: "error",
			title: "Bạn chưa nhập thông tin",
			message: "'CMND' không được để trống!!",

			button: {
				title: "Đóng",
				bg: "Cancel Button"
			}
		});
		return false;
	}
	x = document.forms["sign-up-form"]["email"].value;
	if (x == "") {
		//   alert("'Email' can not be empty!!");
		asAlertMsg({
			type: "error",
			title: "Bạn chưa nhập thông tin",
			message: "'E-mail' không được để trống!!",

			button: {
				title: "Đóng",
				bg: "Cancel Button"
			}
		});
		return false;
	}

	x = document.forms["sign-up-form"]["password"].value;
	if (x == "") {
		//   alert("'Password' can not be empty!!");
		asAlertMsg({
			type: "error",
			title: "Bạn chưa nhập thông tin",
			message: "'Password' không được để trống!!",

			button: {
				title: "Đóng",
				bg: "Cancel Button"
			}
		});
		return false;
	}
}

function signInValidateForm() {

	x = document.forms["sign-in-form"]["email"].value;
	if (x == "") {
		//   alert("'Email' can not be empty!!");
		asAlertMsg({
			type: "error",
			title: "Bạn chưa nhập thông tin",
			message: "'E-mail' không được để trống!!",

			button: {
				title: "Đóng",
				bg: "Cancel Button"
			}
		});
		return false;
	}
	x = document.forms["sign-in-form"]["matkhau"].value;
	if (x == "") {
		//   alert("'Password' can not be empty!!");
		asAlertMsg({
			type: "error",
			title: "Bạn chưa nhập thông tin",
			message: "'Password' không được để trống!!",

			button: {
				title: "Đóng",
				bg: "Cancel Button"
			}
		});
		return false;
	}
}

signUpButton.addEventListener('click', () => {
	container.classList.add("right-panel-active");
});
2
signInButton.addEventListener('click', () => {
	container.classList.remove("right-panel-active");
});