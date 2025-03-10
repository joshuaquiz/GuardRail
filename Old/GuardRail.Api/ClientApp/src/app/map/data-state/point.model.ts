export class Point {
  constructor(
    public readonly X: number,
    public readonly Y: number) {
  }

  public toString(): string {
    return `{${this.X},${this.Y}}`;
  }
}