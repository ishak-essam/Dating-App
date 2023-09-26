import { Component, OnInit } from '@angular/core';
import { AccountService } from '../services/account.service';
import { User } from '../Interfaces/User';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  models: any = {};
  IsLogin: boolean = false;
  UserCurrent!: any;
  User!: any;
  ngOnInit(): void {
    this.GetCurrentUser()
  }
  CurrentUser!: any;
  constructor(private http: HttpClient, private accountService: AccountService, private router: Router, private toastr: ToastrService) {
    this.CurrentUser = accountService.CurrentUser$
  }
  submit() {
    this.accountService.Login(this.models).subscribe({
      next: (ele) => {
        this.router.navigateByUrl('/members')
        this.IsLogin = true
        this.models = {};
      }, error: (err: any) => {
        console.log(err)
        this.toastr.error(err.error)
      }
    })
    console.log(this.models)
  }
  GetCurrentUser() {
    this.accountService.CurrentUser$.subscribe({
      next: (ele: any) => {
        this.IsLogin = !!ele
        this.User = ele;
        this.UserCurrent = ele?.userName
      },
      error: (err) => { console.log(err) }
    })
  }
  logout() {
    this.accountService.Logout();
    this.IsLogin = false
    this.router.navigateByUrl('/')
  }

  NotFound() {
    this.router.navigateByUrl('NoServerFound')

  }
}
