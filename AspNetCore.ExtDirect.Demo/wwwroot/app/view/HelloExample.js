Ext.define("ExtDirectDemo.view.HelloExample", {
    extend: "Ext.panel.Panel",
    alias: "widget.HelloExample",
    title: "Hello example",

    layout: {
        type: "hbox"
    },
    items: [
        {
            xtype: "textfield",
            reference: "txtName",
            bind: {
                value: "{name}"
            },
            fieldLabel: "What's your name?",
            labelWidth: 150
        },
        {
            xtype: "button",
            text: "Say hello",
            itemId: "cmdHello"
        }
    ]
});
