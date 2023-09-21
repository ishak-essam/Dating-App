import { Component, OnInit } from '@angular/core';
import { Member } from '../_modules/member';
import { MembersService } from '../_services/members.service';
import { Pagination } from '../_modules/Pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  members: Member[] | undefined;
  predicate = "liked";
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination | undefined
  constructor(private MemberService: MembersService) { }
  ngOnInit(): void {
    this.loadLiked()
  }
  loadLiked() {

    this.MemberService.GetLike(this.predicate, this.pageNumber, this.pageSize).subscribe(ele => {
      this.members = ele.result;
      this.pagination = ele.pagination;
    })
  }
  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadLiked()
    }
  }
}
