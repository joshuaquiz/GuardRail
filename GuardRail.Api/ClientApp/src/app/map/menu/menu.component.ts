import { Component } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';

@Component({
  selector: 'map-menu',
  animations: [
    trigger('openClose',
      [
        state('open',
          style({
            height: '215px'
          })),
        state('closed',
          style({
            height: '62px'
          })),
        transition('open => closed',
          [
            animate('0.3s')
          ]),
        transition('closed => open',
          [
            animate('0.3s')
          ])
      ])
  ],
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent {
  public MenuOpen = false;
}
