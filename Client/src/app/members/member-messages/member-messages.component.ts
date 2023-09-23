import { CommonModule } from '@angular/common';
import { Component,ChangeDetectionStrategy, Input, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormsModule, NgForm, ReactiveFormsModule } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_modules/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection:ChangeDetectionStrategy.OnPush,
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
