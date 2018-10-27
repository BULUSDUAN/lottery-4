import {NgModule, ErrorHandler} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {IonicApp, IonicModule, IonicErrorHandler} from 'ionic-angular';
import {AppComponent} from './app.component';
import {JPush} from '@jiguang-ionic/jpush';
import {Device} from "@ionic-native/device";
import {HTTP} from '@ionic-native/http';
import {IonicStorageModule} from '@ionic/storage';
// import {BackgroundMode} from '@ionic-native/background-mode';
import { Autostart } from '@ionic-native/autostart';

import {ContactPage} from '../pages/contact/contact';
import {SettingsPage} from '../pages/settings/settings';
import {BetPage} from '../pages/bet/bet';
import {DetailPage} from '../pages/home/detail';
import {HomePage} from '../pages/home/home';
import {TabsPage} from '../pages/tabs/tabs';

import {StatusBar} from '@ionic-native/status-bar';
import {SplashScreen} from '@ionic-native/splash-screen';

@NgModule({
    declarations: [
        AppComponent,
        SettingsPage,
        ContactPage,
        BetPage,
        DetailPage,
        HomePage,
        TabsPage
    ],
    imports: [
        BrowserModule,
        IonicModule.forRoot(AppComponent),
        IonicStorageModule.forRoot()
    ],
    bootstrap: [IonicApp],
    entryComponents: [
        AppComponent,
        SettingsPage,
        ContactPage,
        BetPage,
        DetailPage,
        HomePage,
        TabsPage
    ],
    providers: [
        StatusBar,
        SplashScreen,
        JPush,
        Device,
        HTTP,
        // BackgroundMode,
        Autostart,
        {provide: ErrorHandler, useClass: IonicErrorHandler}
    ]
})
export class AppModule {
}
