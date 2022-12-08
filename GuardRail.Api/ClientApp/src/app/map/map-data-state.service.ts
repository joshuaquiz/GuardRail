import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { LogService } from '../log.service';
import { Room } from './room.model';

@Injectable({
  providedIn: 'root'
})
export class MapDataStateService {
  private readonly historyDepth: number;
  private historyCurrentPosition: number;
  private readonly dataHistory: Array<Map<string, Room>>;
  private currentState: Map<string, Room>;
  public CurrentState: BehaviorSubject<Map<string, Room>>;

  constructor(
    private readonly logService: LogService) {
    this.historyDepth = 100;
    this.historyCurrentPosition = 0;
    this.dataHistory = new Array<Map<string, Room>>();
    this.currentState = new Map<string, Room>();
    this.CurrentState = new BehaviorSubject(this.currentState);
  }

  public addRoom(room: Room): void {
    this.logService.debug('A room add was published', room);
    const copy = Object.assign({}, this.currentState);
    copy.set(room.Id, room);
    this.updateState(copy);
  }

  public updateRoom(room: Room): void {
    this.logService.debug('A room update was published', room);
    const copy = Object.assign({}, this.currentState);
    copy.set(room.Id, room);
    this.updateState(copy);
  }

  public setRoomAsSelected(id: string): void {
    this.logService.debug('A room was marked as selected', id);
    const room = this.currentState.get(id);
    if (room) {
      room.IsSelected = true;
    }

    this.CurrentState.next(this.currentState);
  }

  public unsetRoomAsSelected(id: string): void {
    this.logService.debug('A room was marked as unselected', id);
    const room = this.currentState.get(id);
    if (room) {
      room.IsSelected = false;
    }

    this.CurrentState.next(this.currentState);
  }

  public getCurrentState(): Map<string, Room> {
    return this.currentState;
  }

  public tryHistoryGoBack(): void {
    this.logService.debug('Attempting to go back in the history');
    if (this.historyCurrentPosition === 0) {
      return;
    }

    this.historyCurrentPosition = this.historyCurrentPosition - 1;
    this.currentState = this.dataHistory[this.historyCurrentPosition];
    this.CurrentState.next(this.currentState);
  }

  public tryHistoryGoForward(): void {
    this.logService.debug('Attempting to go forward in the history');
    if (this.dataHistory.length <= this.historyCurrentPosition) {
      return;
    }

    this.historyCurrentPosition = this.historyCurrentPosition + 1;
    this.currentState = this.dataHistory[this.historyCurrentPosition];
    this.CurrentState.next(this.currentState);
  }

  private updateState(state: Map<string, Room>): void {
    if (this.dataHistory.length > this.historyCurrentPosition) {
      this.dataHistory.splice(this.historyCurrentPosition, this.dataHistory.length - this.historyCurrentPosition);
    }

    this.historyCurrentPosition = this.historyCurrentPosition + 1;
    this.dataHistory.push(state);
    if (this.dataHistory.length > this.historyDepth) {
      this.dataHistory.shift();
    }

    this.currentState = state;
    this.CurrentState.next(this.currentState);
  }
}
