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
                        $(opts.inputEmail).val(data.email);
                        $(opts.registeredAtInfo).text('Data rejestracji: ' + data.registeredAt);
                    } else {
                        console.log(data.message);
                    }

                }
            });

        }

    };

    return {
        checkGuid: function (opts) {
            checkGuid(opts);
        }
    };

})();