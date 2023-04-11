import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
    didOpen: (toast) => {
      toast.addEventListener('mouseenter', Swal.stopTimer)
      toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
  })

  success(message: string) {
    this.toast.fire({
      icon: 'success',
      title: message
    })
  }

  error(message: string) {
    this.toast.fire({
      icon: 'error',
      title: message
    })
  }

  info(message: string) {
    this.toast.fire({
      icon: 'info',
      title: message
    })
  }

  warning(message: string) {
    this.toast.fire({
      icon: 'warning',
      title: message
    })
  }


  confirm(message: string) {
    return Swal.fire({
      title: message,
      showDenyButton: true,
      showCancelButton: false,
      confirmButtonText: 'Yes',
      denyButtonText: 'No',
      customClass: {
        actions: 'my-actions',
        cancelButton: 'order-1 right-gap',
        confirmButton: 'order-2',
        denyButton: 'order-3',
      }
    });
  }
}
