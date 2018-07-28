// import * as signalR from "../lib/signalr/signalr";

(function () {
    //玩法
    let rule = getLotteryRule() - 0;
    $(".nav-tabs li:eq(" + (rule - 1) + ") a").tab('show');
    loading();

    //创建连接
    let hub = '/hubs/pk10';
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(hub)
        //.withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
        .configureLogging(signalR.LogLevel.Information)
        .build();

    //启动连接并初始化数据
    connection.start().then(
        function () {
            connection.invoke('GetForcastData', rule).catch(err => console.error(err.toString()));
        },
        function () {
            console.error("服务器(" + hub + ")连接失败");
        })
        .catch(error => {
            console.error(error.message);
        });

    function setTotalMoney() {
        let count = $("#betNumbersPreview").val().split(' ').length;
        let money = parseFloat($("#betMoney").val());
        let total = count * money;
        $("#preTotalMoney").text(total);
    }

    $("#betMoney").on("keyup mouseup", function () {
        setTotalMoney();
    });

    function bindBetButtonClick() {
        $(".btnBet").on("click", function () {
            rule = getLotteryRule() - 0;

            let $tr = $(this).parent().parent();
            let lastDrawedPeriod = $tr.find('input[name=lastDrawedPeriod]').val();
            let nextBetPeriod = parseInt(lastDrawedPeriod) + 1;
            let $tds = $tr.find('td');
            let periodRange = $tds.first().next().text().trim();

            let $form = $("#formBetPreview");
            $form.find("#hidRule").val(rule);
            $form.find("#betPeriod").val(nextBetPeriod);
            $form.find("#betNumbersPreview").val(periodRange);

            $("#confirmBet").modal()
            setTotalMoney();
        });
    }


    //显示预测数据
    connection.on("ShowPlans", (data, lotterData) => {

        $('#betBalance').text(lotterData && lotterData.balance);
        $('#totalMoney').text(lotterData && lotterData.totalTotalMoney);

        if (!data || data.length < 2) {
            console.error("预测数据格式错误");
            return;
        }
        data[0].name = "Plan A";
        data[1].name = "Plan B";

        let container = $(template('planContainer')());
        container.append(template('planTemplate', data[0]));
        container.append(template('planTemplate', data[1]));
        $(".tab-pane.active").html(container);

        jqNotify("", "最新期预测号码已更新!", "info");
        bindBetButtonClick();
    });


    //无数据返回
    connection.on("NoResult", () => {
        $(".tab-pane.active").html($(template('noResultTemplate')()));
    });

    // 提醒投注结果
    connection.on("ShowBetResult", (msg, level) => {
        jqNotify("<strong>投注结果:</strong>", msg, level);
    });

    //切换Tab加载数据
    $(".nav-tabs a").on("click", function () {
        let id = $(this).attr("aria-controls");
        loading(id);
        connection.invoke('GetForcastData', id).catch(err => console.error(err.toString()));
        //无刷新更新URL
        history.replaceState(null, null, id);
    });

    //加载动画
    function loading(id) {
        $(".tab-pane" + (!id ? ".active" : "#" + id)).html(template('planLoading')());
    }

    // 点击“下注”
    $("#btnBetConfirm").on("click", function () {
        let $form = $("#formBetPreview");
        let formData = $form.serializeJSON();
        console.log(formData);

        connection.invoke('BetDa2088', parseInt(formData.periodNo), parseInt(formData.rule), formData.numberRange, parseInt(formData.money))
            .catch(err => console.error(err.toString()));

        $("#confirmBet").modal("hide")
    });
})();