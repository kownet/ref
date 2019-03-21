APP.notificationtypes = (function () {

    var getAll = function (opts) {

        $.get(opts.url)
            .done(function (data) {
                
            });

    };

    return {
        getAll: function (opts) {
            getAll(opts);
        }
    };

})();