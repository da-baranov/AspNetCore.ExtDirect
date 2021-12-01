Ext.define("ExtDirectDemo.view.MakePersonNameExample", {
    extend: "Ext.panel.Panel",
    alias: "widget.MakePersonNameExample",
    layout: {
        type: "vbox",
        align: "stretch"
    },
    items: [
        {
            xtype: "combobox",
            fieldLabel: "Prefix",
            labelAlign: "top",
            bind: {
                value: "{personName.prefix}"
            },
            value: "Some string",
            margin: 2,
            store: ["Mr.", "Ms.", "Mrs."]
        },
        {
            xtype: "textfield",
            fieldLabel: "First name",
            labelAlign: "top",
            bind: {
                value: "{personName.firstName}"
            },
            margin: 2
        },
        {
            xtype: "textfield",
            fieldLabel: "Last name",
            labelAlign: "top",
            decimalPrecision: 0,
            bind: {
                value: "{personName.lastName}"
            },
            margin: 2
        },
        {
            xtype: "button",
            text: "Call",
            itemId: "cmdPersonName",
            margin: 2
        }
    ]
});