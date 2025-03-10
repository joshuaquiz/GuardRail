import { Component, Input } from '@angular/core';

@Component({
  selector: 'InputBase',
  standalone: false,
  
  templateUrl: './input-base.component.html',
  styleUrl: './input-base.component.css'
})
export class InputBaseComponent {
  @Input()
  public Label: string;

  @Input()
  public Type: string;

  @Input()
  public Value: string;
}
