let ssrs = [],ssrStrs;

function analyze(text) {
    //解码
    asyncFromBase64(text, 'utf8', function (data) {
        ssrStrs = data.split('\n');
        for (let i = 0; i < ssrStrs.length; i++)
            ssrDecode(ssrStrs[i].slice(6));
    });

    //渲染列表
    $('#ssrs').html(template('ssrTemplate', ssrs));

    //导出配置
    $('#btn-export').removeAttr('disabled').on('click', function () {
        let configs = {'configs': ssrs};
        console.log(JSON.stringify(configs));
    });

    //二维码
    $('.qrCode').on('click', function () {
        let ssContianer = $('#ssCode');
        ssContianer.html('');
        let ss = ssrs[$(this).attr('no')];
        asyncToBase64(ss.method+':'+ss.password+'@'+ss.server+':'+ss.server_port,'utf8',function (data) {
            let ssStr="ss://"+data;
            new QRCode(ssContianer[0], {
                text: ssStr,
                width: 250,
                height: 250,
                colorDark: "#000000",
                colorLight: "#ffffff",
                correctLevel: QRCode.CorrectLevel.H
            });
            ssContianer.next().children().last().text(ssStr);
        });

        let ssrContianer = $('#ssrCode');
        ssrContianer.html('');
        let ssr = ssrStrs[$(this).attr('no')];
        new QRCode(ssrContianer[0], {
            text: ssr,
            width: 250,
            height: 250,
            colorDark: "#000000",
            colorLight: "#ffffff",
            correctLevel: QRCode.CorrectLevel.H
        });
        ssrContianer.next().children().last().text(ssr);
    });
}

function ssrDecode(text) {
    if (!text) return;
    asyncFromBase64(text, 'utf8', function (data) {
            let arr = data.split(':');
            let ssr = {
                "server": arr[0],
                "server_port": arr[1],
                "protocol": arr[2],
                "method": arr[3],
                "obfs": arr[4]
            };
            let arr2 = arr[5].split('/?');
            asyncFromBase64(arr2[0], 'utf8', function (data) {
                ssr.password = data;
                let arr3 = arr2[1].split('&');
                for (let i = 0; i < arr3.length; i++) {
                    let parts = arr3[i].split('=');
                    ssr[parts[0]] = parts[1];
                }
                if (!!ssr["remarks"])
                    asyncFromBase64(ssr.remarks, 'utf8', function (data) {
                        ssr.remarks = data;
                    });
                if (!!ssr["group"])
                    asyncFromBase64(ssr.group, 'utf8', function (data) {
                        ssr.group = data;
                    });

                ssrs.push(ssr);
            }, function (err) {
                notify('SSR解析错误', 'SSR解析出现错误,请稍后重试');
                console.error(err);
            })
        },
        function (err) {
            notify('SSR解析错误', 'SSR解析出现错误,请稍后重试');
            console.error(err);
        });
}

function makeCode() {
    console.log(123);
}