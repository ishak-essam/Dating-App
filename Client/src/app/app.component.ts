import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from './services/account.service';
import { User } from './Interfaces/User';
import { ToastrService } from 'ngx-toastr';
import { MembersService } from './_services/members.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'client';
  constructor(private Member: MembersService, private account: AccountService) {

  }
  ngOnInit(): void {
    this.SetCurrentUser();
    // this.Member.getMembers().subscribe(ele => console.log(ele))
  }
  SetCurrentUser() {
    const User = localStorage.getItem('user');
    if (!User) return;
    const UserInterface: User = JSON.parse(User);
    this.account.SetCurrentUser(UserInterface);
  }
}
