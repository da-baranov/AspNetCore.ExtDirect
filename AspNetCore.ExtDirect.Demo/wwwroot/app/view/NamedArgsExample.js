Ext.define("ExtDirectDemo.view.NamedArgsExample", {
    extend: "Ext.panel.Panel",
    alias: "widget.NamedArgsExample",
    title: "Calling a remote function with named arguments",
    layout: {
        type: "vbox",
    },
    padding: 10,
    items: [
        {
            xtype: "textfield",
            reference: "txtN1",
            fieldLabel: "Argument a (string)",
            labelAlign: "top",
            bind: {
                value: "{namedArguments.a}"
            },
            value: "Some string",
            margin: 2
        },
        {
            xtype: "numberfield",
            reference: "txtN2",
            fieldLabel: "Argument b (double)",
            labelAlign: "top",
            bind: {
                value: "{namedArguments.b}"
            },
            margin: 2
        },
        {
            xtype: "numberfield",
            reference: "txtN3",
            fieldLabel: "Argument c (integer)",
            labelAlign: "top",
            decimalPrecision: 0,
            bind: {
                value: "{namedArguments.c}"
            },
            margin: 2
        },
        {
            xtype: "button",
            text: "Call",
            itemId: "cmdNamedArguments",
            margin: 2
        }
    ]
});