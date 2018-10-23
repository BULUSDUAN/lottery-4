import {Component} from '@angular/core';
import {NavParams} from 'ionic-angular';
import {DomSanitizer} from '@angular/platform-browser';

@Component({
    selector: 'page-bet',
    templateUrl: 'bet.html'
})
export class BetPage {
    url: string = "https://qxbet.com/wap#/lottery/index/20";
    srcUrl: any;

    constructor(private params: NavParams, private sanitizer: DomSanitizer) {
        this.srcUrl = this.sanitizer.bypassSecurityTrustResourceUrl(this.url);
        // let rule = this.params.get('rule');
    }

    loaded(iframe) {
        // console.log(iframe);
        let doc = iframe.contentDocument || iframe.contentWindow.document;
        let money = doc.querySelector('[placeholder="输入金额"]');
        money.setAttribute('value', 10);
    }
}
