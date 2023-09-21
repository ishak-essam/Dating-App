import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormsModule, NgForm, ReactiveFormsModule } from '@angular/forms';
import { timepickerReducer } from 'ngx-bootstrap/timepicker/reducer/timepicker.reducer';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_modules/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule, FormsModule, ReactiveFormsModule]
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm;
  @Input() messages: Message[] = [];
  @Input() Username?: string;
  MessageContent = '';
  constructor(public messageService: MessageService) {
  }
  ngOnInit(): void {
  }
  sendMessage() {
    console.log(this.Username)
    if (!this.Username) return;
    this.messageService.sendMessage(this.Username, this.MessageContent)?.then((ele: any) => {
      this.messageForm?.reset();
    })
  }
}
