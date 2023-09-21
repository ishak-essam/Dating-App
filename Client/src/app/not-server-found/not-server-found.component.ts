import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-not-server-found',
  templateUrl: './not-server-found.component.html',
  styleUrls: ['./not-server-found.component.css']
})
export class NotServerFoundComponent implements OnInit {
  err  !: any
  constructor(private router: Router) {
    const nav = this.router.getCurrentNavigation();
    this.err = nav?.extras?.state?.['err'];
  }
  ngOnInit(): void {

  }
}
