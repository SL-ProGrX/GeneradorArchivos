import { Injectable } from '@angular/core';
import { throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import Swal from 'sweetalert2';
//import {  } from '../models/frmAF_Pruebas.models';

@Injectable({providedIn: 'root'
})

export class frmAF_PruebasService {

    private apiUrl = environment.apiUrl + 'frmAF_Pruebas/';
    constructor(private http: HttpClient) { }

    private obtenerHeaders(jwtToken: string): HttpHeaders {
        return new HttpHeaders({
            Authorization: 'Bearer ' + jwtToken,
            'Content-Type': 'application/json'
        });
    }

    handleError(error: any) {
        let errorMessage = '';
        console.log(error);
        if (error.error instanceof ErrorEvent) {
            // Get client-side error
            errorMessage = error.error.message;
        } else {
            // Get server-side error
            errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
        }
        //window.alert(errorMessage);
        Swal.fire({
            title: "Error de Servidor",
            html: 'Por favor envíe el siguiente mensaje al administrador del sistema: ' + '<b>' + errorMessage + '</b>',
            icon: "error",
            width: 600,
            padding: "3em",
            confirmButtonColor: "#28a745",
            confirmButtonText: "Aceptar",
        });
        return throwError(errorMessage);
    }

    // AQUI VAN LOS METODOS DEL SERVICIO

}
