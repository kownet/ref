$.urlParam = function (name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)')
        .exec(window.location.search);

    return (results !== null) ? results[1] || 0 : false;
};

$.homeUrl = "https://pewnemieszkanie.pl";
$.successHeader = "Zrobione!";
$.errorHeader = "Błąd!";
$.confirmHeader = "Czy na pewno?";

$.notActiveUserMessage = "Okres próbny minął. Proszę o kontakt na tomek@kownet.info";

$.removeEmpties = function (container) {

    $(document).ready(function () {
        $(container).focusout(function () {
            var text = $(container).val();
            text = text.replace(/(?:(?:\r\n|\r|\n)\s*){2}/gm, "");
            $(this).val(text);
        });
    });

};