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
            connection.invoke('GetForecastData', rule).catch(err => console.error(err.toString()));
        },
        function () {
            console.error("服务器(" + hub + ")连接失败");
        })
        .catch(error => {
            console.error(error.message);
        });

    //显示预测数据
    connection.on("ShowPlans", (data) => {
        if (!data || data.length < 2) {
            console.error("预测数据格式错误");
            return;
        }
        data[0].name = "Plan A";
        data[1].name = "Plan B";

        let container = $(template('planContainer')());
        container.append(template('planTemplate', data[0]));
        container.append(template('planTemplate', data[1]));
        $(".tab-pane#" + rule).html(container);
    });


    //无数据返回
    connection.on("NoResult", () => {
        $(".tab-pane.active").html($(template('noResultTemplate')()));
    });

    //切换Tab加载数据
    $(".nav-tabs a").on("click", function () {
        rule = $(this).attr("aria-controls");
        loading(rule);
        connection.invoke('GetForecastData', rule).catch(err => console.error(err.toString()));
        //无刷新更新URL
        history.replaceState(null, null, rule);
    });

    //加载动画
    function loading() {
        $(".tab-pane" + (!rule ? ".active" : "#" + rule)).html(template('planLoading')());
    }
})();