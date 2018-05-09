(function () {
    $('#test').click();
    $('#test1').click();

    // localStorage.setItem("colin",'chang');
    // alert(localStorage.getItem('colin'));

    //玩法联动效果
    $('[name=lotteryType]').change(function () {
        $('#rule_' + $(this).val()).removeClass('hide').siblings().addClass('hide');
    });
})();