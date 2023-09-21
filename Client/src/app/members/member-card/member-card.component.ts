import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_modules/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class MemberCardComponent implements OnInit {
  @Input() Member: Member | undefined;
  constructor(private MemberService: MembersService, private toast: ToastrService , 
    public presence:PresenceService) {

  }
  ngOnInit(): void {
  }

  AddLike(member: Member) {
    this.MemberService.AddLike(member.userName).subscribe((ele) => {
      this.toast.success("U have Liked" + member.knownAs);
    })
  }
}
