import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { PanDirection } from './pan-direction.model';
import { LogService } from '../../log.service';

@Injectable({
  providedIn: 'root'
})
export class PanService {
  public PanDirection: BehaviorSubject<PanDirection>;

  constructor(
    private readonly logService: LogService) {
    this.PanDirection = new BehaviorSubject<PanDirection>(PanDirection.None);
  }

  public pan(panDirection: PanDirection): void {
    this.logService.debug(`Publishing pan ${PanDirection[panDirection]}`);
    this.PanDirection.next(panDirection);
  }
}
