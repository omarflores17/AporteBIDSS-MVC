   /********************************************************************************************************
    -- NAME   :  generalfuntions
    -- PROPOSE:  show 
    REVISIONS:
    version              Date                Author                        Description
    ----------           -------------       ---------------               -------------------------------
    1.0                  19/11/2019          Marvin.Guillen                Creation of js
    ********************************************************************************************************/
function CerrarModal() {
    if (changes) {
        swal({
            title: "Cerrar",
            text: "Los datos que aun no se han guardado del formulario se perderan ¿Desea Continuar? ",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Aceptar",
            cancelButtonText: "Cancelar"
            //closeOnConfirm: false
        }, function () {
            $('#ModalCurrency').modal('hide');
            changes = false;
        });
    } else {
        $('#ModalCurrency').modal('hide');
        changes = false;
    }
};
