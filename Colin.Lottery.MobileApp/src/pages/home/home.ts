import {Component, ChangeDetectorRef} from '@angular/core';
import {ToastController, LoadingController, ModalController, Platform} from 'ionic-angular';
import {JPush} from "@jiguang-ionic/jpush";
import {HTTP} from '@ionic-native/http';
import {Storage} from '@ionic/storage';
import {DetailPage} from "./detail";

@Component({
    selector: 'page-home',
    templateUrl: 'home.html'
})
export class HomePage {
    isiOS: boolean;
    ready: boolean = false;
    srcUrl: string;
    forecasts: any[] = [];


    constructor(
        public toastCtrl: ToastController,
        public loadingCtrl: LoadingController,
        public jpush: JPush,
        private http: HTTP,
        private cdRef: ChangeDetectorRef,
        private storage: Storage,
        public modalCtrl: ModalController,
        private platform: Platform
    ) {
        this.isiOS = platform.is('ios');
        this.loadData();

        this.receiveMessage();
    }

    ionViewWillEnter() {
        if (!this.ready)
            return;

        this.storage.get('config').then(config => {
            let url = (!config || !config.initUrl) ? 'http://bet518.win' : config.initUrl;
            if (this.srcUrl.indexOf(url) >= 0)
                return;

            this.loadData();
        });
    }

    //加载数据
    loadData(refresher?) {
        //进度条
        let complete;
        let current = this;
        if (!refresher) {
            const loader = this.loadingCtrl.create({
                content: "加载中...",
            });
            loader.present();
            complete = function () {
                loader.dismiss()
                current.ready = true;
            };
        }
        else {
            complete = function () {
                refresher.complete()
            };
        }

        this.storage.get('config').then(config => {
            this.srcUrl = (!config || !config.initUrl) ? 'http://bet518.win' : config.initUrl;
            let requireTwoSide: number = (!config || !config.twoSide) ? 1 : (config.twoSide ? 1 : 0);
            this.http.get(this.srcUrl + '/App/Plans/0/' + requireTwoSide, {}, {})
                .then(data => {
                    complete();
                    if (data.status != 200)
                        this.presentToast("请求数据失败。状态码:" + data.status);
                    else {
                        let fcs = JSON.parse(data.data);
                        let newPeriod = -1;
                        for (let i = 0; i < fcs.length; i++) {
                            let fc = fcs[i];
                            //处理期号异常数据
                            fc.LastDrawnPeriod =
                                fc.LastDrawnPeriod % 1000 >= fc.LastPeriod
                                    ? Math.floor(fc.LastDrawnPeriod / 1000) * 1000 + fc.LastPeriod - 1
                                    : fc.LastDrawnPeriod;
                            if (fc.LastDrawnPeriod <= newPeriod)
                                continue;

                            newPeriod = fc.LastDrawnPeriod;
                        }
                        let newFcs: any[] = [];
                        for (let i = 0; i < fcs.length; i++) {
                            let fc = fcs[i];
                            if (fc.LastDrawnPeriod < newPeriod)
                                continue;

                            newFcs.push(fc);
                        }
                        this.forecasts = newFcs;
                    }
                })
                .catch(error => {
                    complete();
                    this.presentToast("请求数据失败。错误消息:" + error.error);
                });
        }).catch(err => {
            complete();
        });
    }

    //获取自定义消息推送内容
    receiveMessage() {
        let current: HomePage = this;
        document.addEventListener("jpush.receiveMessage", function (event: any) {
            let message: string = current.platform.is('android') ? event.message : event.content;
            let plans: any[] = JSON.parse(message);
            if (!plans || plans.length <= 0) {
                return;
            }
            //处理期号异常数据
            for (let i = 0; i < plans.length; i++) {
                let fc = plans[i];
                fc.LastDrawnPeriod =
                    fc.LastDrawnPeriod % 1000 >= fc.LastPeriod
                        ? Math.floor(fc.LastDrawnPeriod / 1000) * 1000 + fc.LastPeriod - 1
                        : fc.LastDrawnPeriod;
            }

            let allFcs = current.forecasts.concat(plans);
            let validFcs = [];
            let currentPeriod = -1;
            for (let i = 0; i < allFcs.length; i++) {
                let fc = allFcs[i];
                let key = fc.Rule + fc.Plan;

                if (!validFcs[key] || fc.LastDrawnPeriod > validFcs[key].LastDrawnPeriod)
                    validFcs[key] = fc;

                if (fc.LastDrawnPeriod > currentPeriod)
                    currentPeriod = fc.LastDrawnPeriod;
            }
            let newFcs: any[] = [];
            for (let key in validFcs) {
                let fc = validFcs[key];
                if (fc.LastDrawnPeriod < currentPeriod)
                    continue;

                newFcs.push(fc);
            }
            let rules = {'冠军': 1, '亚军': 2, '季军': 3, '第4名': 4, '冠军大小': 5, '冠军单双': 6, '冠军龙虎': 7};
            let compare = function (fc1, fc2) {
                if (rules[fc1.Rule] < rules[fc2.Rule])
                    return -1;
                else if (rules[fc1.Rule] > rules[fc2.Rule])
                    return 1;
                else {
                    if (fc1.Plan < fc2.Plan)
                        return -1;
                    else if (fc1.Plan > fc2.Plan)
                        return 1;
                }
                return 0;
            };
            newFcs.sort(compare);
            current.forecasts = newFcs;
            current.cdRef.detectChanges();
        }, false);
    }

    //查看详情
    viewDetailModal(rule, plan) {
        let profileModal = this.modalCtrl.create(DetailPage, {rule: rule, plan: plan});
        profileModal.present();
    }

    //删除
    remove(index: number) {
        if (index < 0 || index > this.forecasts.length)
            return;

        this.forecasts.splice(index, 1);
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
