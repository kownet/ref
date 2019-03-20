APP.cities = (function () {

    var getAll = function (opts) {

        $.get(opts.url)
            .done(function (data) {
                console.log(data);
            });

    };

    return {
        getAll: function (opts) {
            getAll(opts);
        }
    };

})();