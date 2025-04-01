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

document.addEventListener("DOMContentLoaded", function () {
    let modal = document.getElementById("editGenreModal");

    if (modal) { // Only run if the modal exists
        function loadEditGenre(genreId) {
            modal.style.display = "block";

            // Load the partial inside the modal dynamically
            fetch(`/Genre/EditGenre?genreEditId=${genreId}`)
                .then(response => response.text())
                .then(html => {
                    document.getElementById("modalBody").innerHTML = html;
                })
                .catch(error => console.error("Error loading genre edit:", error));
        }

        function closeModal() {
            modal.style.display = "none";
        }

        window.onclick = function (event) {
            if (event.target === modal) {
                closeModal();
            }
        };

        // Handle form submission via AJAX
        document.addEventListener("submit", function (event) {
            if (event.target.id === "editGenreForm") {
                event.preventDefault(); // Prevent default full-page refresh

                let form = event.target;
                let formData = new FormData(form);

                fetch(form.action, {
                    method: "POST",
                    body: formData
                })
                    .then(response => {
                        if (response.ok) {
                            closeModal(); // Close the modal after successful submission
                            location.reload(); // Refresh the page to show the updated genre list
                        }
                    })
                    .catch(error => console.error("Error submitting genre edit:", error));
            }
        });

        // Expose functions globally so they can be used in Razor views
        window.loadEditGenre = loadEditGenre;
        window.closeModal = closeModal;
    }
});

