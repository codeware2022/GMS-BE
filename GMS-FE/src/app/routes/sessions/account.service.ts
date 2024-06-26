import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Register } from './sessionModels/register';
import { environment } from '@env/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private http: HttpClient) { }

  register(model: Register)
  {
    return this.http.post(`${environment.appUrl}/api/account/register`, model);
  }
}
