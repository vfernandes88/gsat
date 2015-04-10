//  When invoked open the modal dialog box.
//  We will call this function from the Web Parts page.
function popupmodalui(url) {

    // Set the required properties.
    var options = { autoSize: true,
        title: "Busca de cargo",
        showClose: true,
        allowMaximize: true
    };
    // Pop up the application page in the modal dialog box, 
    // and pass the site URL as a query string to the application page.
    SP.UI.ModalDialog.commonModalDialogOpen("../_layouts/Globosat.Remuneracao.CustomPages/BuscaCargo.aspx?url="
      + url,

options, closecallback, null);
}

// Handles the click event for OK button on the modal dialog box.
// This function runs in the context of the application page.
function ModalOk_click() {

    // Get the value of the hidden text box on the modal dialog box.
    var value = getValueByClass('.modalhiddenfield');

    // Pass the hidden text box value to the callback and close the modal dialog box.
    SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.OK, value);
}

// Handles the click event for the Cancel button on the modal dialog box.
function ModalCancel_click() {

    // Set the dialog result property to Cancel and close the modal dialog box.
    SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.cancel, 'Cancel clicked');
}

// Executes when the modal dialog box is closed.
// This function runs in the context of the Web Part's page.
function closecallback(result, value) {

    // Determine whether the OK button was clicked.
    if (result === SP.UI.DialogResult.OK) {

        // Set the value of the hidden text box on the Web Part 
        // with the value passed by the OK button event.
        var ispostback = setValue('.webparthiddenfield', value);
        if (ispostback == true) {

            // Postback the page so the Web Part life cycle is reinitiated.
            postpage();
        }
    }
}

// Finds a control by CSS class name and retrieves its value.
function getValueByClass(className) {
    formtextBox = $(className);
    if (formtextBox != null) {
        return formtextBox.val();
    }
}

// Finds a control by CSS class name and sets its value.
// This function runs in the context of the Web Part's page.
function setValue(className, value) {
    hiddenfieldid = $(className);
    if (hiddenfieldid != null) {
        hiddenfieldid.val(value);
        /*For testing the modal dialog box with the CEWP. Can be removed.*/
        if (hiddenfieldid.css('visibility') == "visible") {
            return false;
        }
        /*--*/
        return true;
    }
}

// Check if the hidden text box on the modal dialog box is empty.
function checkTextChange() {
    value = jQuery.trim(getValueByClass('.modalhiddenfield'));

    // Enable the OK button on the modal window if the hidden text box has a value.
    if (value) {
        $('#btnModalOK').removeAttr('disabled');
    }
    // Disable the OK button on the modal window if the hidden text box does not have a value.
    else {
        $('#btnModalOK').attr("disabled", "true");
    }
}

// Postback the page to reinitiate the Web Part life cycle.
function postpage() {
    document.forms[0].submit();
}

// Look for a change every time the page is loaded.
$(document).ready(function () {
    checkTextChange();
});