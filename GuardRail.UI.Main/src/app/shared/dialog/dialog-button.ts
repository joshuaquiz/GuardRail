export class DialogButton {
  public constructor(
    text: string,
    action: Function) {
    this.Text = text;
    this.Action = action;
  }

  public Text: string;

  public Action: Function;
}
