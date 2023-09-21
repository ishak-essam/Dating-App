import { CanActivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

export const preventUnsaveChangesGuard: CanActivateFn = (component: any) => {
  if (component.editform?.dirty) {
    return confirm("Are you to continue ? Changes don't save ")
  }
  return true;
};
