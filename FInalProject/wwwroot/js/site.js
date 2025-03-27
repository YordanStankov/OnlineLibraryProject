    function DeleteBook(bookId){
                    if (confirm("Are you sure you want to delete this")){
        fetch(`/BookOperations/DeleteBook?doomedId=${bookId}`,{
        method:"DELETE"
        })
            .then(response => response.json())
            .then(data => {
                if (data.succes) {
                    alert("Book has been deleted");
                    window.location.href = data.redirectUrl;
                }
                else {
                    alert("Womp womp didn't delete" + data.message)
                }
            })
            .catch(error => console.error("Error: ", error))
        }
    }

function BorrowBook(borrowedId) {
        fetch(`/BookOperations/BorrowBook?borrowedId=${borrowedId}`, {
            method: "POST"
        })
            .then(response => response.json())
            .then(data => {
                if (data.succes) {
                    alert("Book borrowed check your account");
                    window.location.href = data.redirectUrl;
                }
                else {
                    alert("Womp womp didn't borrow" + data.message)
                }
            })
            .catch(error => console.error("Error: ", error))
    }
