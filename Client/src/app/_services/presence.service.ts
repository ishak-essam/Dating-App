import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from '../Interfaces/User';
import { BehaviorSubject, take } from 'rxjs';
import { Route, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubsUrl = environment.hubsUrl;
  private hubConnection?: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUserSource$ = this.onlineUserSource.asObservable();
  constructor(private toastr: ToastrService, private router: Router) { }
  CreatHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder().withUrl(this.hubsUrl + 'presence', {
      accessTokenFactory: () => user.token,
    })
      .withAutomaticReconnect()
      .build();
    this.hubConnection.start().catch(err => console.log(err));
    // UserIsOnline
    this.hubConnection.on("UserIsOnline", username => {
      this.onlineUserSource$.pipe(take(1)).subscribe(ele => this.onlineUserSource.next([...ele, username]));
    })
    this.hubConnection.on("UserIsOffline", username => {
      this.toastr.warning(username + " has disconnected")
      this.onlineUserSource$.pipe(take(1)).subscribe(ele => this.onlineUserSource.next(ele.filter(x => x !== username)));

    })
    this.hubConnection.on("GetOnlineUsers", usernames => {
      this.onlineUserSource.next(usernames);
    })
    this.hubConnection.on("NewMessageRecived", ({ username, knownAs }) => {
      console.log({ username, knownAs })
      this.toastr.info(knownAs + " sent you new message !click me").onTap.pipe(take(1)).subscribe(() => {
        this.router.navigateByUrl('/members/' + username + "?tab=Messages")
      })

    })
  }
  StopHubConnection() {
    this.hubConnection?.stop().catch(err => console.log(err));
  }
}
