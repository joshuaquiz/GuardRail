import { Component, Input } from '@angular/core';

@Component({
  selector: 'TextInput',
  standalone: false,
  
  templateUrl: './text-input.component.html',
  styleUrl: './text-input.component.css'
})
export class TextInputComponent {
  @Input()
  public Label: string;

  @Input()
  public Value: string;
}
