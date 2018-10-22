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

  constructor(public alertCtrl: AlertController,
              public toastCtrl: ToastController,
              public jpush: JPush,
              private storage: Storage,
              ) {
    if (!this.config)
      this.config = {
        initUrl: "http://bet518.win",
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
        this.presentToast("配置保存错误，标签清理失败" + "\nSequence: " + err.sequence + "\nCode: " + err.code)
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
        this.presentToast("配置保存错误，标签设置失败" + "\nSequence: " + err.sequence + "\nCode: " + err.code)
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

  //用户登录
  logIn = function () {
    const prompt = this.alertCtrl.create({
      title: '登录',
      message: "使用设置中所选下注平台的账户进行登录。登录后才可使用下注功能",
      inputs: [
        {
          name: 'username',
          placeholder: '用户名'
        },
        {
          name: 'password',
          placeholder: '密码'
        }
      ],
      buttons: [
        {
          text: '取消',
          handler: data => {
            console.log('Cancel clicked');
          }
        },
        {
          text: '登录',
          handler: data => {
            console.log('Saved clicked');
          }
        }
      ]
    });
    prompt.present();
  }

}
