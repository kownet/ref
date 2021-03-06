﻿APP.filters = (function () {

    var getUserFilters = function (opts) {

        function parseValues(from, to) {

            var info = "<td class=\"d-none d-md-table-cell\">";

            if (from === null && to === null) {
                info = info + "brak";
            }

            if (from !== null && to === null) {
                if (from === 0) {
                    info = info + "brak";
                } else {
                    info = info + "od " + from;
                }
            }

            if (from === null && to !== null) {
                if (to === 0) {
                    info = info + "brak";
                } else {
                    info = info + "do " + to;
                }
            }

            if (from !== null && to !== null) {
                if (from === 0 && to === 0) {
                    info = info + "brak";
                } else if (from === 0) {
                    info = info + "do " + to;
                }
                else if (to === 0) {
                    info = info + "od " + from;
                }
                else {
                    info = info + from + " - " + to;
                }
            }

            info = info + "</td>";

            return info;
        }

        function parseProperty(property) {

            if (property === 0) {
                return "- mieszkania";
            }

            if (property === 1) {
                return "- domy";
            }
        }

        var params = JSON.stringify({ userId: opts.userId });

        $.ajax({
            contentType: 'application/json',
            type: 'POST',
            url: opts.url,
            data: params,
            success: function (data) {

                if (data.succeed) {

                    $(opts.cntFiltersTable).empty();

                    if (data.filters.length !== 0) {

                        $.each(data.filters, function (i, item) {

                            var keywordsContain = !item.shouldContain
                                ? ""
                                : "<tr class=\"tr-small\">" +
                                "<td colspan=\"7\" class=\"d-none d-sm-table-cell\">&nbsp;<i>Ogłoszenia powinny zawierać frazy:</i> '" + item.shouldContain + "'</td>" +
                                "</tr>";

                            var keywordsNotContain = !item.shouldNotContain
                                ? ""
                                : "<tr class=\"tr-small\">" +
                                "<td colspan=\"7\" class=\"d-none d-sm-table-cell\">&nbsp;<i>Ogłoszenia <strong>nie</strong> powinny zawierać fraz:</i> '" + item.shouldNotContain + "'</td>" +
                                "</tr>";

                            var ppmInfo = parseValues(item.pricePerMeterFrom, item.pricePerMeterTo);
                            var faInfo = parseValues(item.flatAreaFrom, item.flatAreaTo);
                            var pInfo = parseValues(item.priceFrom, item.priceTo);

                            var city = item.district === null
                                ? item.city
                                : item.city + "<br>" + "<small>(" + item.district + ")</small>";

                            var property = parseProperty(item.property);

                            var allOrPrivate = item.allowFromAgency ? "<small class=\"text-primary\">wszystkie ogłoszenia " + property + "</small>" : "<small class=\"text-warning\">ogłoszenia prywatne " + property + "</small>";

                            var rows = "<tr>" +
                                "<td>" + item.name + "<br>" + allOrPrivate + "<br><small>ostatnie powiadomienie: " + item.lastCheckedAtFormatted + "</small>" + "</td>" +
                                "<td class=\"d-none d-sm-table-cell\">" + city + "</td>" +
                                faInfo +
                                pInfo +
                                ppmInfo +
                                "<td class=\"d-none d-lg-table-cell\">" + item.notificationFormatted + "</td>" +
                                "<td>" +
                                "<div class=\"btn-group btn-group-sm\">" +
                                "<button type=\"button\" class=\"btn btn-sm btn-danger\" id=\"btn-filter-del\" data-id=" + item.id + " data-user-id=" + item.userId + "> Usuń</button>" +
                                "<a class=\"btn btn-sm btn-warning\" id=\"btn-filter-edit\" data-id=" + item.id + " data-user-id=" + item.userId + " data-district-id=" + item.districtId + " href=\"/edit/" + item.userGuid + "/" + item.id + "\"> Edytuj</a>" +
                                "</div >" + "</td>" +
                                "</tr>" +
                                keywordsContain +
                                keywordsNotContain;

                            $(opts.cntFiltersTable).append(rows);
                        });

                        APP.filters.deleteFilter({
                            url: '/poc/deletefilter',
                            btnDelete: '#btn-filter-del'
                        });

                    }

                } else {
                    swal($.errorHeader, data.message, "error");
                }

            }
        });
    };

    var deleteFilter = function (opts) {

        $(document).on('click', opts.btnDelete, function () {

            var filterId = $(this).data("id");
            var userId = $(this).data("user-id");

            swal({
                title: $.confirmHeader,
                text: "Usuniętego filtra nie będzie można odzyskać!",
                icon: "warning",
                buttons: ["Anuluj", "Usuń"],
                dangerMode: true
            }).then((willDelete) => {
                if (willDelete) {

                    $.ajax({
                        type: 'DELETE',
                        url: opts.url + '/' + filterId,
                        success: function (data) {

                            if (data.succeed) {

                                swal($.successHeader, {
                                    icon: "success"
                                });

                                APP.filters.getUserFilters({
                                    url: '/filters/allbyuserid',
                                    userId: userId,
                                    cntFiltersTable: '#filters-table'
                                });

                            } else {
                                swal($.errorHeader, data.message, "error");
                            }

                        }
                    });

                }
            });

        });

    };

    var editFilter = function (opts) {

        APP.filters.validateRequired({
            requiredClass: '.required-edit',
            disabledClass: '.toggle-disabled-edit',
            notClass: '.not'
        });

        var filterId = null;
        var userId = null;

        $(document).on('click', opts.btn, function (e) {

            e.preventDefault();

            filterId = $(this).data("id");
            userId = $(this).data("user-id");

            $.get(opts.url + '/' + filterId)
                .done(function (data) {

                    if (data.succeed) {

                        $(opts.cntName).val(data.filter.name);
                        $(opts.cntCity).val(data.filter.cityId);
                        $(opts.cntPriceFrom).val(data.filter.priceFrom);
                        $(opts.cntPriceTo).val(data.filter.priceTo);
                        $(opts.cntPpmFrom).val(data.filter.pricePerMeterFrom);
                        $(opts.cntPpmTo).val(data.filter.pricePerMeterTo);
                        $(opts.cntAreaFrom).val(data.filter.flatAreaFrom);
                        $(opts.cntAreaTo).val(data.filter.flatAreaTo);
                        $(opts.cntNtf).val(data.filter.notification);
                        $(opts.cntPrp).val(data.filter.property);
                        $(opts.cntShouldContain).val(data.filter.shouldContain);
                        $(opts.cntShouldNotContain).val(data.filter.shouldNotContain);
                        $(opts.cntDistrictEdit).val(data.filter.districtId);

                        if (data.filter.districtId !== null) {

                            $(opts.cntDistrictEdit).empty();
                            $(opts.cntDistrictEdit).append("<option value='' selected>Wybierz</option>");
                            $(opts.cntDistrictEdit).val(null).trigger('change');

                            $(opts.cntDistrictEdit).prop("disabled", false);

                            APP.districts.getAll({
                                url: '/districts/city/' + data.filter.cityId,
                                cntAdd: opts.cntDistrictEdit,
                                selectedDistrict: data.filter.districtId
                            });
                        }

                    } else {
                        swal($.errorHeader, data.message, "error");
                    }

                });

        });

        $(document).on('click', opts.btnSave, function (e) {

            e.preventDefault();

            var params = JSON.stringify({
                id: filterId,
                userId: userId,
                cityId: $(opts.cntCity).val(),
                flatAreaFrom: $(opts.cntAreaFrom).val(),
                flatAreaTo: $(opts.cntAreaTo).val(),
                priceFrom: $(opts.cntPriceFrom).val(),
                priceTo: $(opts.cntPriceTo).val(),
                pricePerMeterFrom: $(opts.cntPpmFrom).val(),
                pricePerMeterTo: $(opts.cntPpmTo).val(),
                notification: $(opts.cntNtf).val(),
                property: $(opts.cntPrp).val(),
                name: $(opts.cntName).val(),
                shouldContain: $(opts.cntShouldContain).val(),
                shouldNotContain: $(opts.cntShouldNotContain).val(),
                districtId: $(opts.cntDistrictEdit).val()
            });

            $.ajax({
                contentType: 'application/json',
                type: 'PUT',
                url: opts.urlSave,
                data: params,
                success: function (data) {

                    if (data.succeed) {

                        swal($.successHeader, {
                            icon: "success"
                        });

                        $(opts.cntForm).trigger("reset");

                        APP.filters.closeModal({
                            cntModal: opts.cntModal
                        });

                        APP.filters.getUserFilters({
                            url: '/poc/filters',
                            userId: userId,
                            cntFiltersTable: '#filters-table'
                        });

                    } else {

                        APP.filters.closeModal({
                            cntModal: opts.cntModal
                        });

                        swal($.errorHeader, data.message, "error");
                    }

                }
            });

        });

    };

    return {
        getUserFilters: function (opts) {
            getUserFilters(opts);
        },
        deleteFilter: function (opts) {
            deleteFilter(opts);
        },
        editFilter: function (opts) {
            editFilter(opts);
        }
    };

})();