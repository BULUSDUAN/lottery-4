import {Component} from '@angular/core';

import {ContactPage} from '../contact/contact';
import {BetPage} from '../bet/bet';
import {HomePage} from '../home/home';

@Component({
    templateUrl: 'tabs.html'
})
export class TabsPage {

    homeRoot = HomePage;
    betRoot = BetPage;
    contactRoot = ContactPage;

    constructor() {

    }
}
