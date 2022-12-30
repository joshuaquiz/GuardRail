import { Component } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { MouseModes } from './mouse-modes.model';
import { MenuService } from './menu.service'
import { LogService } from '../../log.service';

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

  constructor(
    private readonly menuService: MenuService,
    private readonly logService: LogService) {
  }

  public setModeAddRoom(): void {
    this.menuService.setState(MouseModes.AddingRoom);
  }

  public setModeAddCamera(): void {
    this.menuService.setState(MouseModes.AddingCamera);
  }

  public setModeDelete(): void {
    this.menuService.setState(MouseModes.Deleting);
  }
}
