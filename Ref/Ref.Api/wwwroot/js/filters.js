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
                            "<td>" + item.name + "<br><small>ostatnie sprawdzenie: " + item.lastCheckedAtFormatted + "</small>" + "</td>" +
                            "<td>" + item.city + "</td>" +
                            "<td>" + item.flatAreaFrom + " - " + item.flatAreaTo + "</td>" +
                            "<td>" + item.priceFrom + " - " + item.priceTo + "</td>" +
                            "<td>" + item.notificationFormatted + "</td>" +
                            "<td>" +
                            "<div class=\"btn-group btn-group-sm\">" +
                            "<button type=\"button\" id=\"btn-filter-del\" class=\"btn btn-sm btn-danger\" data-id=" + item.id + " data-user-id=" + item.userId + "> Usuń</button>" +
                            "<button type=\"button\" id=\"btn-filter-edit\" class=\"btn btn-sm btn-warning\" data-id=" + item.id + " data-user-id=" + item.userId + "> Edytuj</button>" +
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
            console.log(userId);
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

        function closeModal() {

            $(opts.cntModal).modal('hide');
            //hide the modal

            $('body').removeClass('modal-open');
            //modal-open class is added on body so it has to be removed

            $('.modal-backdrop').remove();
            //need to remove div with modal-backdrop class

        }

        $(document).on('change keyup', '.required', function (e) {
            let disabled = true;
            $(".required").each(function () {
                let value = this.value;
                if ((value) && (value.trim() !== '')) {
                    disabled = false;
                } else {
                    disabled = true;
                    return false;
                }
            });

            if (disabled) {
                $('.toggle-disabled').prop("disabled", true);
            } else {
                $('.toggle-disabled').prop("disabled", false);
            }
        });

        $(document).on('click', opts.btn, function () {

            var userId = $(opts.cntUserId).val();

            var params = JSON.stringify({
                userId: userId,
                cityId: $(opts.cntCity).val(),
                flatAreaFrom: $(opts.cntPriceFrom).val(),
                flatAreaTo: $(opts.cntPriceTo).val(),
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
                    console.log(data);
                    if (data.succeed) {

                        swal($.successHeader, {
                            icon: "success"
                        });

                        $(opts.cntForm).trigger("reset");

                        closeModal();

                        APP.filters.getUserFilters({
                            url: '/poc/filters',
                            userId: userId,
                            cntFiltersTable: '#filters-table'
                        });

                    } else {

                        closeModal();

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
        addFilter: function (opts) {
            addFilter(opts);
        }
    };

})();