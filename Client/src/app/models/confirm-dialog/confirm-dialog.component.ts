import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css']
})
export class ConfirmDialogComponent {
  title = ""
  message = ""
  btnOkText = ""
  btnCancleText = ""
  result = false
  constructor( public bsmodel: BsModalRef) {
  }

  confirm() {
    this.result = true;
    this.bsmodel.hide();
  }
  decline() {
    this.bsmodel.hide();
   }
}
