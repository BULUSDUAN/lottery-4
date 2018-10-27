import {Component} from '@angular/core';
import {ViewController, LoadingController, ToastController, NavParams, Platform} from 'ionic-angular';
import {HTTP} from '@ionic-native/http';
import {Storage} from '@ionic/storage';

@Component({
    selector: 'page-detail',
    templateUrl: 'detail.html'
})
export class DetailPage {
    rule: string;
    twoSide: boolean;
    plans: any;
    plansSeg: number;
    isiOS: boolean;

    constructor(
        public viewCtrl: ViewController,
        public loadingCtrl: LoadingController,
        public toastCtrl: ToastController,
        private params: NavParams,
        private storage: Storage,
        private platform: Platform,
        private http: HTTP) {
        this.isiOS = platform.is('ios');
        let rules = {'冠军': 1, '亚军': 2, '季军': 3, '第4名': 4, '冠军大小': 5, '冠军单双': 6, '冠军龙虎': 7};
        this.rule = this.params.get('rule');
        let ru = rules[this.rule];
        this.twoSide = ru > 4;
        this.loadData(ru);
    }

    //初始化数据
    loadData(rule: number) {
        //进度条
        const loader = this.loadingCtrl.create({
            content: "加载中...",
        });
        loader.present();

        this.storage.get('config').then(config => {
            let url: string = (!config || !config.initUrl) ? 'http://bet518.win' : config.initUrl;
            this.http.get(url + '/App/PlanDetails/0/' + rule, {}, {})
                .then(data => {
                    loader.dismiss();

                    if (data.status != 200) {
                        this.presentToast("请求数据失败。状态码:" + data.status);
                        return;
                    }
                    this.plans = JSON.parse(data.data);
                    //延迟渲染样式
                    setTimeout(() => {
                        this.plansSeg = this.params.get('plan');
                    }, 100);
                })
                .catch(error => {
                    loader.dismiss();
                    this.presentToast("请求数据失败。错误消息:" + error.error);
                });
        }).catch(err => {
            loader.dismiss();
        });
    }

    switchSeg(seg) {
        this.plansSeg = seg;
    }

    goBack() {
        this.viewCtrl.dismiss();
    }

    //悬浮消息
    presentToast(msg: string) {
        const toast = this.toastCtrl.create({
            message: msg,
            duration: 3000,
            position: 'bottom'
        });
        toast.present();
    };
}
