APP.propertytypes = (function () {

    var getAll = function (opts) {

        $.get(opts.url)
            .done(function (data) {

                $.each(data, function (i, item) {
                    var rows = "<option value='" + item.id + "'>" + item.namePl + "</option>";
                    $(opts.cnt).append(rows);
                });

                if (opts.selectedProperty !== null) {
                    $(opts.cnt).val(opts.selectedProperty);
                }

            });

    };

    return {
        getAll: function (opts) {
            getAll(opts);
        }
    };

})();