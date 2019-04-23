APP.cities = (function () {

    var getAll = function (opts) {

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

            if ($.citiesWithDistrict.includes(city)) {

                APP.districts.getAll({
                    url: '/districts/city/' + city,
                    cntAdd: opts.cntDistrictAdd
                });

                $(opts.cntDistrictAdd).prop("disabled", false);

            } else {
                $(opts.cntDistrictAdd).empty();
                $(opts.cntDistrictAdd).append("<option value='' selected disabled>Wybierz</option>");
                $(opts.cntDistrictAdd).val(null).trigger('change');
                $(opts.cntDistrictAdd).prop("disabled", true);
            }
            
        });

    };

    return {
        getAll: function (opts) {
            getAll(opts);
        }
    };

})();