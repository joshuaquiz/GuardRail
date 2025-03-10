import { Component, Input } from '@angular/core';

@Component({
  selector: 'NumberInput',
  standalone: false,
  
  templateUrl: './number-input.component.html',
  styleUrl: './number-input.component.css'
})
export class NumberInputComponent {
  @Input()
  public Label: string;

  @Input()
  public Value: string;
}
