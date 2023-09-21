import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account.service';
import { map } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService)

  return accountService.CurrentUser$.pipe(map(user => {
    if (!user) return false;
    console.log(user)
    console.log(user.roles)
    if (user.roles.includes('Admin') || user.roles.includes('Moderator'))
      return true;
    else {
      toastr.error("U can't Access this area")
      return false;
    }
  }));
};
