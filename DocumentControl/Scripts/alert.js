function alertNotification(title, detail, icon) {
    Swal.fire(
        title,
        detail,
        icon
    )
}

function alertNotificationAndRedirect(title, detail, icon, LinkPage) {
    Swal.fire(
        title,
        detail,
        icon
    ).then(function () {
        window.location = LinkPage;
    });
}

// ลิงค์นี้ช่วยชีวิต
// https://stackoverflow.com/questions/44729434/using-sweetalert2-to-replace-return-confirm-on-an-asp-net-button
function alertConfirm(btn, title, text, icon, confirmText, cancelText) {
    if (btn.dataset.confirmed) {
        btn.dataset.confirmed = false;
        return true;
    } else {
        event.preventDefault();
        Swal.fire({
            title: title,
            text: text,
            icon: icon,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: confirmText,
            cancelButtonText: cancelText
        }).then(function (result) {
            if (result.isConfirmed) {
                btn.dataset.confirmed = true;
                btn.click();
            }
        })
    }
}

function alertToast(text, icon) {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    })

    Toast.fire({
        icon: icon,
        title: text
    })
}
