import { Injectable } from '@angular/core'
import { FormGroup, AbstractControl } from '@angular/forms'

@Injectable({
    providedIn: 'root'
})
export class FormService {
    constructor() {}

    isInvalid(control: AbstractControl): boolean {
        const form = <FormGroup>control
        return form.invalid && form.touched
    }
}
