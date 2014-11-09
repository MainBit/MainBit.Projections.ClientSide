$(function () {

    var filterForm = $('.client-side-filters');
    var fastSearchUrl = filterForm.data('fast-search-url');
    var fastSearchResult = filterForm.find('.fast-search-result');

    function GetQueryString() {
        var queryString = $.map(filters, function (item, key) { return item.value ? key + '=' + item.value : null }).join('&');
        queryString = 'search=1' + (queryString ? ('&' + queryString) : '');

        var sort = getQueryStringParameterByName("sort");
        if (sort) {
            queryString += '&sort=' + sort;
        }

        return queryString;
    }

    // http://stackoverflow.com/questions/901115/how-can-i-get-query-string-values-in-javascript
    function getQueryStringParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    function DoFastSearch(top) {
        if (fastSearchUrl) {
            var filtersQueryString = GetQueryString();
            $.ajax({
                url: fastSearchUrl,
                dataType: 'json',
                data: filtersQueryString,
                beforeSend: function (data) {
                    fastSearchResult.addClass('process');
                    fastSearchResult.removeClass('complete success error');
                    fastSearchResult.css('top', top + 'px');
                },
                success: function (data) {
                    fastSearchResult.find('.count').html(data.count);
                    fastSearchResult.find('.link').attr('href', data.url);
                    fastSearchResult.addClass('complete success');
                },
                error: function (data) {
                    fastSearchResult.addClass('complete error');
                    alert(data.statusText);
                },
                complete: function (data) {
                    fastSearchResult.removeClass('process');
                }
            });
        }
    }

    filterForm.find('.submit').click(function () {
        var resultUrl = filterForm.attr('action');
        var filtersQueryStirng = GetQueryString();
        var test = ConcatQueryStringToUrl(resultUrl, filtersQueryStirng);
        window.location.href = test;
    });

    function ConcatQueryStringToUrl(url, querySring) {
        return querySring
            ? (url + (url.indexOf('?') ? '&' : '?') + querySring)
            : url
    }

    var stoppedTyping;
    filterForm.find(':input').on('input change', function () {
        var _this = $(this);

        var filterName = $(this).attr('name');
        var filter = filters[filterName];
        var filterControls = filterForm.find(':input[name=' + filterName + ']').sort(function (a, b) { return $(a).data('position') - $(b).data('position'); });
        var filterControlsValues = filterControls.serializeArray().map(function (item) { return item.value; });

        switch (filter.type) {
            case 'numeric':
                filter.value = filterControlsValues.map(function (item) { return item.replace(",", "."); }).join('_');
                break;
            default:
                filter.value = filterControlsValues.join('-');
                break;
        }

        // is there already a timer? clear if if there is
        if (stoppedTyping) clearTimeout(stoppedTyping);
        // set a new timer to execute 3 seconds from last keypress
        stoppedTyping = setTimeout(function () {
            // code to trigger once timeout has elapsed
            var top = _this.offset().top - filterForm.offset().top;
            DoFastSearch(top);
        }, 200);
    });

    filterForm.find('.client-side-filter.numeric').each(function () {

        var _this = $(this);
        var slider = _this.find('.slider');
        var from = _this.find('.value-container.from .value');
        var to = _this.find('.value-container.to .value');
        var minValue = parseFloat(from.attr('min'));
        var maxValue = parseFloat(to.attr('max'));

        if (minValue && maxValue) {

            _this.addClass('with-slider');

            slider.slider({
                range: true,
                min: minValue,
                max: maxValue,
                step: parseFloat(from.attr('step')),
                values: [from.val() != "" ? parseFloat(from.val()) : minValue, to.val() != "" ? parseFloat(to.val()) : maxValue],
                create: function (event, ui) {
                    $(this).find(".ui-slider-handle:first").addClass("left");
                },
                slide: function (event, ui) {
                    if ($(ui.handle).hasClass("left")) {
                        from.val(ui.values[0]);
                    }
                    else {
                        to.val(ui.values[1]);
                    }
                },
                stop: function (event, ui) {
                    if ($(ui.handle).hasClass("left")) {
                        from.change();
                    }
                    else {
                        to.change();
                    }
                }
            });

            from.on('input', function () {
                var value = $(this).val();
                slider.slider("values", 0, value == "" ? minValue : value);
            });

            to.on('input', function () {
                var value = $(this).val();
                slider.slider("values", 1, value == "" ? maxValue : value);
            });
        }
    });
});

$(function () {
    $('.client-side-filter .b-more-less').each(function () {
        var _this = $(this);
        _this.find('.b-more-less__link').click(function () {
            var item = _this.find('.b-more-less__item'),
                link = $(this);

            var otherLinkText = link.data("other-text");
            link.data("other-text", link.html());
            link.html(otherLinkText);

            _this.add(link).add(item).toggleClass('more less');
            item.slideToggle();
        });
    });

    $('.client-side-filter.collapse .title, .client-side-filter.expand .title').click(function () {
        var filter = $(this).closest('.client-side-filter');
        filter.toggleClass('collapse expand');
        filter.find('.content').slideToggle();
    });
});