import {Component} from '@angular/core';
import {ToastController, ViewController, LoadingController} from 'ionic-angular';
import {JPush} from "@jiguang-ionic/jpush";
import {Storage} from '@ionic/storage';

@Component({
    selector: 'page-settings',
    templateUrl: 'settings.html'
})
export class SettingsPage {
    presentLoader: any;
    dismissLoader: any;
    sequence: number = 0;
    config: any;
    betPlatform: any[];
    oldConfig: any = {};

    constructor(public viewCtrl: ViewController,
                public toastCtrl: ToastController,
                public loadingCtrl: LoadingController,
                public jpush: JPush,
                private storage: Storage) {

        //进度条
        let loader;
        this.presentLoader = function () {
            loader = this.loadingCtrl.create({
                content: "加载中...",
            });
            loader.present();
        };
        this.dismissLoader = function () {
            loader.dismiss();
        };

        //默认配置
        this.betPlatform = [
            {name: '金皇朝3(9.56)', url: 'https://c.jhc3w.com/'},
            {name: '谦喜彩票(9.96)', url: 'https://qxbet.com/wap#/home'},
            {name: '大象彩(9.95)', url: 'https://da8088.com/mobile/#/home/'},
            {name: '鸿利彩票(9.95)', url: 'http://m.hl3999.com/'}
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

        //加载配置
        storage.get('config').then(cfg => {
            if (!cfg)
                storage.set('config', this.config);
            else
                this.config = cfg;

            for (let k in this.config)
                this.oldConfig[k] = this.config[k];

        }).catch(reason => {
            this.presentToast('抱歉，加载配置出现错误，请稍后重试');
            this.oldConfig = this.config;//引用复制，将不保存任何配置修改
        });
    }

    goBack() {
        this.viewCtrl.dismiss();
    }

    //保存配置
    saveSettings() {
        let jpushChanged = this.oldConfig.realTimeUpdate != this.config.realTimeUpdate
            || this.oldConfig.twoSide != this.config.twoSide
            || this.oldConfig.keepGuaAlert != this.config.keepGuaAlert
            || (this.config.keepGuaAlert && this.oldConfig.keepGuaCnt != this.config.keepGuaCnt)
            || this.oldConfig.repetitionAlert != this.config.repetitionAlert
            || (this.config.repetitionAlert && this.oldConfig.repetition != this.config.repetition);

        let baseChanged = this.oldConfig.initUrl != this.config.initUrl
            || this.oldConfig.betPlatform != this.config.betPlatform;

        if (!jpushChanged && baseChanged)
            this.saveBaseSettings(null);
        else if (jpushChanged)
            this.saveJpushSettings(baseChanged);
    }

    //保存基本配置
    saveBaseSettings(jpushErrMsg: string) {
        this.oldConfig.initUrl = this.config.initUrl;
        this.oldConfig.betPlatform = this.config.betPlatform;
        this.storage.set('config', this.oldConfig);

        let msg = "配置保存成功";
        msg += (!jpushErrMsg ? '' : '。"更新配置"出错,错误信息:' + jpushErrMsg);

        this.presentToast(msg);
    }

    //Jpush变更
    saveJpushSettings(baseSettingsChanged: boolean) {
        this.presentLoader();

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
                    this.dismissLoader();
                    this.presentToast("配置保存成功");
                    return;
                }

                //重新设置Tags
                this.setTags(tags, baseSettingsChanged);
            })
            .catch(err => {
                let msg = "Tag清理失败" + "\n错误码: " + err.code;
                this.dismissLoader();

                if (baseSettingsChanged)
                    this.saveBaseSettings(msg);
                else
                    this.presentToast("配置保存失败。" + msg);
            });
    }

    //覆盖设置Tags
    setTags(tagsName: string[], baseSettingsChanged: boolean) {
        this.jpush
            .setTags({sequence: this.sequence++, tags: tagsName})
            .then(result => {
                this.storage.set('config', this.config);
                this.dismissLoader();
                let tags: string[] = result.tags == null ? [] : result.tags;
                this.presentToast("配置保存成功" + "。注册标签组:\n" + this.getTagsName(tags));
            })
            .catch(err => {
                let msg = "Tag设置失败" + "\n错误码: " + err.code;
                this.dismissLoader();
                if (baseSettingsChanged)
                    this.saveBaseSettings(msg);
                else
                    this.presentToast("配置保存失败。" + msg);
            });
    }

    //获取所有Tags
    getAllTags() {
        this.presentLoader();
        this.jpush
            .getAllTags({sequence: this.sequence++})
            .then(result => {
                let tags: string[] = result.tags == null ? [] : result.tags;
                this.dismissLoader();
                this.presentToast("注册标签组: " + this.getTagsName(tags));
            })
            .catch(err => {
                this.dismissLoader();
                this.presentToast("查询标签组失败" + "\n错误码: " + err.code);
            });
    }

    getTagsName(tags: string[]) {
        let tagDict = {
            'realtime': '实时更新',
            'twoside': '两面盘',
            'liangua1': '1连挂',
            'liangua2': '2连挂',
            'liangua3': '3连挂',
            'liangua4': '4连挂',
            'repetition60': '60%重复',
            'repetition80': '80%重复',
            'repetition100': '100%重复'
        };
        let names = [];
        for (let i = 0; i < tags.length; i++)
            names.push(tagDict[tags[i]]);

        return names.toString();
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
