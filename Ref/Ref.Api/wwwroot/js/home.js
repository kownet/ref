APP.home = (function () {

    var checkGuid = function (opts) {

        function goBackIfError() {
            window.location.replace($.homeUrl);
        }

        if (opts.guid === false) {
            goBackIfError();
        } else {

            var params = JSON.stringify({ guid: opts.guid });

            $.ajax({
                contentType: 'application/json',
                type: 'POST',
                url: opts.url,
                data: params,
                success: function (data) {

                    if (data.succeed) {
                        
                        if (data.isActive) {

                            $(opts.cntInputEmail).val(data.email);
                            $(opts.cntRegisteredAtInfo).text('Data rejestracji: ' + data.registeredAt);
                            $(opts.cntUserId).val(data.userId);

                            APP.users.updateEmail({
                                url: '/poc/email',
                                userId: data.userId,
                                cntInputEmail: '#input-email',
                                btnSave: '#input-email-save'
                            });

                            APP.filters.getUserFilters({
                                url: '/poc/filters',
                                userId: data.userId,
                                cntFiltersTable: '#filters-table'
                            });

                        } else {

                            swal({
                                title: $.errorHeader,
                                text: $.notActiveUserMessage,
                                icon: "error",
                                dangerMode: true
                            }).then((goOut) => {
                                if (goOut) {

                                    goBackIfError();

                                }
                            });

                        }

                    } else {
                        swal($.errorHeader, data.message, "error");
                    }

                }
            });

        }
    };

    var yesOrNo = function (opts) {

        var rowYes = "<option value=\"1\">Tak</option>";
        var rowNo = "<option value=\"0\">Nie</option>";

        $(opts.cntAdd).append(rowYes);
        $(opts.cntAdd).append(rowNo);

        if (opts.selected !== null) {
            $(opts.cntAdd).val(opts.selected);
        }

    };

    return {
        checkGuid: function (opts) {
            checkGuid(opts);
        },
        yesOrNo: function (opts) {
            yesOrNo(opts);
        }
    };

})();