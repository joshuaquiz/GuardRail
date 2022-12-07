export interface IMapInterfaceState {
  readonly OffsetX: number;

  readonly OffsetY: number;

  readonly ScaleFactor: number;

  updateOffsetX(deltaX: number): IMapInterfaceState;

  updateOffsetY(deltaY: number): IMapInterfaceState;

  updateScaleFactor(factor: number): IMapInterfaceState;
}
