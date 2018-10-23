import {Component} from '@angular/core';
import {AlertController, ToastController} from 'ionic-angular';
import {JPush} from "@jiguang-ionic/jpush";
import {Storage} from '@ionic/storage';

@Component({
    selector: 'page-settings',
    templateUrl: 'settings.html'
})
export class SettingsPage {
    sequence: number = 0;
    config: any;
    betPlatform: any[];

    constructor(public alertCtrl: AlertController,
                public toastCtrl: ToastController,
                public jpush: JPush,
                private storage: Storage) {
        this.betPlatform = [
            {name: '谦喜彩票(9.96)', url: 'https://qxbet.com/wap#/home'},
            {name: '大象彩(9.95)', url: 'https://da8088.com/mobile/#/home/'},
            {name: '鸿利彩票(9.95)', url: 'http://m.hl3999.com/'},
            {name: '金皇朝(9.56)', url: 'https://c1.jhc2go.com/'},
        ];
        if (!this.config)
            this.config = {
                initUrl: "http://bet518.win",
                betPlatform: this.betPlatform[0].url,
                realTimeUpdate: false,
                twoSide: false,
                keepGuaAlert: false,
                keepGuaCnt: 2,
                repetitionAlert: false,
                repetition: 80
            };

        storage.get('config').then(cfg => {
            if (!cfg)
                storage.set('config', this.config);
            else
                this.config = cfg;
        });
    }

    onSave() {
        let tags: string[] = [];
        if (this.config.realTimeUpdate)
            tags.push('realtime');
        if (this.config.twoSide)
            tags.push('twoside');
        if (this.config.keepGuaAlert)
            tags.push("liangua" + this.config.keepGuaCnt);
        if (this.config.repetitionAlert)
            tags.push("repetition" + this.config.repetition);

        this.jpush
            .cleanTags({sequence: this.sequence++})
            .then(data => {
                if (tags.length <= 0) {
                    this.storage.set('config', this.config);
                    this.presentToast("配置保存成功,但未开启数据更新");
                    return;
                }

                //重新设置Tags
                this.setTags(tags);
            })
            .catch(err => {
                this.saveExceptJpush("Tag清理失败" + "\nCode: " + err.code);
            });
    }

    //覆盖设置Tags
    setTags(tagsName: string[]) {
        this.jpush
            .setTags({sequence: this.sequence++, tags: tagsName})
            .then(result => {
                this.storage.set('config', this.config);
                let tags: Array<string> = result.tags == null ? [] : result.tags;
                this.presentToast("配置保存成功，并成功开启数据更新。注册标签组:\n" + tags.toString());
            })
            .catch(err => {
                this.saveExceptJpush("Tag设置失败" + "\nCode: " + err.code);
            });
    }

    //保存基本配置(除Jpush操作)
    saveExceptJpush(baseErrMsg: string) {
        this.storage.get('config').then(cfg => {
            let oldConfig = cfg;
            oldConfig.initUrl = this.config.initUrl;
            oldConfig.betPlatform = this.config.betPlatform;
            this.storage.set('config', oldConfig);
            this.presentToast("配置保存成功，但数据刚更新配置出现错误。错误信息:" + baseErrMsg);
        }).catch(err => {
            this.presentToast("配置保存失败,请联系管理员");
        });
    }

    //获取所有Tags
    getAllTags() {
        this.jpush
            .getAllTags({sequence: this.sequence++})
            .then(result => {
                var sequence: number = result.sequence;
                var tags: Array<string> = result.tags == null ? [] : result.tags;
                this.presentToast("注册标签组: " + tags.toString());
            })
            .catch(err => {
                this.presentToast("获取所有Tags失败" + "\nSequence: " + err.sequence + "\nCode: " + err.code);
            });
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
