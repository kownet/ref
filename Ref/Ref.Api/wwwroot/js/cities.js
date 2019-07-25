APP.cities = (function () {

    var getAll = function (opts) {

        var cities = getAllWithDistricts({ url: '/cities/withdistricts' });

        $.get(opts.url)
            .done(function (data) {

                $.each(data, function (i, item) {
                    var rows = "<option value='" + item.id + "'>" + item.nameFormatted + "</option>";
                    $(opts.cntAdd).append(rows);
                });

            });

        $(opts.cntAdd).change(function () {

            var selectedCity = $(this).children("option:selected").val();

            var city = parseInt(selectedCity);

            APP.cities.clear({
                container: opts.cntDistrictAdd
            });

            if (cities.includes(city)) {

                APP.districts.getAll({
                    url: '/districts/city/' + city,
                    cntAdd: opts.cntDistrictAdd
                });

                $(opts.cntDistrictAdd).prop("disabled", false);

            } else {
                APP.cities.clear({
                    container: opts.cntDistrictAdd
                });
            }

        });

    };

    var getAllWithDistricts = function (opts) {

        var result = [];

        $.get(opts.url)
            .done(function (data) {

                $.each(data, function (i, item) {

                    result.push(item.id);

                });

            });

        return result;

    };

    var getAllForEdit = function (opts) {

        $.get(opts.url)
            .done(function (data) {

                $.each(data, function (i, item) {

                    var rows = "<option value='" + item.id + "'>" + item.nameFormatted + "</option>";
                    $(opts.cntAdd).append(rows);

                });

                if (opts.selectedCity !== null) {
                    $(opts.cntAdd).val(opts.selectedCity);
                }

            });

        if ((opts.selectedDistrict !== null && opts.selectedDistrict !== '') || opts.hasDistricts === 1) {

            $(opts.cntDistAdd).prop("disabled", false);

            APP.districts.getAll({
                url: '/districts/city/' + opts.selectedCity,
                cntAdd: opts.cntDistAdd,
                selectedDistrict: opts.selectedDistrict
            });

        } else {
            $(opts.cntDistAdd).prop("disabled", true);
        }

        var cities = getAllWithDistricts({ url: '/cities/withdistricts' });

        $(opts.cntAdd).change(function () {

            var selectedCity = $(this).children("option:selected").val();

            var city = parseInt(selectedCity);

            APP.cities.clear({
                container: opts.cntDistAdd
            });

            if (cities.includes(city)) {
                
                APP.districts.getAll({
                    url: '/districts/city/' + city,
                    cntAdd: opts.cntDistAdd
                });

                $(opts.cntDistAdd).prop("disabled", false);

            } else {
                APP.cities.clear({
                    container: opts.cntDistAdd
                });
            }

        });

    };

    var clear = function (opts) {

        $(opts.container).empty();
        $(opts.container).append("<option value='' selected>Wybierz</option>");
        $(opts.container).val(null).trigger('change');
        $(opts.container).prop("disabled", true);

    };

    return {
        getAll: function (opts) {
            getAll(opts);
        },
        getAllWithDistricts: function (opts) {
            return getAllWithDistricts(opts);
        },
        getAllForEdit: function (opts) {
            getAllForEdit(opts);
        },
        clear: function (opts) {
            clear(opts);
        }
    };

})();