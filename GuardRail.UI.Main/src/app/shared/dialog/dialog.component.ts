import { Component, EventEmitter, Output, Input } from '@angular/core';
import { DialogButton } from './dialog-button';

@Component({
  selector: 'dialog',
  standalone: false,
  
  templateUrl: './dialog.component.html',
  styleUrl: './dialog.component.css'
})
export class DialogComponent {
  @Output()
  public Close = new EventEmitter<void>();

  @Input()
  public Buttons!: DialogButton[];

  @Input()
  public Title!: string;

  public CloseClicked(): void {
    this.Close.emit();
  }
}
