import { Injectable } from '@angular/core'
import CountryList from 'country-list-with-dial-code-and-flag'
import CountryFlagSvg from 'country-list-with-dial-code-and-flag/dist/flag-svg'

@Injectable({
    providedIn: 'root'
})
export class HelperService {
    constructor() {}

    generateRandomNumber() {
        const minNumber = 100000
        const maxNumber = 900000
        return Math.floor(minNumber + Math.random() * maxNumber)
    }

    getCountriesList() {
        const countryList = CountryList.getAll().sort((a, b) => Number(a.dial_code.substring(1)) - Number(b.dial_code.substring(1)))
        countryList.forEach((country: any) => {
            country.imageSrc = this.getCountryImage(country.code)
        })
        return countryList
    }

    getCountryImage(code: string) {
        const svg = CountryFlagSvg[code]
        const blob = new Blob([svg], { type: 'image/svg+xml' })
        return URL.createObjectURL(blob)
    }
}
