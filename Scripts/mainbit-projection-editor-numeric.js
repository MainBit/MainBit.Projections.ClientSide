jQuery(function ($) {
    function displayNumericEditorOptions() {
        $("#operator-min option:selected").each(function () {
            var val = $(this).val();
            switch (val) {
                case 'Ignored':
                    $('#fieldset-min').hide();
                    $('#fieldset-max, #fieldset-op-max').show();
                    break;
                case 'Equals':
                    $('#fieldset-min').show();
                    $('#fieldset-max, #fieldset-op-max').hide();
                    break;
                default:
                    $('#fieldset-min, #fieldset-max, #fieldset-op-max').show();
                    break;
        
            }
        });
        $("#operator-max option:selected").each(function () {
            var val = $(this).val();
            switch (val) {
                case 'Ignored':
                    $('#fieldset-max').hide();
                    break;
                default:
                    $('#fieldset-max').show();
                    break;
            }
        });
    }

    displayNumericEditorOptions();
    $('#operator-min, #operator-max').on('change', displayNumericEditorOptions);
});