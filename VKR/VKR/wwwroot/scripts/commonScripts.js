function showPopupMessage(message, type) {
    swal({
        position: 'top-end',
        icon: type,
        title: message,
        showConfirmButton: false,
        timer: 1500
    })
}

function sendRequest(url, method, data, dataType, onSuccess, onError, currentUrl) {
    $(".loading-spinner").show();
    $.ajax({
        url: url,
        type: method,
        data: data,
        dataType: dataType,
        contentType: "application/json; charset=utf-8",
        success: onSuccess,
        error: onError,
        statusCode: {
            401: function () {
                $("#details").empty();
                redirectionToLoginPage(currentUrl);
            }
        }
    });
    $(".loading-spinner").hide();
}