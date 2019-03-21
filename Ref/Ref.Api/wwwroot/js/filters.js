APP.filters = (function () {

    var getUserFilters = function (opts) {

        var params = JSON.stringify({ userId: opts.userId });

        $.ajax({
            contentType: 'application/json',
            type: 'POST',
            url: opts.url,
            data: params,
            success: function (data) {

                if (data.succeed) {

                    $.each(data.filters, function (i, item) {
                        var rows = "<tr>" +
                            "<td>" + item.name + "</td>" +
                            "<td>" + item.city + "</td>" +
                            "<td>" + item.flatAreaFrom + " - " + item.flatAreaTo + "</td>" +
                            "<td>" + item.priceFrom + " - " + item.priceTo + "</td>" +
                            "<td>" + item.notificationFormatted + "</td>" +
                            "</tr>";
                        $(opts.cntFiltersTable).append(rows);
                    });

                } else {
                    swal($.errorHeader, data.message, "error");
                }

            }
        });
    };

    return {
        getUserFilters: function (opts) {
            getUserFilters(opts);
        }
    };

})();