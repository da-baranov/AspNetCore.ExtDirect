Ext.define("ExtDirectDemo.view.OrderedArgsExample", {
    extend: "Ext.panel.Panel",
    alias: "widget.OrderedArgsExample",
    title: "Calling a remote function with ordered arguments",
    layout: {
        type: "vbox",
    },
    padding: 10,
    items: [
        {
            xtype: "textfield",
            reference: "txtO1",
            fieldLabel: "Argument 1 (string)",
            labelAlign: "top",
            bind: {
                value: "{orderedArguments.a}"
            },
            margin: 2
        },
        {
            xtype: "numberfield",
            reference: "txtO2",
            fieldLabel: "Argument 2 (number)",
            labelAlign: "top",
            bind: {
                value: "{orderedArguments.b}",
            },
            margin: 2
        },
        {
            xtype: "datefield",
            reference: "txtO3",
            fieldLabel: "Argument 3 (date)",
            labelAlign: "top",
            bind: {
                value: "{orderedArguments.c}"
            },
            margin: 2
        },
        {
            xtype: "button",
            fieldLabel: "",
            text: "Call",
            itemId: "cmdOrderedArguments",
            margin: 2
        }
    ]
});