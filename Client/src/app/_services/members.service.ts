import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_modules/member';
import { async, map, of, take } from 'rxjs';
import { UserParams } from '../_modules/userParams';
import { AccountService } from '../services/account.service';
import { User } from '../Interfaces/User';
import { GetPaginationHeader, GetPaginationResult } from './PaginationHelper';
import { Message } from '../_modules/message';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.BaseUrl;
  members: Member[] = [];
  memberCache = new Map();
  userparams: UserParams | undefined;
  user: User | undefined;
  constructor(private http: HttpClient, private account: AccountService) {
    this.account.CurrentUser$.subscribe((user2) => {
      if (user2) {
        this.userparams = new UserParams(user2);
        this.user = user2;
      }
    })

  }
  getUserParams() {
    return this.userparams;
  }
  setUserParams(params: UserParams) {
    this.userparams = params;
  }
  restUserParams() {
    if (this.user) {
      this.userparams = new UserParams(this.user);
      return this.userparams;
    }
    return;
  }
  /*
  getMembers(page?: number, itemsPerPage?: number) {
    let params = new HttpParams();
    if (page && itemsPerPage) {
      params = params.append('PageNumber', page)
      params = params.append('PageSize', itemsPerPage)
    }
    // if (this.members.length > 0) return of(this.members);
    // return this.http.get<Member[]>(this.baseUrl + 'users', { observe: 'response', params }).pipe(
    //   map((ele: any) => {
    //     this.members = ele;
    //     return this.members;
    //    })
    //    );
    return this.http.get<Member[]>(this.baseUrl + 'users', { observe: 'response', params }).pipe(
      map((response: any) => {
        this.paginationResulat.result = response.body;
        if (response?.body) {
          this.paginationResulat.result = response?.body;
        }
        const pagination = response.headers?.get('Pagination');
        if (pagination) {
          this.paginationResulat.pagination = JSON.parse(pagination);
        }
        return this.paginationResulat;
      })
    );
  };
    */
  getMembers(userparams: UserParams) {
    const response = this.memberCache.get(Object.values(userparams).join('-'));
    // if (response) return of(response)
    let params = GetPaginationHeader(userparams.pageNumber, userparams.pageSize);
    params = params.append('minAge', userparams.minAge)
    params = params.append('maxAge', userparams.maxAge)
    params = params.append('gender', userparams.gender)
    params = params.append('orderby', userparams.orderby)
    return GetPaginationResult<Member[]>(this.baseUrl + 'users', params, this.http).pipe(map(response => {
      this.memberCache.set(Object.values(userparams).join('-'), response);
      return response;
    }));
  };

  GetMember(UserName: string) {
    const member = [...this.memberCache.values()].reduce((arr, elum) => arr.concat(elum.result), []).find((member: Member) => member.userName = UserName);
    // if (member) return of(member);
    return this.http.get(this.baseUrl + 'Users/' + UserName);
  }
  PutMember(member: Member) {
    return this.http.put(this.baseUrl + 'Users', member).pipe(map((ele) => {
      const index = this.members.indexOf(member);
      this.members[index] = { ...this.members[index], ...member };
    }));
  }
  SetMainPhoto(photoid: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoid, {});
  }
  DeletePhoto(photoid: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoid, {});
  }
  AddLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});

  }
  GetLike(predicate: string, pageNumber: number, pageSize: number) {
    let params = GetPaginationHeader(pageNumber, pageSize);
    params = params.append("predicate", predicate);
    // return this.http.get<Member[]>(this.baseUrl + 'likes/?predicate=' + predicate);
    return GetPaginationResult<Member[]>(this.baseUrl + 'likes/?predicate=' + predicate, params, this.http);

  }
  async sendMessage(username: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'Messages', {
      "recipientUsername": username,
      "content": content
    });
  }

}
