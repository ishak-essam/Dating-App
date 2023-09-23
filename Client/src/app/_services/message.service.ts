import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { GetPaginationHeader, GetPaginationResult } from './PaginationHelper';
import { Message } from '../_modules/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../Interfaces/User';
import { BehaviorSubject, take } from 'rxjs';
import { Group } from '../_modules/group';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  BaseUrl = environment.BaseUrl;
  hubUrl = environment.hubsUrl;
  private hubConnection?: HubConnection;
  private messageThread = new BehaviorSubject<Message[]>([]);

  messagesThreadSource$ = this.messageThread.asObservable();

  constructor(private http: HttpClient) { }

  createHunConnections(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();
    console.log(user);
    console.log(otherUsername);
    this.hubConnection.start().catch(ele => console.log(ele));
    this.hubConnection.on("RecivedMessageThread", (ele) => {
      console.log(ele);
      this.messageThread.next(ele);
    })
    this.hubConnection.on("UpdatedGroup", (group: Group) => {
      if (group.Connection.some(x => x.connectionId == otherUsername)) {
        this.messagesThreadSource$.pipe(take(1)).subscribe((ele) => {
          ele.forEach(mes => {
            if (!mes.dateRead) {
              mes.dateRead = new Date(Date.now());
            }
          })
          this.messageThread.next([...ele]);
        });
      }
    })
    this.hubConnection.on("NewMessage", (message) => {
      this.messagesThreadSource$.pipe(take(1)).subscribe(messages => {
        this.messageThread.next([...messages, message])
      });

    })

  }
  stopHubConnections() {
    if (this.hubConnection)
      this.hubConnection?.stop();
  }
  GetMessage(pageNumber: number, pageSize: number, container: string) {
    let params = GetPaginationHeader(pageNumber, pageSize);
    params = params.append("Container", container);
    return GetPaginationResult<Message[]>(this.BaseUrl + 'messages', params, this.http)
  }

  GetMessageThread(username: string) {
    return this.http.get<Message[]>(this.BaseUrl + 'messages/thread/' + username);
  }
  DeleteMessage(id: number) {
    return this.http.delete(this.BaseUrl + 'messages/' + id);
  }
  async sendMessage(username: string, content: string) {
    return this.hubConnection?.invoke("SendMessage", {
      "recipientUsername": username,
      "content": content
    }).catch(ele => console.log(ele));
  }
}
