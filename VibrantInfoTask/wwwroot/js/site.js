
$('#ConfirmPassowrd').on("blur", function () {
    let Password = $('#Password').val();
    let ConfirmPassowrd = $('#ConfirmPassowrd').val();
    if (Password != ConfirmPassowrd) {
        $("#ConfirmPasswordErrorMessage").text("Password and Confirm password are not matched.");
    }
    else {
        $("#ConfirmPasswordErrorMessage").text("");
    }
});