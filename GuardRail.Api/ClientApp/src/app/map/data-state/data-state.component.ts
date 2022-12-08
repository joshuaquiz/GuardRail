import { Component } from '@angular/core';
import { MapDataStateService } from './map-data-state.service';
import { LogService } from '../../log.service';

@Component({
  selector: 'map-data-state',
  templateUrl: './data-state.component.html',
  styleUrls: ['./data-state.component.css']
})
export class DataStateComponent {
  public LastSavedDate: string | null;
  public SavingError: string | null;
  public Saving: boolean;

  constructor(
    private readonly mapDataStateService: MapDataStateService,
    private readonly logService: LogService) {
    this.LastSavedDate = null;
    this.SavingError = null;
    this.Saving = false;
  }

  public undo(): void {
    this.logService.debug('Undo triggered');
    this.mapDataStateService.tryHistoryGoBack();
  }

  public redo(): void {
    this.logService.debug('Redo triggered');
    this.mapDataStateService.tryHistoryGoForward();
  }
}
