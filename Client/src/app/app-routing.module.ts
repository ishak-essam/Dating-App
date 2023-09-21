import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { authGuard } from './_guards/auth.guard';
import { NotServerFoundComponent } from './not-server-found/not-server-found.component';
import { NotfoundComponent } from './notfound/notfound.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { preventUnsaveChangesGuard } from './_guards/prevent-unsave-changes.guard';
import { memberDetailResolver } from './_resolvers/member-detail.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { adminGuard } from './_guards/admin.guard';
const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      { path: 'members', component: MemberListComponent },
      { path: 'members/:name', component: MemberDetailComponent, resolve: { memeber: memberDetailResolver } },
      { path: 'edit', component: MemberEditComponent, canDeactivate: [preventUnsaveChangesGuard] },
      { path: 'lists', component: ListsComponent },
      { path: 'messages', component: MessagesComponent },
      { path: 'admin', component: AdminPanelComponent, canActivate: [adminGuard] },
      { path: 'NoServerFound', component: NotServerFoundComponent },
    ]
  },
  { path: 'notfound', component: NotfoundComponent },
  { path: '**', component: NotfoundComponent, pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
