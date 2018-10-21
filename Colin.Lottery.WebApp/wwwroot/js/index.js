// import * as signalR from "../lib/signalr/signalr";

(function () {
    //更新PK10计时器
    (function () {
        let timer_pk10 = $("#timer-pk10");
        let now = new Date();
        let curMin = now.getMinutes();
        let deadMin = 0;
        for (let i = curMin; i <= 60; i++) {
            if (i % 5 !== 0)
                continue;

            deadMin = i;
            break;
        }
        let totalSeconds = (deadMin - curMin) * 60 - now.getSeconds();
        setTime(totalSeconds);

        function setTime(seconds) {
            seconds = seconds <= 0 ? seconds + 5 * 60 : seconds;
            let leftTime = Math.floor(seconds / 60).toString().padStart(2, '0') + ":" + (seconds % 60).toString().padStart(2, '0');
            timer_pk10.text(leftTime);
        }

        function updatePK10Timer() {
            let cur = timer_pk10.text();
            if (!cur)
                return;

            let parts = cur.split(':');
            let leftTime = (parts[0] - 0) * 60 + (parts[1] - 0) - 1;
            setTime(leftTime);
        }

        setInterval(updatePK10Timer, 1000);
    })();

    //PK10汇总数据
    (function () {
        loading();

        //创建连接
        let hub = '/hubs/pk10';
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(hub)
            //.withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        //启动连接并初始化数据
        connection.start().then(
            function () {
                connection.invoke('GetAllNewForecast');
            },
            function () {
                console.error("服务器(" + hub + ")连接失败");
            })
            .catch(error => {
                console.error(error.message);
            });

        let dict = {
            '冠军': 'champion',
            '亚军': 'second',
            '季军': 'third',
            '第4名': 'fourth',
            '冠军大小': 'bigOrSmall',
            '冠军单双': 'oddOrEven',
            '冠军龙虎': 'dragonOrTiger',
            '冠亚和值': 'sum'
        };

        function getRuleName(rule) {
            for (let name in dict) {
                if (name === rule)
                    return dict[name];
            }
            console.error('没有给定的玩法' + rule)
        }


        //显示预测数据
        connection.on("ShowPlans", data => {
            if (!data || data.length <= 0)
                return;

            let plans = {'champion': [], 'second': [], 'third': [], 'fourth': [], 'bigOrSmall': [], 'oddOrEven': [], 'dragonOrTiger': [], 'sum': []};
            for (let i = 0; i < data.length; i++) {
                let plan = data[i];
                plan.cls = plan.score < 80 ? 'text-success' : 'text-danger';
                plans[dict[plan.rule]].push(plan);
            }
            for (let rule in plans) {
                if (plans[rule].length <= 0)
                    continue;

                let tr = $('#' + rule);
                let title = tr.children('td:first');
                title.children('.expired').remove();
                title.nextAll().remove();
                tr.append(template('planTemplate', plans[rule][0]));
                tr.next().html(template('planTemplate', plans[rule][1]));
            }

            //更新期号
            let pc = $('.pk-period');
            let newPeriod = data.pop().lastPeriod;
            if (pc.text() - 0 < newPeriod)
                pc.text(newPeriod.toString().padStart(3, '0'));

            $('.planLoading').remove();
            $('.plan-noresult').remove();
            $('.hide').removeClass('hide');
        });

        //无数据返回
        connection.on("NoResult", function (rule) {
            if (rule === 'allRules') {
                $('.planLoading').remove();
                $('.pk10-panel').append(template('noResultTemplate')());
                return;
            }

            let td = $('#' + getRuleName(rule) + ' td:first');
            if (td.children('.expired').length <= 0)
                td.append(template('planExpired')());
        });

        //加载动画
        function loading() {
            $('.pk10-panel').append(template('planLoading')());
        }
    })();
})(); 
