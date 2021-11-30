
Ext.define("ExtDirectDemo.view.PersonForm", {
    extend: "Ext.window.Window",
    title: "Add Person",
    modal: true,
    width: 600,
    height: 160,
    modalResult: "cancel",
    layout: {
        type: "vbox",
        align: "stretch"
    },
    bodyPadding: 10,
    controller: {

    },
    viewModel: {
        data: {
            person: {
                lastName: "John",
                firstName: "Doe",
                givenName: "Aaron"
            }
        }
    },
    items: [
        {
            xtype: "textfield",
            fieldLabel: "Last Name",
            bind: {
                value: "{person.lastName}"
            }
        },
        {
            xtype: "textfield",
            fieldLabel: "First Name",
            bind: {
                value: "{person.firstName}"
            }
        },
        {
            xtype: "textfield",
            fieldLabel: "Given Name",
            bind: {
                value: "{person.givenName}"
            }
        }
    ],
    fbar: [
        {
            text: "OK",
            handler: function () {
                this.up("window").modalResult = "ok";
                this.up("window").close();
            }
        },
        {
            text: "Cancel",
            handler: function () {
                this.up("window").modalResult = "cancel";
                this.up("window").close();
            }
        }
    ]
});