import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../Interfaces/User';
import { environment } from 'src/environments/environment.development';
import { PresenceService } from '../_services/presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  BaseUrl: string = environment.BaseUrl;
  UrlLogin = this.BaseUrl + 'Account/login';
  UrlRegister = this.BaseUrl + 'Account/register';
  private CurrentUserSource = new BehaviorSubject<User | null>(null);
  CurrentUser$ = this.CurrentUserSource.asObservable();
  constructor(private http: HttpClient, private presenceService: PresenceService) { }
  Login(Data: any) {
    return this.http.post<User>(this.UrlLogin, Data).pipe(map((respo: User) => {
      const user = respo;
      if (user) {
        this.CurrentUserSource.next(user);
        this.SetCurrentUser(user);
      }
    }));
  }
  SetCurrentUser(user: User) {
    user.roles = [];
    const roles = this.GetDecodeToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.CurrentUserSource.next(user);
    this.presenceService.CreatHubConnection(user);
  }
  Register(model: any) {
    return this.http.post<User>(this.UrlRegister, model).pipe(
      map((ele: any) => {
        if (ele) {
          this.CurrentUserSource.next(ele)
        }
      })
    )
  }
  Logout() {
    localStorage.removeItem('user');
    this.CurrentUserSource.next(null);
    this.presenceService.StopHubConnection();
  }
  GetDecodeToken(token: string) {

    return JSON.parse(atob(token.split('.')[1]));
  }
}
