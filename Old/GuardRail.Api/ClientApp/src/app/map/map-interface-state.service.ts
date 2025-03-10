import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { IMapInterfaceState } from './map-interface-state.interface';
import { LogService } from '../log.service';

@Injectable({
  providedIn: 'root'
})
export class MapInterfaceStateService {
  private readonly storageKey = 'map-ui-state';

  private currentInterfaceState: MapInterfaceState;
  public CurrentInterfaceState: BehaviorSubject<IMapInterfaceState>;

  constructor(
    private readonly logService: LogService) {
    const storageStringValue = sessionStorage.getItem(this.storageKey);
    this.logService.debug('UI state string', storageStringValue);
    if (storageStringValue) {
      this.currentInterfaceState = new MapInterfaceState(storageStringValue);
    } else {
      this.currentInterfaceState = new MapInterfaceState();
    }

    this.CurrentInterfaceState = new BehaviorSubject(this.currentInterfaceState);
  }

  public updateOffsetX(deltaX: number): void {
    this.updateState(this.currentInterfaceState.updateOffsetX(deltaX));
  }

  public updateOffsetY(deltaY: number): void {
    this.updateState(this.currentInterfaceState.updateOffsetY(deltaY));
  }

  public updateScaleFactor(factor: number): void {
    this.updateState(this.currentInterfaceState.updateScaleFactor(factor));
  }

  private updateState(state: MapInterfaceState): void {
    this.logService.debug('UI state updated', JSON.stringify(state));
    sessionStorage.setItem(this.storageKey, JSON.stringify(state));
    this.currentInterfaceState = state;
    this.CurrentInterfaceState.next(this.currentInterfaceState);
  }
}

class MapInterfaceState implements IMapInterfaceState {
  public readonly OffsetX: number;
  public readonly OffsetY: number;
  public readonly ScaleFactor: number;

  constructor(...args: any[]) {
    if (args.length === 0) {
      this.OffsetX = 0;
      this.OffsetY = 0;
      this.ScaleFactor = 1;
    } else if (args.length === 1) {
      const jsonObj = JSON.parse(args[0]);
      this.OffsetX = jsonObj.OffsetX;
      this.OffsetY = jsonObj.OffsetY;
      this.ScaleFactor = jsonObj.ScaleFactor;
    } else if (args.length === 3) {
      [this.OffsetX, this.OffsetY, this.ScaleFactor] = args;
    } else {
      throw new Error('Invalid arguments for MapInterfaceState');
    }
  }

  public updateOffsetX(deltaX: number): IMapInterfaceState {
    return new MapInterfaceState(
      this.OffsetX + deltaX,
      this.OffsetY,
      this.ScaleFactor);
  }

  public updateOffsetY(deltaY: number): IMapInterfaceState {
    return new MapInterfaceState(
      this.OffsetX,
      this.OffsetY + deltaY,
      this.ScaleFactor);
  }

  public updateScaleFactor(factor: number): IMapInterfaceState {
    return new MapInterfaceState(
      this.OffsetX,
      this.OffsetY,
      factor);
  }
}
