(function () {
    //设置导航条高亮
    (function () {
        let menu = location.pathname.split('/')[1];
        $(".navbar-nav li").each(function () {
            if ($(this).children("a").attr("href") === "/" + menu)
                $(this).addClass("active").siblings().removeClass("active");
        });
    })();

    //消息通知
    (function () {
        //创建连接
        let hub = '/hubs/notify';
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(hub)
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        //启动连接并初始化数据
        connection.start().then(
            function () {
                connection.invoke('GetNotifications', 100, true);
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

            for (let i = msgs.length - 1; i >= 0; i--) {
                notify('高分提醒', msgs[i]);
            }
        });
    })();
})();

//获取URL中最后一个参数
function getLastParameter() {
    return location.pathname.split('/').pop();
}

function notify(title, content) {
    if (!title || !content) {
        console.error('消息通知参数错误');
        return;
    }

    let options = {body: content, icon: '/images/logo.png'};

    // 先检查浏览器是否支持
    if (!("Notification" in window))
        alert("抱歉，您的浏览器不支持通知提醒");

    // 检查用户是否同意接受通知
    else if (Notification.permission === "granted")
        new Notification(title, options);

    // 否则我们需要向用户获取权限
    else if (Notification.permission === 'default') {
        Notification.requestPermission(function (permission) {
            // 如果用户同意，就可以向他们发送通知
            if (permission === "granted") 
                new Notification(title, options);
        });
    }
    // 最后，如果执行到这里，说明用户已经拒绝对相关通知进行授权
    // 出于尊重，我们不应该再打扰他们了
}

