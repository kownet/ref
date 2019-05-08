APP.cities = (function () {

    var getAll = function (opts) {

        var cities = getAllWithDistricts({ url: '/cities/withdistricts' });

        function clearContainer(container) {

            $(container).empty();
            $(container).append("<option value='' selected>Wybierz</option>");
            $(container).val(null).trigger('change');
            $(container).prop("disabled", true);

        }

        $.get(opts.url)
            .done(function (data) {

                $.each(data, function (i, item) {
                    var rows = "<option value='" + item.id + "'>" + item.nameFormatted + "</option>";
                    $(opts.cntAdd).append(rows);
                    $(opts.cntEdit).append(rows);
                });

            });

        $(opts.cntAdd).change(function () {

            var selectedCity = $(this).children("option:selected").val();

            var city = parseInt(selectedCity);

            clearContainer(opts.cntDistrictAdd);

            if (cities.includes(city)) {

                APP.districts.getAll({
                    url: '/districts/city/' + city,
                    cntAdd: opts.cntDistrictAdd
                });

                $(opts.cntDistrictAdd).prop("disabled", false);

            } else {
                clearContainer(opts.cntDistrictAdd);
            }

        });

        $(opts.cntEdit).change(function () {

            var selectedCity = $(this).children("option:selected").val();

            var city = parseInt(selectedCity);

            clearContainer(opts.cntDistrictEdit);

            if (cities.includes(city)) {

                APP.districts.getAll({
                    url: '/districts/city/' + city,
                    cntAdd: opts.cntDistrictEdit
                });

                $(opts.cntDistrictEdit).prop("disabled", false);

            } else {
                clearContainer(opts.cntDistrictEdit);
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

    return {
        getAll: function (opts) {
            getAll(opts);
        },
        getAllWithDistricts: function (opts) {
            return getAllWithDistricts(opts);
        }
    };

})();