jQuery(function ($) {
    function displayClientSideEditorOptions() {
        var checked = $('#client-side-switcher').is(":checked");
        if (checked) {
            $('#operator').val('ContainsAny');
            $('#operator, #operator-max').change();
        }
    }

    displayClientSideEditorOptions();
    $('#client-side-switcher').change(displayClientSideEditorOptions);
});