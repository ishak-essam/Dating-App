import { CanActivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { inject } from '@angular/core';
import { ConfirmService } from '../_services/confirm.service';

export const preventUnsaveChangesGuard: CanActivateFn = (component: any) => {
  const confirmServices=inject(ConfirmService)
  if (component.editform?.dirty) {
    return confirmServices.confirm();
  }
  return true;
};
