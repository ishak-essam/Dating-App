import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs';
import { User } from 'src/app/Interfaces/User';
import { AccountService } from 'src/app/services/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[] = [];
  user: User = {} as User;
  constructor(private viewcontainerRef: ViewContainerRef, private templateRef: TemplateRef<any>,
    private accountService: AccountService) {

    this.accountService.CurrentUser$.pipe(take(1)).subscribe((ele: any) => {
      if (ele) this.user = ele;
    })
  }
  ngOnInit(): void {
    if (this.user.roles.some(x => this.appHasRole.includes(x))) {
      this.viewcontainerRef.createEmbeddedView(this.templateRef);
    }
    else {
      this.viewcontainerRef.clear();
    }
  }

}
