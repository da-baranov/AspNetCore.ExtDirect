Ext.define("ExtDirectDemo.view.Chat", {
    extend: "Ext.panel.Panel",
    alias: "widget.Chat",
    layout: {
        type: "border",
    },
    items: [
        {
            xtype: "gridpanel",
            region: "center",
            title: "Talk to yourself",
            tbar: [
                {
                    xtype: "label",
                    text: "Say something:"
                },
                {
                    xtype: "textfield",
                    bind: {
                        value: "{chatMessage}"
                    }
                },
                {
                    xtype: "button",
                    text: "Send",
                    iconCls: "fa fa-envelope",
                    itemId: "cmdSendMessage"
                }
            ],
            columns: [
                {
                    text: "Date/time",
                    flex: 1,
                    dataIndex: "date",
                    format: "c"
                },
                {
                    text: "You said",
                    flex: 3,
                    dataIndex: "message"
                }
            ],
            bind: {
                store: "{chat}"
            }
        }
    ]
});