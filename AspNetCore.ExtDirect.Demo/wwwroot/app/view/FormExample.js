Ext.define("ExtDirectDemo.view.FormExample", {
    extend: "Ext.form.Panel",
    alias: "widget.FormExample",
    items: [
        {
            xtype: "textfield",
            name: "lastName",
            fieldLabel: "Last Name",
        },
        {
            xtype: "textfield",
            name: "firstName",
            fieldLabel: "First Name",
        },
        {
            xtype: "textfield",
            name: "givenName",
            fieldLabel: "Given Name",
        },
    ],
    height: 200,
    buttons: [
        {
            text: "Submit",
            formBind: true, //only enabled once the form is valid
            disabled: true,
            handler: function() {
                var form = this.up("form").getForm();
                if (form.isValid()) {
                    form.submit({
                        success: function(form, action) {
                            Ext.Msg.alert("Success", action.result.msg);
                        },
                        failure: function(form, action) {
                            Ext.Msg.alert("Failed", action.result.msg);
                        },
                    });
                }
            },
        },
    ],
    api: {
        submit: "Test.formSubmit",
    },
});
