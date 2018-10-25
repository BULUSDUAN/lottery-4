import {Component} from '@angular/core';
import {Platform} from 'ionic-angular';
import {StatusBar} from '@ionic-native/status-bar';
import {SplashScreen} from '@ionic-native/splash-screen';
import {JPush} from '@jiguang-ionic/jpush';

import {TabsPage} from '../pages/tabs/tabs';

@Component({
    templateUrl: 'app.html'
})
export class AppComponent {
    rootPage: any = TabsPage;

    constructor(platform: Platform, statusBar: StatusBar, splashScreen: SplashScreen, jpush: JPush) {
        platform.ready().then(() => {
            // Okay, so the platform is ready and our plugins are available.
            // Here you can do any higher level native things you might need.
            if (platform.is('android'))
                statusBar.styleLightContent();
            else
                statusBar.styleDefault();

            splashScreen.hide();

            jpush.init();
            // jpush.setDebugMode(true);
        });
    }
}
