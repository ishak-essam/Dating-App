import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { User } from 'src/app/Interfaces/User';
import { Pagination } from 'src/app/_modules/Pagination';
import { Member } from 'src/app/_modules/member';
import { UserParams } from 'src/app/_modules/userParams';

import { MembersService } from 'src/app/_services/members.service';
import { AccountService } from 'src/app/services/account.service';
@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members$: Observable<Member[]> | undefined;
  pagination: Pagination | undefined;
  userparams: UserParams | undefined;
  gender = 'male';
  genderList = [{ value: 'male', display: 'Males' }, { value: 'female', display: 'Females' }];
  members: Member[] = [];
  ngOnInit(): void {
    this.loadMember();
  }
  constructor(private MemberService: MembersService) {
    this.userparams = this.MemberService.getUserParams();
  }

  loadMember() {
    if (this.userparams) {
      this.MemberService.setUserParams(this.userparams);
      this.MemberService.getMembers(this.userparams).subscribe((ele: any) => {
        if (ele?.result && ele.pagination) {
          this.members = ele?.result;
          this.pagination = ele?.pagination;
        }
      })
    }
  }
  pageChanged(event: any) {
    if (this.userparams && this.userparams?.pageNumber !== event.page) {
      this.userparams.pageNumber = event.page;
      this.MemberService.setUserParams(this.userparams);
      this.loadMember()
    }
  }
  restFilter() {
    this.userparams = this.MemberService.restUserParams();
    this.loadMember()
  }
}