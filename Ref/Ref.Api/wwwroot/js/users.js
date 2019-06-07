APP.users = (function () {

    var updateEmail = function (opts) {

        var email = $(opts.cntInputEmail).val();
        var id = opts.userId;

        $(document).on('click', opts.btnSave, function () {

            var newEmailValue = $(opts.cntInputEmail).val();

            var params = JSON.stringify({ email: newEmailValue, id: id });

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

                        $(opts.cntInputEmail).val(email);
                    }

                }
            });

        });

    };

    var checkIfActive = function (opts) {

        if (opts.active === 0) {
            swal({
                title: $.errorHeader,
                text: $.notActiveUserMessage,
                icon: "error",
                dangerMode: true
            }).then((goOut) => {
                if (goOut) {

                    window.location.replace($.homeUrl);

                }
            });
        }

    };

    return {
        updateEmail: function (opts) {
            updateEmail(opts);
        },
        checkIfActive: function (opts) {
            checkIfActive(opts);
        }
    };

})();