Ext.define("ExtDirectDemo.view.HelloExample", {
    extend: "Ext.panel.Panel",
    alias: "widget.HelloExample",

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
            itemId: "cmdHello",
            iconCls: "fa fa-comment-alt"
        }
    ]
});
