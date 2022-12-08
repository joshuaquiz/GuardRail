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
import { Point } from './point.model';
import { Room } from './room.model';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements AfterViewInit {
  public MenuOpen = false;
  public PanStepAmount = 125;
  public GridSpacing = 50;
  public Rooms = new Array<Room>();
  private mapInterfaceState = { OffsetX: 0, OffsetY: 0, ScaleFactor: 1 } as IMapInterfaceState;

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
    this.mapInterfaceStateService.CurrentInterfaceState
      .subscribe((x: IMapInterfaceState): void => {
        this.logService.debug('Map interface state updated', x);
        if (this.canvasRenderingContext) {
          this.mapInterfaceState = x;
          this.renderGridAndCurrentRooms(
            this.canvasRenderingContext,
            this.mapInterfaceState);
        }
      });
  }

  private setupZoomService(): void {
    this.zoomService.ZoomAmount
      .subscribe((x: ZoomSteps): void => {
        this.mapInterfaceStateService.updateScaleFactor(x);
        this.logService.debug(`Set zoom offset to ${ZoomSteps[x]} with value ${x}`);
        this.renderGridAndCurrentRooms(
          this.canvasRenderingContext,
          this.mapInterfaceState);
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

        this.renderGridAndCurrentRooms(
          this.canvasRenderingContext,
          this.mapInterfaceState);
      });
  }

  public ngAfterViewInit() {
    const canvasEl: HTMLCanvasElement = this.Canvas?.nativeElement;
    this.canvasRenderingContext = canvasEl.getContext('2d');

    this.boundingRect = canvasEl.getBoundingClientRect();
    canvasEl.width = this.boundingRect.width;
    canvasEl.height = this.boundingRect.height;

    if (this.canvasRenderingContext) {
      this.canvasRenderingContext.lineWidth = 2;
      this.canvasRenderingContext.strokeStyle = '#000';
      this.canvasRenderingContext.font = '15px';
      this.canvasRenderingContext.textAlign = 'center';
    }

    this.captureEvents(canvasEl);
    this.renderGridAndCurrentRooms(
      this.canvasRenderingContext,
      this.mapInterfaceState);
  }

  private roundToNearest(numToRound: number, numToRoundTo: number): number {
    return Math.round(numToRound / numToRoundTo) * numToRoundTo;
  }

  private captureEvents(canvasEl: HTMLCanvasElement) {
    var startPos: Point;
    fromEvent(canvasEl, 'mousedown')
      .pipe(
        tap(x => {
          const start = x as MouseEvent;
          startPos = new Point(
            this.roundToNearest(start.clientX - this.boundingRect!.left, 10),
            this.roundToNearest(start.clientY - this.boundingRect!.top, 10));
        }),
        filter(x => (x as MouseEvent).button === 0),
        switchMap(() =>
          fromEvent(canvasEl, 'mousemove')
            .pipe(
              takeUntil(fromEvent(canvasEl, 'mouseup')),
              takeUntil(fromEvent(canvasEl, 'mouseleave'))
            ))
      )
      .subscribe(x => this.handleMouseUpAndMove(x, startPos));
    fromEvent(canvasEl, 'mouseup')
      .subscribe(x => this.handleMouseUpAndMove(x, startPos));
  }

  private handleMouseUpAndMove(
    res: Event,
    startPos: Point): void {
    const m1 = res as MouseEvent;
    const currentPos = new Point(
      this.roundToNearest(m1.clientX - this.boundingRect!.left, 10),
      this.roundToNearest(m1.clientY - this.boundingRect!.top, 10));

    if (m1.type === 'mouseup') {
      if (Math.abs(startPos.X - currentPos.X) < 10 || Math.abs(startPos.Y - currentPos.Y) < 10) {
        this.logService.debug('The box was to small');
      } else {
        this.Rooms.push(
          new Room(
            startPos,
            currentPos));
      }
    }

    this.renderGridAndCurrentRooms(
      this.canvasRenderingContext!,
      this.mapInterfaceState);

    if (m1.type !== 'mouseup') {
      const tempRoom = new Room(
        startPos,
        currentPos);
      this.renderRoom(
        tempRoom,
        this.canvasRenderingContext!,
        this.mapInterfaceState);
    }
  }

  private renderGridAndCurrentRooms(
    canvasContext: CanvasRenderingContext2D | null | undefined,
    mapInterfaceState: IMapInterfaceState): void {
    if (!canvasContext) {
      return;
    }

    canvasContext.clearRect(0, 0, Number.MAX_VALUE, Number.MAX_VALUE);
    this.renderGrid(
      canvasContext,
      mapInterfaceState);
    for (let i = 0; i < this.Rooms.length; i++) {
      this.renderRoom(
        this.Rooms[i],
        canvasContext,
        mapInterfaceState);
    }
  }

  private renderGrid(
    ctx: CanvasRenderingContext2D,
    mapInterfaceState: IMapInterfaceState): void {
    ctx.strokeStyle = '#ddd';
    const lines = new Array<{ Start: Point, End: Point }>();
    const xStart = mapInterfaceState.OffsetX > 0
      ? -mapInterfaceState.OffsetX
      : mapInterfaceState.OffsetX;
    for (let i = xStart; i < 50; i++) {
      lines.push({
        Start: new Point(
          (i * this.GridSpacing) + mapInterfaceState.OffsetX,
          0),
        End: new Point(
          (i * this.GridSpacing) + mapInterfaceState.OffsetX,
          1000)
      });
    }

    const yStart = mapInterfaceState.OffsetY > 0
      ? -mapInterfaceState.OffsetY
      : mapInterfaceState.OffsetY;
    for (let i = yStart; i < 50; i++) {
      lines.push({
        Start: new Point(
          0,
          (i * this.GridSpacing) + mapInterfaceState.OffsetY),
        End: new Point(
          5000,
          (i * this.GridSpacing) + mapInterfaceState.OffsetY)
      });
    }

    for (let i = 0; i < lines.length; i++) {
      ctx.beginPath();
      ctx.moveTo(lines[i].Start.X, lines[i].Start.Y);
      ctx.lineTo(lines[i].End.X, lines[i].End.Y);
      ctx.stroke();
    }

    ctx.strokeStyle = '#000';
  }

  private renderRoom(
    room: Room,
    canvasContext: CanvasRenderingContext2D,
    mapInterfaceState: IMapInterfaceState): void {
    canvasContext.beginPath();
    const p1X = (room.P1.X - mapInterfaceState.OffsetX) * mapInterfaceState.ScaleFactor;
    const width = (room.P2.X - room.P1.X) * mapInterfaceState.ScaleFactor;
    const p1Y = (room.P1.Y - mapInterfaceState.OffsetY) * mapInterfaceState.ScaleFactor;
    const height = (room.P2.Y - room.P1.Y) * mapInterfaceState.ScaleFactor;
    canvasContext.rect(
      Math.floor(p1X),
      Math.floor(p1Y),
      Math.floor(width),
      Math.floor(height));
    canvasContext.fillText(
      `${Math.abs(room.P2.X - room.P1.X)}ft`,
      Math.floor(p1X + (width / 2)),
      Math.floor((room.P1.Y + 12 - mapInterfaceState.OffsetY) * mapInterfaceState.ScaleFactor));
    canvasContext.stroke();
    canvasContext.save();
    canvasContext.translate(
      Math.floor(p1X) + 3,
      Math.floor(p1Y + (height / 2)));
    canvasContext.rotate(Math.PI / 2);
    canvasContext.fillText(
      `${Math.abs(room.P2.Y - room.P1.Y)}ft`,
      0,
      0);
    canvasContext.stroke();
    canvasContext.restore();
  }
}
