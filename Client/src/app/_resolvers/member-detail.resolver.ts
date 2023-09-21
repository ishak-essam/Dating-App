import { inject } from '@angular/core';
import { ActivatedRoute, ResolveFn } from '@angular/router';
import { MembersService } from '../_services/members.service';
import { Member } from '../_modules/member';

export function memberDetailResolver(route:any,state:any) {
  const MemberSeriveResolver = inject(MembersService);

  return MemberSeriveResolver.GetMember(route.paramMap.get("username")!);
}
