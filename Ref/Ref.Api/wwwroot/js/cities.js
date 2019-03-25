APP.cities = (function () {

    var getAll = function (opts) {

        $.get(opts.url)
            .done(function (data) {

                $.each(data, function (i, item) {
                    var rows = "<option value='" + item.id + "'>" + item.name + "</option>";
                    $(opts.cntAdd).append(rows);
                    $(opts.cntEdit).append(rows);
                });

            });

    };

    return {
        getAll: function (opts) {
            getAll(opts);
        }
    };

})();