import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'dialog',
  standalone: false,
  
  templateUrl: './dialog.component.html',
  styleUrl: './dialog.component.css'
})
export class DialogComponent {
  @Output()
  public Close = new EventEmitter<void>();

  public CloseClicked(): void {
    this.Close.emit();
  }
}
