import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { User } from '../Interfaces/User';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.BaseUrl;
  constructor(private http: HttpClient) { }
  GetUserWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'Admin/users-with-roles')
  }
  UpdateUserRole(username: string, role: any) {
    console.log(role)
    console.log(username)
    return this.http.post<string[]>(this.baseUrl + 'Admin/edit-roles/' + username + '?roles=' + role, {})
  }
}
