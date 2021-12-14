Ext.define("ExtDirectDemo.view.FormExample", {
    extend: "Ext.form.Panel",
    alias: "widget.FormExample",
    items: [
        {
            xtype: "textfield",
            name: "lastName",
            fieldLabel: "Last Name",
            allowBlank: false
        },
        {
            xtype: "textfield",
            name: "firstName",
            fieldLabel: "First Name",
            allowBlank: false
        },
        {
            xtype: "textfield",
            name: "givenName",
            fieldLabel: "Given Name",
            allowBlank: false
        },
        {
            fieldLabel: "File",
            xtype: "filefield",
            name: "file1"
        },
        {
            text: "Submit",
            xtype: "button",
            formBind: true, //only enabled once the form is valid
            disabled: true,
            handler: function () {
                var form = this.up("form").getForm();
                if (form.isValid()) {
                    form.submit({
                        success: function (form, action) {
                            Ext.Msg.alert("Success", action.result.result);
                        },
                        failure: function (form, action) {
                            Ext.Msg.alert("Failed", action.result.message);
                        },
                    });
                }
            },
        }
    ],
    height: 200,
    standardSubmit: false,
    buttons: [
        
    ],
    api: {
        submit: "Test.formSubmit"
    },
});
