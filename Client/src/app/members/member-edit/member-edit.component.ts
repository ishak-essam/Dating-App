import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { MembersService } from 'src/app/_services/members.service';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editform') editform: NgForm | undefined;
  @HostListener('window:beforeunload', ['$event']) preventChange($event: any) {
    if (this.editform?.dirty) {
      $event.returnValue = true;
    }
  }
  user!: any;
  member!: any;
  constructor(private account: AccountService, private members: MembersService, private toastr: ToastrService) {
  }
  ngOnInit(): void {
    this.account.CurrentUser$.pipe(take(1)).subscribe(ele => {
      this.user = ele;
    })
    this.loadMember()
  }
  loadMember() {
    console.log(this.user);
    this.members.GetMember(this.user?.userName).subscribe(ele => {
      console.log(this.member)
      this.member = ele;
      console.log(this.member)
    });
  }
  updatedMember() {
    this.members.PutMember(this.editform?.value).subscribe((ele: any) => {
      console.log(ele)
      this.toastr.success("Profile Updated Successfuly")
    })
  }
}
