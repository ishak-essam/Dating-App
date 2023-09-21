import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-model',
  templateUrl: './roles-model.component.html',
  styleUrls: ['./roles-model.component.css']
})
export class RolesModelComponent {

  username = '';
  availabelRoles: any[] = [];
  SelectRoles: any[] = [];
  constructor(public bsModalRef: BsModalRef) {
  }
  UpdateChecked(check: string) {
    const index = this.SelectRoles.indexOf(check)
    index !== -1 ? this.SelectRoles.splice(index, 1) : this.SelectRoles.push(check);
  }
}
