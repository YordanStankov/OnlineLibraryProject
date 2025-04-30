function BanUser(banId) {

    if (confirm("Are you sure you to ban the user")) {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        fetch(`/Admin/BanUser?banId=${banId}`, {

            method: "POST",
            headers: {
                "RequestVerificationToken": token
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.succes) {
                    alert("User has been banned");
                    window.location.href = data.redirectUrl;
                }
                else {
                    alert("Error: " + data.message)
                }
            })
            .catch(error => console.error("Error: ", error))
    }
}
function UnbanUser(unbanId) {

    if (confirm("Are you sure you to unban the user")) {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        fetch(`/Admin/UnbanUser?unbanId=${unbanId}`, {

            method: "POST",
            headers: {
                "RequestVerificationToken": token
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.succes) {
                    alert("User has been banned");
                    window.location.href = data.redirectUrl;
                }
                else {
                    alert("Error: " + data.message)
                }
            })
            .catch(error => console.error("Error: ", error))
    }
}
