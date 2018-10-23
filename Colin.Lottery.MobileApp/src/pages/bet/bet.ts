import {Component} from '@angular/core';
import {NavParams} from 'ionic-angular';
import {DomSanitizer} from '@angular/platform-browser';
import {Storage} from '@ionic/storage';

@Component({
    selector: 'page-bet',
    templateUrl: 'bet.html'
})
export class BetPage {
    srcUrl: any;

    constructor(private params: NavParams, private storage: Storage, private sanitizer: DomSanitizer) {
        storage
            .get('config')
            .then(cfg => {
                let url = (!cfg || !cfg.betPlatform) ? 'https://qxbet.com/wap#/home' : cfg.betPlatform;
                this.srcUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
            })
            .catch(err => {
                alert('内容加载失败，请联系管理员');
            });
        // let rule = this.params.get('rule');
    }

    loaded(iframe) {
        let doc = iframe.contentDocument || iframe.contentWindow.document;
        let money = doc.querySelector('[placeholder="输入金额"]');
        // money.setAttribute('value', 10);//TODO:存在跨域访问问题
    }
}
