import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { LogService } from '../../log.service';
import { Room } from './room.model';

@Injectable({
  providedIn: 'root'
})
export class MapDataStateService {
  private readonly historyDepth: number;
  private historyCurrentPosition: number;
  private readonly dataHistory: Array<Map<string, Room>>;
  public CurrentState: BehaviorSubject<Map<string, Room>>;

  constructor(
    private readonly logService: LogService) {
    this.historyDepth = 100;
    this.historyCurrentPosition = 0;
    this.dataHistory = [
      new Map<string, Room>()
    ];
    this.CurrentState = new BehaviorSubject(new Map<string, Room>());
  }

  public addRoom(room: Room): void {
    this.logService.debug('A room add was published', room);
    const newState = new Map<string, Room>(this.CurrentState.value);
    newState.set(room.Guid, room);
    this.updateState(newState);
  }

  public updateRoom(room: Room): void {
    this.logService.debug('A room update was published', room);
    const newState = new Map<string, Room>(this.CurrentState.value);
    newState.set(room.Guid, room);
    this.updateState(newState);
  }

  public setRoomAsSelected(id: string): void {
    this.logService.debug('A room was marked as selected', id);
    const room = this.CurrentState.value.get(id);
    if (room) {
      room.IsSelected = true;
    }

    this.CurrentState.next(this.CurrentState.value);
  }

  public unsetRoomAsSelected(id: string): void {
    this.logService.debug('A room was marked as unselected', id);
    const room = this.CurrentState.value.get(id);
    if (room) {
      room.IsSelected = false;
    }

    this.CurrentState.next(this.CurrentState.value);
  }

  public tryHistoryGoBack(): void {
    this.logService.debug('Attempting to go back in the history');
    if (this.historyCurrentPosition <= 0) {
      this.logService.debug('Back as far as we can go already');
      return;
    }

    this.historyCurrentPosition = this.historyCurrentPosition - 1;
    this.CurrentState.next(this.dataHistory[this.historyCurrentPosition]);
  }

  public tryHistoryGoForward(): void {
    this.logService.debug('Attempting to go forward in the history', this.dataHistory.length, this.historyCurrentPosition);
    if (this.historyCurrentPosition === -1 || (this.dataHistory.length - 1) <= this.historyCurrentPosition) {
      this.logService.debug('Forward as far as we can go already');
      return;
    }

    this.historyCurrentPosition = this.historyCurrentPosition + 1;
    this.CurrentState.next(this.dataHistory[this.historyCurrentPosition]);
  }

  private updateState(state: Map<string, Room>): void {
    this.logService.debug('Adding a new data state to the history', Array.from(state));
    if ((this.dataHistory.length -1) > this.historyCurrentPosition) {
      this.dataHistory.splice(this.historyCurrentPosition, this.dataHistory.length - this.historyCurrentPosition);
    }

    this.historyCurrentPosition = this.dataHistory.push(state) - 1;
    if (this.dataHistory.length > this.historyDepth) {
      this.dataHistory.shift();
    }

    this.CurrentState.next(state);
  }
}
