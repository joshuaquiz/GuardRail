import { Component, ElementRef, AfterViewInit, ViewChild } from '@angular/core';
import { fromEvent } from 'rxjs';
import { tap, switchMap, takeUntil, filter } from 'rxjs/operators'
import { ZoomService } from './zoom/zoom.service';
import { ZoomSteps } from './zoom/zoom-steps.model';
import { PanService } from './pan/pan.service';
import { PanDirection } from './pan/pan-direction.model';
import { MapInterfaceStateService } from './map-interface-state.service';
import { MapDataStateService } from './data-state/map-data-state.service';
import { LogService } from '../log.service';
import { IMapInterfaceState } from './map-interface-state.interface';
import { Point } from './data-state/point.model';
import { Room } from './data-state/room.model';
import { MouseModes } from './menu/mouse-modes.model';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements AfterViewInit {
  public MenuOpen: boolean;
  public PanStepAmount: number;
  public GridSpacing: number;

  public Mode: MouseModes | undefined;
  private boundingRect: DOMRect | undefined;

  @ViewChild('map')
  public Canvas: ElementRef | undefined;

  private canvasRenderingContext: CanvasRenderingContext2D | null | undefined;

  constructor(
    private readonly zoomService: ZoomService,
    private readonly panService: PanService,
    private readonly mapInterfaceStateService: MapInterfaceStateService,
    private readonly mapDataStateService: MapDataStateService,
    private readonly logService: LogService) {
    this.MenuOpen = false;
    this.PanStepAmount = 125;
    this.GridSpacing = 50;
    this.setupInterfaceState();
    this.setupZoomService();
    this.setupPanService();
    this.setupDataStateService();
  }

  public setupInterfaceState(): void {
    this.mapInterfaceStateService.CurrentInterfaceState
      .subscribe((x: IMapInterfaceState): void => {
        this.logService.debug('Map interface state updated', x);
        if (this.canvasRenderingContext) {
          this.renderGridAndCurrentRooms(
            this.mapDataStateService.CurrentState.value,
            this.canvasRenderingContext,
            this.mapInterfaceStateService.CurrentInterfaceState.value);
        }
      });
  }

  private setupZoomService(): void {
    this.zoomService.ZoomAmount
      .subscribe((x: ZoomSteps): void => {
        this.mapInterfaceStateService.updateScaleFactor(x);
        this.logService.debug(`Set zoom offset to ${ZoomSteps[x]} with value ${x}`);
        this.renderGridAndCurrentRooms(
          this.mapDataStateService.CurrentState.value,
          this.canvasRenderingContext,
          this.mapInterfaceStateService.CurrentInterfaceState.value);
      });
  }

  private setupPanService(): void {
    this.panService.PanDirection
      .subscribe((x: PanDirection): void => {
        switch (x) {
        case PanDirection.Up:
          this.mapInterfaceStateService.updateOffsetY(this.PanStepAmount);
          this.logService.debug(`Panned up ${this.PanStepAmount} to ${this.mapInterfaceStateService.CurrentInterfaceState.value.OffsetY}`);
          break;
        case PanDirection.Down:
          this.mapInterfaceStateService.updateOffsetY(-this.PanStepAmount);
          this.logService.debug(`Panned down ${this.PanStepAmount} to ${this.mapInterfaceStateService.CurrentInterfaceState.value.OffsetY}`);
          break;
        case PanDirection.Left:
          this.mapInterfaceStateService.updateOffsetX(this.PanStepAmount);
          this.logService.debug(`Panned left ${this.PanStepAmount} to ${this.mapInterfaceStateService.CurrentInterfaceState.value.OffsetX}`);
          break;
        case PanDirection.Right:
          this.mapInterfaceStateService.updateOffsetX(-this.PanStepAmount);
          this.logService.debug(`Panned right ${this.PanStepAmount} to ${this.mapInterfaceStateService.CurrentInterfaceState.value.OffsetX}`);
          break;
        }

        this.renderGridAndCurrentRooms(
          this.mapDataStateService.CurrentState.value,
          this.canvasRenderingContext,
          this.mapInterfaceStateService.CurrentInterfaceState.value);
      });
  }

  private setupDataStateService(): void {
    this.mapDataStateService.CurrentState
      .subscribe((x: Map<string, Room>): void => {
        this.logService.debug('new state', Array.from(x));
        this.renderGridAndCurrentRooms(
          x,
          this.canvasRenderingContext,
          this.mapInterfaceStateService.CurrentInterfaceState.value);
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
      this.mapDataStateService.CurrentState.value,
      this.canvasRenderingContext,
      this.mapInterfaceStateService.CurrentInterfaceState.value);
  }

  private roundToNearest(numToRound: number, numToRoundTo: number): number {
    return Math.round(numToRound / numToRoundTo) * numToRoundTo;
  }

  private captureEvents(canvasEl: HTMLCanvasElement) {
    var startPos: Point;
    fromEvent(canvasEl, 'mousedown')
      .pipe(
        filter(x => this.Mode === MouseModes.AddingRoom),
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
      if (Math.abs(startPos.X - currentPos.X) < 5
        || Math.abs(startPos.Y - currentPos.Y) < 5) {
        this.logService.debug('The box was to small');
      } else {
        this.mapDataStateService.addRoom(
          new Room(
            startPos,
            currentPos));
      }
    }

    this.renderGridAndCurrentRooms(
      this.mapDataStateService.CurrentState.value,
      this.canvasRenderingContext!,
      this.mapInterfaceStateService.CurrentInterfaceState.value);
    if (m1.type !== 'mouseup') {
      const tempRoom = new Room(
        startPos,
        currentPos);
      this.renderRoom(
        tempRoom,
        this.canvasRenderingContext!,
        this.mapInterfaceStateService.CurrentInterfaceState.value);
    }
  }

  private renderGridAndCurrentRooms(
    rooms: Map<string, Room>,
    canvasContext: CanvasRenderingContext2D | null | undefined,
    mapInterfaceState: IMapInterfaceState): void {
    if (!canvasContext) {
      return;
    }

    canvasContext.clearRect(0, 0, Number.MAX_VALUE, Number.MAX_VALUE);
    this.renderGrid(
      canvasContext,
      mapInterfaceState);
    rooms.forEach(x => this.renderRoom(x, canvasContext, mapInterfaceState));
  }

  private renderGrid(
    ctx: CanvasRenderingContext2D,
    mapInterfaceState: IMapInterfaceState): void {
    ctx.strokeStyle = '#ddd';
    const lines = new Array<{ Start: Point, End: Point }>();
    const xStart = mapInterfaceState.OffsetX > 0
      ? -mapInterfaceState.OffsetX
      : mapInterfaceState.OffsetX;
    for (let i = xStart; i < 80; i++) {
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
    for (let i = yStart; i < 80; i++) {
      lines.push({
        Start: new Point(
          0,
          (i * this.GridSpacing) + mapInterfaceState.OffsetY),
        End: new Point(
          10000,
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
    const numberFormat = new Intl.NumberFormat('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 2 });
    canvasContext.fillText(
      `${numberFormat.format(Math.abs(room.P2.X - room.P1.X) / 2)}ft`,
      Math.floor(p1X + (width / 2)),
      Math.floor((room.P1.Y + 12 - mapInterfaceState.OffsetY) * mapInterfaceState.ScaleFactor));
    canvasContext.stroke();
    canvasContext.save();
    canvasContext.translate(
      Math.floor(p1X) + 3,
      Math.floor(p1Y + (height / 2)));
    canvasContext.rotate(Math.PI / 2);
    canvasContext.fillText(
      `${numberFormat.format(Math.abs(room.P2.Y - room.P1.Y) / 2)}ft`,
      0,
      0);
    canvasContext.stroke();
    canvasContext.restore();
  }
}
