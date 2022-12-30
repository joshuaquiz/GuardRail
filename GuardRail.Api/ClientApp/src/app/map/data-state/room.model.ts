import { Point } from './point.model';

export class Room {
  public readonly Guid: string;

  public IsSelected = false;

  constructor(
    public readonly P1: Point,
    public readonly P2: Point) {
    this.Guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(
      /[xy]/g,
      c => {
        const r = Math.random() * 16 | 0;
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
      });
  }

  public toString(): string {
    return `{P1:${this.P1.toString()},P2:${this.P2.toString()},W:${this.P2.X - this.P1.X},H:${this.P2.Y - this.P1.Y}}`;
  }
}
