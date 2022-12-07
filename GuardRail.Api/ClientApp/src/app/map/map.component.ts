import { Component, ElementRef, AfterViewInit, ViewChild } from '@angular/core';
import { fromEvent } from 'rxjs';
import { tap, switchMap, takeUntil, filter } from 'rxjs/operators'
import { ZoomService } from './zoom/zoom.service';
import { ZoomSteps } from './zoom/zoom-steps.model';
import { PanService } from './pan/pan.service';
import { PanDirection } from './pan/pan-direction.model';
import { MapInterfaceStateService } from './map-interface-state.service';
import { LogService } from '../log.service';
import { IMapInterfaceState } from './map-interface-state.interface';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements AfterViewInit {
  public MenuOpen = false;
  public PanStepAmount = 100;
  public Rooms = new Array<Room>();

  private boundingRect: DOMRect | undefined;

  @ViewChild('map')
  public Canvas: ElementRef | undefined;

  private canvasRenderingContext: CanvasRenderingContext2D | null | undefined;

  constructor(
    private readonly zoomService: ZoomService,
    private readonly panService: PanService,
    private readonly mapInterfaceStateService: MapInterfaceStateService,
    private readonly logService: LogService) {
    this.setupInterfaceState();
    this.setupZoomService();
    this.setupPanService();
  }

  public setupInterfaceState(): void {
    this.mapInterfaceStateService.updateOffsetX(0);
    this.mapInterfaceStateService.updateOffsetY(0);
    this.mapInterfaceStateService.CurrentInterfaceState
      .subscribe((x: IMapInterfaceState): void => {
        if (this.canvasRenderingContext) {
          this.render(
            this.canvasRenderingContext!,
            x);
        }
      });
  }

  private setupZoomService(): void {
    this.zoomService.ZoomAmount
      .subscribe((x: ZoomSteps): void => {
        this.mapInterfaceStateService.updateScaleFactor(x);
        this.logService.debug(`Set zoom offset to ${ZoomSteps[x]} with value ${x}`);
      });
  }

  private setupPanService(): void {
    this.panService.PanDirection
      .subscribe((x: PanDirection): void => {
        switch (x) {
        case PanDirection.Up:
          this.mapInterfaceStateService.updateOffsetY(this.PanStepAmount);
          this.logService.debug(`Panned up ${this.PanStepAmount} to ${this.mapInterfaceStateService.getOffsetY()}`);
          break;
        case PanDirection.Down:
          this.mapInterfaceStateService.updateOffsetY(-this.PanStepAmount);
          this.logService.debug(`Panned down ${this.PanStepAmount} to ${this.mapInterfaceStateService.getOffsetY()}`);
          break;
        case PanDirection.Left:
          this.mapInterfaceStateService.updateOffsetX(this.PanStepAmount);
          this.logService.debug(`Panned left ${this.PanStepAmount} to ${this.mapInterfaceStateService.getOffsetX()}`);
          break;
        case PanDirection.Right:
          this.mapInterfaceStateService.updateOffsetX(-this.PanStepAmount);
          this.logService.debug(`Panned right ${this.PanStepAmount} to ${this.mapInterfaceStateService.getOffsetX()}`);
          break;
        }
      });
  }

  public ngAfterViewInit() {
    const canvasEl: HTMLCanvasElement = this.Canvas?.nativeElement;
    this.canvasRenderingContext = canvasEl.getContext('2d');

    this.boundingRect = canvasEl.getBoundingClientRect();
    canvasEl.width = this.boundingRect.width;
    canvasEl.height = this.boundingRect.height;


    if (this.canvasRenderingContext) {
      this.canvasRenderingContext.lineWidth = 3;
      this.canvasRenderingContext.strokeStyle = '#000';
    }

    this.captureEvents(canvasEl);
  }

  private roundToNearest(numToRound: number, numToRoundTo: number): number {
    return Math.round(numToRound / numToRoundTo) * numToRoundTo;
  }

  private captureEvents(canvasEl: HTMLCanvasElement) {
    // this will capture all mousedown events from the canvas element
    var start: MouseEvent;
    fromEvent(canvasEl, 'mousedown')
      .pipe(
        tap(x => start = x as MouseEvent),
        filter(x => (x as MouseEvent).button === 0),
        tap(x => console.log((x as MouseEvent).button)),
        switchMap(() =>
          // after a mouse down, we'll record all mouse moves
          fromEvent(canvasEl, 'mousemove')
            .pipe(
              // we'll stop (and unsubscribe) once the user releases the mouse
              // this will trigger a 'mouseup' event
              takeUntil(fromEvent(canvasEl, 'mouseup')),
              // we'll also stop (and unsubscribe) once the mouse leaves the canvas (mouseleave event)
              takeUntil(fromEvent(canvasEl, 'mouseleave'))
            ))
      )
      .subscribe(res => {
        console.log(res, this.boundingRect);
        if (!this.boundingRect || !this.canvasRenderingContext) {
          return;
        }

        const m1 = res as MouseEvent;
        if (start && m1) {
          const startPos = {
            X: this.roundToNearest(start.clientX - this.boundingRect.left, 10),
            Y: this.roundToNearest(start.clientY - this.boundingRect.top, 10)
          };

          const currentPos = {
            X: this.roundToNearest(m1.clientX - this.boundingRect.left, 10),
            Y: this.roundToNearest(m1.clientY - this.boundingRect.top, 10)
          };

          // this method we'll implement soon to do the actual drawing
          this.drawOnCanvas(startPos, currentPos);
          this.canvasRenderingContext.beginPath();
          this.canvasRenderingContext.font = '20px serif';
          this.canvasRenderingContext.fillText(`Start:(${start.clientX},${start.clientY}),m1:(${m1.clientX},${m1.clientY})`, m1.clientX - this.boundingRect.left, m1.clientY - this.boundingRect.top);
          this.canvasRenderingContext.stroke();
        }
      });
    fromEvent(canvasEl, 'mouseup')
      .subscribe(res => {
        console.log(res);
        if (!start || !this.boundingRect) {
          return;
        }

        const m1 = res as MouseEvent;
        if (start && m1) {
          const startPos = {
            X: this.roundToNearest(start.clientX - this.boundingRect.left, 10),
            Y: this.roundToNearest(start.clientY - this.boundingRect.top, 10)
          };

          const currentPos = {
            X: this.roundToNearest(m1.clientX - this.boundingRect.left, 10),
            Y: this.roundToNearest(m1.clientY - this.boundingRect.top, 10)
          };

          if (Math.abs(startPos.X - currentPos.X) < 10
            || Math.abs(startPos.Y - currentPos.Y) < 10) {
            console.log('too small');
            this.drawOnCanvas(null, null);
            return;
          }

          console.log(startPos, currentPos);

          this.Rooms.push(
            new Room(
              startPos.X,
              startPos.Y,
              currentPos.X,
              currentPos.Y));

          // this method we'll implement soon to do the actual drawing
          this.drawOnCanvas(null, null);
        }
      });
  }

  private drawOnCanvas(prevPos: { X: number, Y: number } | null, currentPos: { X: number, Y: number } | null): void {
    if (!this.canvasRenderingContext) {
      return;
    }

    this.canvasRenderingContext.clearRect(0, 0, Number.MAX_VALUE, Number.MAX_VALUE); //clear canvas

    for (let i = 0; i < this.Rooms.length; i++) {
      this.Rooms[i].render(this.canvasRenderingContext, null);
    }

    if (!prevPos || !currentPos) {
      return;
    }

    const tempRoom = new Room(
      prevPos.X,
      prevPos.Y,
      currentPos.X,
      currentPos.Y);
    tempRoom.render(this.canvasRenderingContext, null);
  }

  private render(
    canvasContext: CanvasRenderingContext2D,
    mapInterfaceState: IMapInterfaceState): void {
    canvasContext.clearRect(0, 0, Number.MAX_VALUE, Number.MAX_VALUE);
    this.renderGrid(
      canvasContext,
      mapInterfaceState);
    for (let i = 0; i < this.Rooms.length; i++) {
      this.Rooms[i].render(canvasContext, mapInterfaceState);
    }
  }

  private renderGrid(
    ctx: CanvasRenderingContext2D,
    mapInterfaceState: IMapInterfaceState): void {
    ctx.strokeStyle = '#ddd';
    for (let i = 0; i < 50; i++) {
      ctx.beginPath();
      ctx.moveTo((i * 100) + mapInterfaceState.OffsetX, 0);
      ctx.lineTo((i * 100) + mapInterfaceState.OffsetX, 1000);
      ctx.stroke();
    }

    for (let i = 0; i < 50; i++) {
      ctx.beginPath();
      ctx.moveTo(0, (i * 100) + mapInterfaceState.OffsetY);
      ctx.lineTo(2000, (i * 100) + mapInterfaceState.OffsetY);
      ctx.stroke();
    }

    ctx.strokeStyle = '#000';
  }
}

class Room {
  constructor(
    public X1: number,
    public Y1: number,
    public X2: number,
    public Y2: number) {
  }

  public render(
    canvasContext: CanvasRenderingContext2D,
    mapInterfaceState: IMapInterfaceState | null): void {
    canvasContext.beginPath();
    canvasContext.rect(
      this.X1,
      this.Y1,
      this.X2 - this.X1,
      this.Y2 - this.Y1);
    canvasContext.font = '20px serif';
    canvasContext.fillText(this.toString(), this.X1 + 3, this.Y1 + 20);
    canvasContext.stroke();
  }

  public toString(): string {
    return `{P1:{${this.X1},${this.Y1}},P2:{${this.X2},${this.Y2}},W:${this.X2 - this.X1},H:${this.Y2 - this.Y1}}`;
  }
}
