import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from './../services/account.service';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {}
  registerform: FormGroup = new FormGroup({});
  ValidationError = [];
  maxDate: Date = new Date();

  constructor(private account: AccountService, private toast: ToastrService, private router: Router, private fb: FormBuilder) { }
  ngOnInit(): void {
    this.IntilizeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18)
  }
  @Output() CancelRequest = new EventEmitter();
  register() {
    const dob=this.GetDateOnly(this.registerform.controls['dateofBirth'].value);
    const values={...this.registerform.value,dateofBirth:dob};
    console.log(this.registerform.value)
    console.log(values)
    this.account.Register(values).subscribe(ele => {
      console.log(ele)
      this.toast.success('Done')
      this.router.navigate(['/members'])
      this.cancel()
    }, (ele: any) => {
      console.log(ele)
      console.log(ele)
      this.ValidationError = ele
      this.toast.error(ele.error);
    });
  }
  IntilizeForm() {
    // this.registerform = new FormGroup({
    //   userName: new FormControl('', Validators.required),
    //   password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
    //   Confirmpassword: new FormControl('', [Validators.required, this.MatchValue('password')])
    // })


    this.registerform = this.fb.group({
      userName: ['', Validators.required],
      gender: ['male'],
      knownAs: ['', Validators.required],
      dateofBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      Confirmpassword: ['', [Validators.required, this.MatchValue('password')]]
    })
    this.registerform.controls['password'].valueChanges.subscribe(
      () => {
        this.registerform.controls['Confirmpassword'].updateValueAndValidity()
      }
    )
  }
  cancel() {
    this.CancelRequest.emit(false)
  }
  GetDateOnly(dob: string | undefined) {
    if (!dob) return;
    let TheDob = new Date(dob);
    return new Date(TheDob.setMinutes(TheDob.getMinutes()-TheDob.getTimezoneOffset())).toISOString().slice(0,10)
  }
  MatchValue(MatchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(MatchTo)?.value ? null : { notMatching: true }
    }
  }
}
