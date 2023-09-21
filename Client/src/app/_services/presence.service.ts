import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from '../Interfaces/User';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubsUrl = environment.hubsUrl;
  private hubConnection?: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUserSource$ = this.onlineUserSource.asObservable();
  constructor(private toastr: ToastrService) { }
  CreatHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder().withUrl(this.hubsUrl + 'presence', {
      accessTokenFactory: () => user.token,
    })
      .withAutomaticReconnect()
      .build();
    this.hubConnection.start().catch(err => console.log(err));
    // UserIsOnline
    this.hubConnection.on("UserIsOnline", username => {
      this.toastr.info(username + " has Connected")
    })
    this.hubConnection.on("UserIsOffline", username => {
      this.toastr.warning(username + " has disconnected")
    })
    this.hubConnection.on("GetOnlineUsers", usernames => {
      this.onlineUserSource.next(usernames);
    })
  }
  StopHubConnection() {
    this.hubConnection?.stop().catch(err => console.log(err));
  }
}
