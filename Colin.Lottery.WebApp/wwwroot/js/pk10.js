(function (win) {
    //玩法
    let rule = getLotteryRule();
    $(".nav-tabs li:eq(" + (rule - 1) + ") a").tab('show');
    loading();

    //创建连接
    let hub = '/hubs/pk10';
    let connection = new signalR.HubConnection(hub);

    //启动连接并初始化数据
    connection.start().then(
        function () {
            connection.invoke('GetForcastData', rule, true);
        },
        function () {
            console.error("服务器(" + hub + ")连接失败");
        })
        .catch(error => {
            console.error(error.message);
        });

    //显示预测数据
    connection.on("ShowPlans", function (data) {
        data.item1.name = "Plan A";
        data.item2.name = "Plan B";

        let container = $(template('planContainer')());
        container.append(template('planTemplate', data.item1));
        container.append(template('planTemplate', data.item2));
        $(".tab-pane.active").html(container);
    });

    //切换Tab加载数据
    $(".nav-tabs a").on("click", function () {
        let id = $(this).attr("aria-controls");
        loading(id);
        connection.invoke('GetForcastData', id, true);
        //无刷新更新URL
        history.replaceState(null, null, id);
    });

    //加载动画
    function loading(id) {
        $(".tab-pane" + (!id ? ".active" : "#" + id)).html(template('planLoading')());
    }


})(window);