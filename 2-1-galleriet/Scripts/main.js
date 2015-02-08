
window.onload = function () {

    // Scroll to active thumbnail if there is one.
    $("#ThumbnailContainer .content").has("a.active").scrollTo($("a.active"), { offset: -200 });

    // Add closebutton to messageboxes
    $(".success-message, .error-message").each(function (element) {
        $(this).append('<a class="close" href="#">&#215;</a>').click(function () {
            $(this).fadeOut();
        });
    })
}


