import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ZoomSteps } from './zoom-steps.model';
import { LogService } from '../../log.service';

@Injectable({
  providedIn: 'root'
})
export class ZoomService {
  public ZoomAmount: BehaviorSubject<ZoomSteps>;
  private startingZoomStepIndex = 4;
  private zoomStepIndex: number;
  private zoomStepArray = [
    ZoomSteps.One,
    ZoomSteps.Two,
    ZoomSteps.Three,
    ZoomSteps.Four,
    ZoomSteps.Five,
    ZoomSteps.Six,
    ZoomSteps.Seven,
    ZoomSteps.Eight,
    ZoomSteps.Nine
  ];

  constructor(
    private readonly logService: LogService) {
    this.zoomStepIndex = this.startingZoomStepIndex;
    this.ZoomAmount = new BehaviorSubject(this.zoomStepArray[this.zoomStepIndex]);
  }

  public zoomIn(): void {
    let zoomScale = this.zoomStepIndex - 1;
    if (zoomScale < 0) {
      this.logService.debug('Zoom out attempted past min allowed value, defaulting to min zoom');
      zoomScale = 0;
    }

    this.setZoom(zoomScale);
  }

  public zoomOut(): void {
    let zoomScale = this.zoomStepIndex + 1;
    if (zoomScale >= this.zoomStepArray.length) {
      this.logService.debug('Zoom out attempted past max allowed value, defaulting to max zoom');
      zoomScale = this.zoomStepArray.length - 1;
    }

    this.setZoom(zoomScale);
  }

  public resetZoom(): void {
    this.setZoom(this.startingZoomStepIndex);
  }

  private setZoom(zoomStepIndex: number): void {
    this.logService.debug(`Publishing zoom, index: ${zoomStepIndex}, value: ${this.zoomStepArray[zoomStepIndex]}, name: ${ZoomSteps[this.zoomStepArray[this.zoomStepIndex]]}`);
    this.zoomStepIndex = zoomStepIndex;
    this.ZoomAmount.next(this.zoomStepArray[this.zoomStepIndex]);
  }
}
