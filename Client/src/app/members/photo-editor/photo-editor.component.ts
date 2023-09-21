import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { User } from 'src/app/Interfaces/User';
import { Photo } from 'src/app/_modules/Photo';
import { Member } from 'src/app/_modules/member';
import { MembersService } from 'src/app/_services/members.service';
import { AccountService } from 'src/app/services/account.service';
import { environment } from 'src/environments/environment';
@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member | undefined;
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver = false;
  baseUrl = environment.BaseUrl;
  user: User | undefined;
  constructor(private Accountservices: AccountService, private MemberServices: MembersService) {
    this.Accountservices.CurrentUser$.pipe(take(1)).subscribe(ele => {
      if (ele) this.user = ele;
    })
  }
  ngOnInit(): void {
    this.initializeUploader();
  }
  fileOverBase(Obj: any) {
    this.hasBaseDropZoneOver = Obj;
  }
  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer '+this.user?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    }
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      console.log(response)
      const photo = JSON.parse(response);
      this.member?.photos.push(photo);
      if (photo.isMain && this.member && this.user) {
        this.user.photoUrl = photo.url;
        this.member.photoUrl = photo.url;
        this.Accountservices.SetCurrentUser(this.user);
      }
    }
  }
  SetPhoto(photo: Photo) {
    console.log(this.member)
    this.MemberServices.SetMainPhoto(photo.id).subscribe(ele => {
      if (this.user && this.member) {
        this.user.photoUrl = photo.url;
        this.Accountservices.SetCurrentUser(this.user);
        this.member.photoUrl = photo.url;
        this.member.photos.forEach(ele => {
          if (ele.isMain) ele.isMain = false;
          if (ele.id == photo.id) ele.isMain = true;
        })
      }
    });
  }
  DeletePhoto(id: number) {
    this.MemberServices.DeletePhoto(id).subscribe((ele) => {
      if (this.member) {
        this.member.photos = this.member.photos.filter(rr => rr.id !== id);
        console.log(this.member.photos.filter(rr => rr.id !== id));
      }
    })
  }

}
