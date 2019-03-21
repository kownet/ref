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
                        $(opts.cntInputEmail).val(data.email);
                        $(opts.cntRegisteredAtInfo).text('Data rejestracji: ' + data.registeredAt);
                        $(opts.cntUserId).val(data.userId);
                    } else {
                        swal($.errorHeader, data.message, "error");
                    }

                }
            });

        }
    };

    var updateEmail = function (opts) {

        $(document).on('click', opts.btnSave, function () {

            var email = $(opts.cntInputEmail).val();
            var id = $(opts.cntUserId).val();

            var params = JSON.stringify({ email: email, id: id });

            $.ajax({
                contentType: 'application/json',
                type: 'POST',
                url: opts.url,
                data: params,
                success: function (data) {

                    if (data.succeed) {
                        swal($.successHeader, "Adres email został zmieniony.", "success");
                    } else {
                        swal($.errorHeader, data.message, "error");
                    }

                }
            });

        });

    };

    return {
        checkGuid: function (opts) {
            checkGuid(opts);
        },
        updateEmail: function (opts) {
            updateEmail(opts);
        }
    };

})();