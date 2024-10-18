import { NgModule } from '@angular/core'
import { CommonModule } from '@angular/common'
import { TranslateModule } from '@ngx-translate/core'
import { RouterModule } from '@angular/router'
import { InfoComponent } from './info/info.component'
import { FooterComponent } from './footer/footer.component'
import { HeaderComponent } from './header/header.component'
import { FooterLinksComponent } from './footer-links/footer-links.component'
import { UserInfoComponent } from './user-info/user-info.component'

@NgModule({
    declarations: [InfoComponent, FooterLinksComponent, FooterComponent, HeaderComponent, UserInfoComponent],
    imports: [CommonModule, RouterModule, TranslateModule.forChild()],
    exports: [InfoComponent, FooterLinksComponent, FooterComponent, HeaderComponent, UserInfoComponent, CommonModule, TranslateModule]
})
export class SharedModule {}
