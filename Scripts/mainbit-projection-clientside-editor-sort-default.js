jQuery(function ($) {
    function displayClientSideEditorOptions() {
        var checked = $('#client-side-switcher').is(":checked");
        if (checked) {
            $('#sortundefined').val('None');
            $('#sortundefined').change();
        }
    }

    displayClientSideEditorOptions();
    $('#client-side-switcher').change(displayClientSideEditorOptions);
});