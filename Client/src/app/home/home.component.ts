import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  constructor() { }
  ngOnInit(): void {
  }
  users: any;
  Register = false;
  RegisterToggle() {
    this.Register = !this.Register;
  }
  CancelFunc(bool: boolean) {

  }
}
