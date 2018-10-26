import {Component} from '@angular/core';
import {ModalController} from 'ionic-angular';
import {SettingsPage} from "../settings/settings";

@Component({
    selector: 'page-contact',
    templateUrl: 'contact.html'
})
export class ContactPage {

    constructor(public modalCtrl: ModalController) {

    }

    viewSettingslModal() {
        let profileModal = this.modalCtrl.create(SettingsPage, {});
        profileModal.present();
    }
}
