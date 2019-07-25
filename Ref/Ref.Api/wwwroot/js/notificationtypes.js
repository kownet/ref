APP.notificationtypes = (function () {

    var getAll = function (opts) {

        $.get(opts.url)
            .done(function (data) {

                $.each(data, function (i, item) {
                    var rows = "<option value='" + item.id + "'>" + item.namePl + "</option>";
                    $(opts.cntAdd).append(rows);
                });

                if (opts.selectedNotification !== null) {
                    $(opts.cntAdd).val(opts.selectedNotification);
                }

            });

    };

    return {
        getAll: function (opts) {
            getAll(opts);
        }
    };

})();