import {Component} from '@angular/core';
import {LoadingController} from 'ionic-angular';
import {DomSanitizer} from '@angular/platform-browser';
import {Storage} from '@ionic/storage';

@Component({
    selector: 'page-bet',
    templateUrl: 'bet.html'
})
export class BetPage {
    dismissLoader: any;
    ready: boolean = false;
    srcUrl: any;

    constructor(private loadingCtrl: LoadingController, private storage: Storage, private sanitizer: DomSanitizer) {
        this.load();
    }


    ionViewWillEnter() {
        if (!this.ready)
            return;

        //检查配置是否变更
        this.storage
            .get('config')
            .then(cfg => {
                let url = (!cfg || !cfg.betPlatform) ? 'https://qxbet.com/wap#/home' : cfg.betPlatform;
                if (this.srcUrl.changingThisBreaksApplicationSecurity.indexOf(url) >= 0)
                    return;

                this.loader();
                this.srcUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
            })
            .catch(err => {
                alert('读取配置失败，请联系管理员');
            });
    }

    load() {
        this.loader();
        this.storage
            .get('config')
            .then(cfg => {
                let url = (!cfg || !cfg.betPlatform) ? 'https://qxbet.com/wap#/home' : cfg.betPlatform;
                this.srcUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
            })
            .catch(err => {
                alert('内容加载失败，请联系管理员');
            });
    }

    loaded() {
        this.dismissLoader();
        this.ready = true;
    }

    loader() {
        const loader = this.loadingCtrl.create({
            content: "加载中...",
        });
        loader.present();
        this.dismissLoader = function () {
            loader.dismiss();
        }
    }
}
