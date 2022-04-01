import styles from './Notification.module.css';
import Swal from 'sweetalert2'
//site for the notification package, many options and customizations available
//https://sweetalert2.github.io/

//can wait for user response if a decision is needed
//const userResponse = await Notifications.error('Device Type not selected', 'A device Type must be selected to continue',);

//or fire and forget
//Notifications.error...

/*
* base notification elemet to be wrapped by the exported functions
*/
const notification = Swal.mixin({
    customClass: {
      confirmButton: `btn ${styles.proceedButton}`,
      cancelButton: `btn btn-danger ${styles.cancelButton}`,
  },
  buttonsStyling: false
})

/*
* displays an error message
*/
export async function error(title, message){
  return await notification.fire({
        title: title,
        text: message,
        icon: 'error',
        confirmButtonText: 'Dismiss'
  })
};

/*
* displays a success message
*/
export async function success(title, message){
  return await notification.fire({
        title: title,
        text: message,
        icon: 'success',
        confirmButtonText: 'Dismiss'
  })
};

/*
* message is an array of strings to support multiple items needing a warning
*array items need to have <br> inserted where line breaks are required
*/
export async function multiWarning(title, message){
  return await notification.fire({
        title: title,
        html: message.join(''),
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Proceed',
        cancelButtonText: 'Cancel'
  })
};

/*
* message is a single string for single error message
*/
export async function warning(title, message){
  return await notification.fire({
        title: title,
        text: message,
        icon: 'warning',
        confirmButtonText: 'Proceed',
        cancelButtonText: 'Cancel'
  })
};