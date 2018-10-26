import {Component} from '@angular/core';
import {Platform} from 'ionic-angular';
import {StatusBar} from '@ionic-native/status-bar';
import {SplashScreen} from '@ionic-native/splash-screen';
import {JPush} from '@jiguang-ionic/jpush';
import {BackgroundMode} from '@ionic-native/background-mode';
import {Autostart} from '@ionic-native/autostart';

import {TabsPage} from '../pages/tabs/tabs';

@Component({
    templateUrl: 'app.html'
})
export class AppComponent {
    rootPage: any = TabsPage;

    constructor(platform: Platform, statusBar: StatusBar, splashScreen: SplashScreen, jpush: JPush, autostart: Autostart, backgroundMode: BackgroundMode) {
        platform.ready().then(() => {
            // Okay, so the platform is ready and our plugins are available.
            // Here you can do any higher level native things you might need.

            statusBar.backgroundColorByHexString('#488aff');
            if (platform.is('android'))
            // statusBar.styleLightContent();
                statusBar.backgroundColorByHexString('#488aff');
            else
                statusBar.styleDefault();

            splashScreen.hide();

            //自启动
            autostart.enable();
            //后台运行
            backgroundMode.enable();
            //启动Jpush
            jpush.init();
        });
    }
}
