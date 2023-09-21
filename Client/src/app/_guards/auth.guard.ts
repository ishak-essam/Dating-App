import { CanActivate } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from '../services/account.service';
import { ToastrService } from 'ngx-toastr';
import { Injectable, Injector } from '@angular/core';
@Injectable({
  providedIn: 'root'
})
export class authGuard implements CanActivate {
  constructor(private authservice: AccountService, private toast: ToastrService) { }
  canActivate(): Observable<boolean> {
    return this.authservice.CurrentUser$.pipe(map((ele => {
      if (ele) {
        return true
      }
      else {
        this.toast.error('error')
        return false
      }
    })))
  }

};
