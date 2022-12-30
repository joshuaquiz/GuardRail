import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { MouseModes } from './mouse-modes.model';
import { LogService } from '../../log.service';

@Injectable({
  providedIn: 'root'
})
export class MenuService {
  public MouseMode: BehaviorSubject<MouseModes>;

  constructor(
    private readonly logService: LogService) {
    this.MouseMode = new BehaviorSubject<MouseModes>(MouseModes.AddingRoom);
  }

  public setState(state: MouseModes): void {
    this.MouseMode.next(state);
  }
}
