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

    return {
        getUserFilters: function (opts) {
            getUserFilters(opts);
        },
        deleteFilter: function (opts) {
            deleteFilter(opts);
        }
    };

})();