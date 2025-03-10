import { Component, HostListener, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { ZoomService } from './zoom.service';
import { ZoomSteps } from './zoom-steps.model';
import { LogService } from '../../log.service';

@Component({
  selector: 'map-zoom',
  templateUrl: './zoom.component.html',
  styleUrls: ['./zoom.component.css']
})
export class ZoomComponent implements OnInit {
  public ZoomName: string;
  public ZoomNumber: ZoomSteps;
  private readonly wheelEvent: BehaviorSubject<WheelEvent>;

  constructor(
    private readonly zoomService: ZoomService,
    private readonly logService: LogService) {
    this.ZoomNumber = ZoomSteps.Five;
    this.ZoomName = ZoomSteps[this.ZoomNumber];
    this.wheelEvent = new BehaviorSubject({} as WheelEvent);
  }

  public ngOnInit() {
    this.wheelEvent
      .pipe(debounceTime(250))
      .subscribe(x => {
        if (x && Math.abs(x.deltaY) > 0) {
          if (x.deltaY > 0) {
            this.zoomOut();
          } else {
            this.zoomIn();
          }
        }
      });

    this.zoomService.ZoomAmount
      .subscribe((x: ZoomSteps): void => {
        this.ZoomNumber = x;
        this.ZoomName = ZoomSteps[this.ZoomNumber].toLowerCase();
      });
  }

  public zoomOut(): void {
    this.logService.debug('Zoom out triggered');
    this.zoomService.zoomOut();
  }

  public zoomIn(): void {
    this.logService.debug('Zoom in triggered');
    this.zoomService.zoomIn();
  }

  public reset(): void {
    this.logService.debug('Zoom reset triggered');
    this.zoomService.resetZoom();
  }

  @HostListener('wheel', ['$event'])
  public onMouseWheel(event: WheelEvent) {
    this.logService.debug('Wheel event triggered', event);
    this.wheelEvent.next(event);
  }
}
