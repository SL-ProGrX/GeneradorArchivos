
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Subject } from 'rxjs';
import { UsuarioLogeadoDTO } from '../../../models/logon.model';
import { frmAF_PruebasService } from '../../../services/proGrx_Beneficios/frmAF_Pruebas.service';

@Component({
    selector: 'app-frmAF_Pruebas',
    templateUrl: './frmAF_Pruebas.component.html',
    providers: [ConfirmationService, MessageService],
})

export class frmAF_Pruebas implements OnInit, OnDestroy {

    public _unsubscribeAll!: Subject<any>;
    public UsuarioSesion: UsuarioLogeadoDTO = new UsuarioLogeadoDTO();

    constructor(private router: Router,
        private _frmAF_PruebasSrv: frmAF_PruebasService,
        private messageService: MessageService) {
        this._unsubscribeAll = new Subject();
    }

    ngOnDestroy(): void {
        this._unsubscribeAll.unsubscribe();
    }

    ngOnInit(): void {
        if (sessionStorage.getItem('UsuarioInfo') != null) {
            this.UsuarioSesion = JSON.parse(sessionStorage.getItem('UsuarioInfo')!);
        }
        else {
            this.router.navigate(['auth/login']);
        }
    }

}