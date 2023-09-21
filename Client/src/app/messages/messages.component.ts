import { Component, OnInit } from '@angular/core';
import { Message } from '../_modules/message';
import { Pagination } from '../_modules/Pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  loading = false;
  message?: Message[];
  paginations?: Pagination;
  container = "Unread";
  pageNumber = 1;
  pageSize = 5;
  constructor(private messagesSerivce: MessageService) {

  }
  ngOnInit(): void {
  }
  deleteMessage(id: number) {
    this.messagesSerivce.DeleteMessage(id).subscribe(
      ele => {
        console.log(ele);
        this.message?.splice(this.message.findIndex(x => x.id == id), 1);
      }
    )
  }

  loadMessage() {
    this.loading = true;
    this.messagesSerivce.GetMessage(this.pageNumber, this.pageSize, this.container).subscribe((ele) => {
      this.message = ele.result;
      this.paginations = ele.pagination;
      this.loading = false;
      console.log(this.message)
      console.log(ele);
      console.log(this.loading);
    })
  }
  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessage();
    }
  }
}
