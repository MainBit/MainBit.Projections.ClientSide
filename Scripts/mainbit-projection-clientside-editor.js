jQuery(function ($) {
    function displayClientSideEditorOptions() {
        var checked = $('#client-side-switcher').is(":checked");
        var clientSideOptions = $('#client-side-switcher').closest('form').find('.client-side-options');
        if (checked) {
            clientSideOptions.show();
        }
        else {
            clientSideOptions.hide();
        }
    }

    displayClientSideEditorOptions();
    $('#client-side-switcher').change(displayClientSideEditorOptions);

    var clienSideNameEl = $('#client-side-name');
    var relatedValues = $('.client-side-related-elements').data('elements');
    if (relatedValues) {
        var relatedSelectors = jQuery.map(relatedValues, function (value, key) { return '[name="' + key + '"]'; }).toString();
        var relatedEls = $(relatedSelectors);

        clienSideNameEl.on('input', function () {
            var curName = clienSideNameEl.val();
            relatedEls.each(function (key, el) {
                var name = $(el).attr('name');
                var relatedValue = relatedValues[name].format(curName);
                $(this).val('{' + relatedValue + '}');
            })
        });
    }

    //http://stackoverflow.com/questions/1038746/equivalent-of-string-format-in-jquery
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/\{\{|\}\}|\{(\d+)\}/g, function (m, n) {
            if (m == "{{") { return "{"; }
            if (m == "}}") { return "}"; }
            return args[n];
        });
    };
});
