$(document).on('click', '.favourite-toggle', function (e) {
    e.preventDefault();
    var $link = $(this);
    var authorId = $link.data('author-id');
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/Authors/FavouriteAuthor',
        type: 'POST',
        data: { authorId: authorId },
        headers: {
            'RequestVerificationToken': token
        },
        success: function (response) {
            if (response.success) {
                $link.toggleClass('favorited');
                $link.text($link.hasClass('favorited') ? '★' : '☆');
            } else {
                alert("Something went wrong.");
            }
        },
        error: function () {
            alert("Server error.");
        }
    });
});