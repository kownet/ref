APP.admininfos = (function () {

    var getAll = function (opts) {

        $.get(opts.url)
            .done(function (data) {

                $.each(data, function (i, item) {

                    var info =
                        "<p class=\"card-text\">"
                            + item.text +
                            "<br><small><i>" + item.dateAdded + "</i></small>" +
                        "</p>";

                    $(opts.cnt).append(info);
                });

            });

    };

    return {
        getAll: function (opts) {
            getAll(opts);
        }
    };

})();