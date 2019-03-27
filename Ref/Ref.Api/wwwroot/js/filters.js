APP.filters = (function () {

    var getUserFilters = function (opts) {

        var params = JSON.stringify({ userId: opts.userId });

        $.ajax({
            contentType: 'application/json',
            type: 'POST',
            url: opts.url,
            data: params,
            success: function (data) {

                if (data.succeed) {

                    $(opts.cntFiltersTable).empty();

                    $.each(data.filters, function (i, item) {
                        var rows = "<tr>" +
                            "<td>" + item.name + "<br><small>ostatnie powiadomienie: " + item.lastCheckedAtFormatted + "</small>" + "</td>" +
                            "<td>" + item.city + "</td>" +
                            "<td>" + item.flatAreaFrom + " - " + item.flatAreaTo + "</td>" +
                            "<td>" + item.priceFrom + " - " + item.priceTo + "</td>" +
                            "<td>" + item.notificationFormatted + "</td>" +
                            "<td>" +
                            "<div class=\"btn-group btn-group-sm\">" +
                            "<button type=\"button\" class=\"btn btn-sm btn-danger\" id=\"btn-filter-del\" data-id=" + item.id + " data-user-id=" + item.userId + "> Usuń</button>" +
                            "<button type=\"button\" class=\"btn btn-sm btn-warning\" id=\"btn-filter-edit\" data-id=" + item.id + " data-user-id=" + item.userId + " data-toggle=\"modal\" data-target=\"#filter-edit\"> Edytuj</button>" +
                            "</div >" + "</td>" +
                            "</tr>";
                        $(opts.cntFiltersTable).append(rows);
                    });

                    APP.filters.deleteFilter({
                        url: '/poc/deletefilter',
                        btnDelete: '#btn-filter-del'
                    });

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
                buttons: true,
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
                                    url: '/poc/filters',
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

    var addFilter = function (opts) {

        APP.filters.validateRequired({
            requiredClass: '.required',
            disabledClass: '.toggle-disabled'
        });

        $(document).on('click', opts.btn, function () {

            var userId = $(opts.cntUserId).val();

            var params = JSON.stringify({
                userId: userId,
                cityId: $(opts.cntCity).val(),
                flatAreaFrom: $(opts.cntAreaFrom).val(),
                flatAreaTo: $(opts.cntAreaTo).val(),
                priceFrom: $(opts.cntPriceFrom).val(),
                priceTo: $(opts.cntPriceTo).val(),
                notification: $(opts.cntNtf).val(),
                name: $(opts.cntName).val()
            });

            $.ajax({
                contentType: 'application/json',
                type: 'POST',
                url: opts.url,
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

    var editFilter = function (opts) {

        APP.filters.validateRequired({
            requiredClass: '.required-edit',
            disabledClass: '.toggle-disabled-edit'
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
                        $(opts.cntAreaFrom).val(data.filter.flatAreaFrom);
                        $(opts.cntAreaTo).val(data.filter.flatAreaTo);
                        $(opts.cntNtf).val(data.filter.notification);

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
                notification: $(opts.cntNtf).val(),
                name: $(opts.cntName).val()
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

    var closeModal = function (opts) {

        $(opts.cntModal).modal('hide');
        //hide the modal

        $('body').removeClass('modal-open');
        //modal-open class is added on body so it has to be removed

        $('.modal-backdrop').remove();
        //need to remove div with modal-backdrop class
    };

    var validateRequired = function (opts) {

        $(document).on('change keyup', opts.requiredClass, function (e) {

            let disabled = true;

            $(opts.requiredClass).each(function () {

                let value = this.value;

                if ((value) && (value.trim() !== '')) {
                    disabled = false;
                } else {
                    disabled = true;
                    return false;
                }
            });

            if (disabled) {
                $(opts.disabledClass).prop("disabled", true);
            } else {
                $(opts.disabledClass).prop("disabled", false);
            }
        });

    };

    return {
        getUserFilters: function (opts) {
            getUserFilters(opts);
        },
        deleteFilter: function (opts) {
            deleteFilter(opts);
        },
        addFilter: function (opts) {
            addFilter(opts);
        },
        editFilter: function (opts) {
            editFilter(opts);
        },
        closeModal: function (opts) {
            closeModal(opts);
        },
        validateRequired: function (opts) {
            validateRequired(opts);
        }
    };

})();