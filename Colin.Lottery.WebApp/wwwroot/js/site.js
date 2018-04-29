(function () {
    //设置导航条高亮
    (function () {
        let menu = location.pathname.split('/')[1];
        $(".navbar-nav li").each(function () {
            if ($(this).children("a").attr("href") == "/" + menu)
                $(this).addClass("active").siblings().removeClass("active");
        });
    })();

    //消息通知
    (function () {
        //创建连接
        let hub = '/hubs/notify';
        let connection = new signalR.HubConnection(hub);

        //启动连接并初始化数据
        connection.start().then(
            function () {
                connection.invoke('GetNotifications', 80,true);
            },
            function () {
                console.error("服务器(" + hub + ")连接失败");
            })
            .catch(error => {
                console.error(error.message);
            });

        //显示预测数据
        connection.on("Notify", function (msgs) {
            if (!msgs || msgs.length <= 0)
                return;

            for (let i = 0; i < msgs.length; i++) {
                $.notify(msgs[i], "success");
            }
        });
    })();
})();

//获取URL中最后一个参数
function getLastParameter() {
    return location.pathname.split('/').pop();
}

