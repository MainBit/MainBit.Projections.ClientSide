jQuery(function ($) {
    function displayClientSideEditorOptions() {
        var checked = $('#client-side-switcher').is(":checked");
        if (checked) {
            $('#operator-min').val('GreaterThanEquals');
            $('#operator-max').val('LessThanEquals');
            $('#operator-min, #operator-max').change();
        }
    }

    displayClientSideEditorOptions();
    $('#client-side-switcher').change(displayClientSideEditorOptions);
});