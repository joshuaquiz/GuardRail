import { Component } from '@angular/core';
import { PanService } from './pan.service';
import { PanDirection } from './pan-direction.model';
import { LogService } from '../../log.service';

@Component({
  selector: 'map-pan',
  templateUrl: './pan.component.html',
  styleUrls: ['./pan.component.css']
})
export class PanComponent {
  constructor(
    private readonly panService: PanService,
    private readonly logService: LogService) {
  }

  public panUp(): void {
    this.logService.debug('Pan up triggered');
    this.panService.pan(PanDirection.Up);
  }

  public panDown(): void {
    this.logService.debug('Pan down triggered');
    this.panService.pan(PanDirection.Down);
  }

  public panLeft(): void {
    this.logService.debug('Pan left triggered');
    this.panService.pan(PanDirection.Left);
  }

  public panRight(): void {
    this.logService.debug('Pan right triggered');
    this.panService.pan(PanDirection.Right);
  }
}
