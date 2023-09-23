import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_modules/member';
import { MembersService } from 'src/app/_services/members.service';
import { NgxGalleryModule, NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from '@kolkov/ngx-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_modules/message';
import { PresenceService } from 'src/app/_services/presence.service';
import { AccountService } from 'src/app/services/account.service';
import { User } from 'src/app/Interfaces/User';
@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    TabsModule, NgxGalleryModule,
    GalleryModule,
    TimeagoModule,
    MemberMessagesComponent
  ],
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent
  member: Member = {} as Member;
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] | any = [];

  images: GalleryItem[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];
  user?: User;
  constructor(private memberserivce: MembersService, private MessagServicesTs: MessageService, private accountService: AccountService, private active: ActivatedRoute, public presence: PresenceService) {
    this.accountService.CurrentUser$.subscribe(user => {
      if (user)
        this.user = user
    });
  }
  ngOnDestroy(): void {
    this.MessagServicesTs.stopHubConnections();
  }
  ngOnInit(): void {
    this.loadMember()
    this.active.data.subscribe(ele => this.member = ele['member']);
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imagePercent: 100,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      },
    ];
    this.galleryImages = this.GetgalleryImages();
    this.GetImages();
    this.active.queryParamMap.subscribe((paramstab: any) => {
      console.log(paramstab.params)
      if (paramstab.params = {}) { }
      else
        paramstab('tab') && this.selectTabs(paramstab('tab'));
    })
    this.GetImages();
  }
  OnTabActivted(detail: TabDirective) {
    this.activeTab = detail;
    if (this.activeTab.heading === "Messages" && this.user) {
      this.loadMessages();
      this.MessagServicesTs.createHunConnections(this.user, this.member.userName);
    }
    else {
      this.MessagServicesTs.stopHubConnections();
    }
  }
  loadMessages() {
    if (this.member) {
      // this.MessagServicesTs.GetMessageThread(this.member.userName).subscribe((ele: any) => {
      //   this.messages = ele;
      // });
    }
  }
  selectTabs(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
    }
  }
  GetgalleryImages() {
    if (!this.member) {
      return;
    }
    const imageUrl = [];
    for (const image of (this.member?.photos)) {
      imageUrl.push({
        small: image.url,
        medium: image.url,
        big: image.url
      })
    }
    return imageUrl;
  }
  Username!: any;
  loadMember() {
    this.active.params.subscribe((ele: any) => {
      this.Username = ele.name;
    })
    this.memberserivce.GetMember(this.Username)
      .subscribe((ele: any) => {
        this.member = ele
        this.GetImages();
        this.GetgalleryImages
        this.galleryImages = this.GetgalleryImages();
      })

  }
  GetImages() {
    if (!this.member) {
      return;
    }
    for (const PH of (this.member?.photos)) {
      this.images.push(new ImageItem({ src: PH.url, thumb: PH.url }));
      this.images.push(new ImageItem({ src: PH.url, thumb: PH.url }));
    }
  }
}
