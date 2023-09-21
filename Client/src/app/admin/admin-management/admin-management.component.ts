import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModelComponent } from 'src/app/models/roles-model/roles-model.component';
import { User } from 'src/app/Interfaces/User';

@Component({
  selector: 'app-admin-management',
  templateUrl: './admin-management.component.html',
  styleUrls: ['./admin-management.component.css']
})
export class AdminManagementComponent implements OnInit {
  BsModelRef: BsModalRef<RolesModelComponent> = new BsModalRef<RolesModelComponent>();
  users: any = [];
  roles: any;
  availabelRoles: any[] = [
    'Admin',
    'Moderator',
    'Member'
  ];
  constructor(private admin: AdminService, private modalService: BsModalService) {
  }
  ngOnInit(): void {
    this.GetUserWithRoles();
  }
  GetUserWithRoles() {
    this.admin.GetUserWithRoles().subscribe((ele: any) => {
      this.users = ele;
    })
  }

  openRoleMode(user: any) {
    this.BsModelRef.content!?.SelectRoles == user.role;
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        username: user.username,
        availabelRoles: this.availabelRoles,
        SelectRoles: [...user.role]
      }
    }
    this.BsModelRef = this.modalService.show(RolesModelComponent, config);
    this.BsModelRef.onHide?.subscribe(ele => {
      const selecteRole = this.BsModelRef.content?.SelectRoles;
      if (!this.ArrayEqual(selecteRole!, user.role)) {
        console.log(selecteRole)
        this.admin.UpdateUserRole(user.username, selecteRole).subscribe(ele => {
          this.GetUserWithRoles()
        });
      }
    })
  }
  private ArrayEqual(arr1: string[], arr2: string[]) {
    return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort())
  }
}
